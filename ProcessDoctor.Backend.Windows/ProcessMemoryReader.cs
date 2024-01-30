using System.Runtime.InteropServices;
using PInvoke;

namespace ProcessDoctor.Backend.Windows;

internal sealed class ProcessMemoryReader
{
    private readonly Kernel32.SafeObjectHandle _processHandle;

    public ProcessMemoryReader(Kernel32.SafeObjectHandle processHandle)
        => _processHandle = processHandle;

    public T? ReadStructure<T>(IntPtr address)
        where T : struct
    {
        var length = Marshal.SizeOf<T>();
        var buffer = Marshal.AllocHGlobal(length);

        if (!TryRead(address, buffer, length))
        {
            return null;
        }

        var structure =  Marshal.PtrToStructure<T>(buffer);
        Marshal.FreeHGlobal(buffer);

        return structure;
    }

    public unsafe T? ReadStructure<T>(void* address)
        where T : struct
        => ReadStructure<T>(new IntPtr(address));

    public byte[] ReadBytes(IntPtr address, int length)
    {
        var buffer = Marshal.AllocHGlobal(length);

        if (!TryRead(address, buffer, length))
        {
            return Array.Empty<byte>();
        }

        var bytes = new byte[length];
        Marshal.Copy(buffer, bytes, startIndex: 0, length);
        Marshal.FreeHGlobal(buffer);

        return bytes;
    }

    private bool TryRead(IntPtr address, IntPtr buffer, int length)
    {
        var isSuccess = Kernel32.ReadProcessMemory(
            _processHandle,
            address,
            buffer,
            new UIntPtr((uint)length),
            out var totalLength);

        return isSuccess && length == totalLength.ToUInt32();
    }
}
