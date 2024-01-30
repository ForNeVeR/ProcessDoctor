using System.Runtime.InteropServices;
using PInvoke;
using ProcessDoctor.Backend.Windows.Exceptions;

namespace ProcessDoctor.Backend.Windows.NT.Extensions;

internal static class ProcessHandleExtensions
{
    internal static NTDll.PROCESS_BASIC_INFORMATION GetBasicInformation(this Kernel32.SafeObjectHandle processHandle)
    {
        var length = Marshal.SizeOf<NTDll.PROCESS_BASIC_INFORMATION>();
        var pointer = Marshal.AllocHGlobal(length);

        var status = NTDll.NtQueryInformationProcess(
            processHandle,
            NTDll.PROCESSINFOCLASS.ProcessBasicInformation,
            pointer,
            length,
            out var totalLength);

        if (status.Value != NTSTATUS.Code.STATUS_SUCCESS || length != totalLength)
        {
            throw new CorruptedProcessHandleException();
        }

        var basicInformation = Marshal.PtrToStructure<NTDll.PROCESS_BASIC_INFORMATION>(pointer);

        Marshal.FreeHGlobal(pointer);

        return basicInformation;
    }
}