using System.Runtime.InteropServices;
using PInvoke;

namespace ProcessDoctor.Backend.Windows.NT;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct RTL_USER_PROCESS_PARAMETERS
{
    private fixed byte Reserved1[16];

    private fixed int Reserved2[20];

    /// <summary>The path of the image file for the process.</summary>
    public NTDll.UNICODE_STRING ImagePathName;

    /// <summary>The command-line string passed to the process.</summary>
    public NTDll.UNICODE_STRING CommandLine;
}
