using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PInvoke;

namespace ProcessDoctor.Backend.Windows.NT;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct RTL_USER_PROCESS_PARAMETERS
{
    private fixed byte Reserved1[16];

    private PVoidArray10 Reserved2;

    /// <summary>The path of the image file for the process.</summary>
    public NTDll.UNICODE_STRING ImagePathName;

    /// <summary>The command-line string passed to the process.</summary>
    public NTDll.UNICODE_STRING CommandLine;
}

[InlineArray(10)]
public struct PVoidArray10
{
    private IntPtr _element0;
}
