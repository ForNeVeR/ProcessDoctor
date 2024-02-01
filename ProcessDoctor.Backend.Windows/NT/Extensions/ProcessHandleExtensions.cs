using System.Runtime.InteropServices;
using PInvoke;
using ProcessDoctor.Backend.Windows.Exceptions;

namespace ProcessDoctor.Backend.Windows.NT.Extensions;

internal static class ProcessHandleExtensions
{
    internal static unsafe NTDll.PROCESS_BASIC_INFORMATION GetBasicInformation(this Kernel32.SafeObjectHandle processHandle)
    {
        var basicInformation = new NTDll.PROCESS_BASIC_INFORMATION();
        var length = Marshal.SizeOf(basicInformation);

        var status = NTDll.NtQueryInformationProcess(
            processHandle,
            NTDll.PROCESSINFOCLASS.ProcessBasicInformation,
            &basicInformation,
            length,
            out var totalLength);

        if (status.Value != NTSTATUS.Code.STATUS_SUCCESS || length != totalLength)
        {
            throw new CorruptedProcessHandleException();
        }

        return basicInformation;
    }
}
