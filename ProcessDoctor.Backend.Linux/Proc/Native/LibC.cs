using System.Runtime.InteropServices;

namespace ProcessDoctor.Backend.Linux.Proc.Native;

internal static class LibC
{
    private const string Name = "libc";

    [DllImport(Name, EntryPoint = "readlink", SetLastError = true)]
    private static extern int NativeReadLink(string path, byte[] buffer, int bufferSize); // TODO[#26]: Make testable

    /// <remarks>
    /// The DllImportAttribute provides a SetLastError property
    /// so the runtime knows to immediately capture the last error and
    /// store it in a place that the managed code can read using Marshal.GetLastWin32Error.
    /// </remarks>
    internal static string? GetLastError()
        => Marshal.PtrToStringAnsi(StrError(Marshal.GetLastWin32Error()));

    [DllImport(Name, EntryPoint = "strerror", SetLastError = false)]
    private static extern IntPtr StrError(int errorCode);

    public static int ReadLink(string path, byte[] buffer, int bufferSize)
    {
        try
        {
            return NativeReadLink(path, buffer, bufferSize);
        }

        catch (DllNotFoundException)
        {
            return -1;
        }
    }
}
