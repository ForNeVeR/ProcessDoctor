using System.Runtime.InteropServices;
using PInvoke;

namespace ProcessDoctor.Backend.Windows.NT;

[StructLayout(LayoutKind.Sequential)]
public struct RTL_USER_PROCESS_PARAMETERS
{
    /// <summary>Reserved for internal use by the operating system.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    private readonly byte[] Reserved1;

    /// <summary>Reserved for internal use by the operating system.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    private readonly IntPtr[] Reserved2;

    /// <summary>The path of the image file for the process.</summary>
    public NTDll.UNICODE_STRING ImagePathName;

    /// <summary>The command-line string passed to the process.</summary>
    public NTDll.UNICODE_STRING CommandLine;
}
