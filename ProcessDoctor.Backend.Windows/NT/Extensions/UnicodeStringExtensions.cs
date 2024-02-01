using PInvoke;
using ProcessDoctor.Backend.Windows.Exceptions;

namespace ProcessDoctor.Backend.Windows.NT.Extensions;

internal static class UnicodeStringExtensions
{
    internal static unsafe string? ToManagedString(this NTDll.UNICODE_STRING unmanagedString, Kernel32.SafeObjectHandle processHandle)
    {
        if (unmanagedString.Length == 0 || unmanagedString.MaximumLength == 0)
        {
            return null;
        }

        if (unmanagedString.Length % 2 != 0)
        {
            // malformed UTF-16 string
            throw new CorruptedProcessHandleException();
        }

        var buffer = stackalloc char[unmanagedString.Length];
        var isSuccess = Kernel32.ReadProcessMemory(
            processHandle,
            unmanagedString.Buffer_IntPtr,
            (IntPtr)buffer,
            unmanagedString.Length,
            out var totalLength);

        if (!isSuccess || unmanagedString.Length != totalLength)
        {
            throw new CorruptedProcessHandleException();
        }

        var span = new ReadOnlySpan<char>(
            buffer,
            unmanagedString.Length / 2);

        return new string(span);
    }
}
