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

    internal static T ReadStructure<T>(this Kernel32.SafeObjectHandle processHandle, IntPtr address)
        where T : unmanaged
    {
        var instance = default(T);
        var span = MemoryMarshal.CreateSpan(ref instance, length: 1);
        var buffer = MemoryMarshal.AsBytes(span);

        if (!processHandle.TryReadMemory(address, buffer))
        {
            throw new CorruptedProcessHandleException();
        }

        return instance;
    }

    internal static unsafe T ReadStructure<T>(this Kernel32.SafeObjectHandle processHandle, void* address)
        where T : unmanaged
        => processHandle.ReadStructure<T>(new IntPtr(address));

    private static unsafe bool TryReadMemory(this Kernel32.SafeObjectHandle processHandle, IntPtr address, Span<byte> buffer)
    {
        var length = (nuint)buffer.Length;

        fixed (byte* pointer = buffer)
        {
            var isSuccess = Kernel32.ReadProcessMemory(
                processHandle,
                address,
                (IntPtr)pointer,
                length,
                out var totalLength);

            return isSuccess && length == totalLength;
        }
    }
}
