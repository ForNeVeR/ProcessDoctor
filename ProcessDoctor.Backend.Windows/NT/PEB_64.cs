using System.Runtime.InteropServices;

namespace ProcessDoctor.Backend.Windows.NT;

[StructLayout(LayoutKind.Sequential)]
public struct PEB_64
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Reserved1;

    public byte BeingDebugged;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
    public byte[] Reserved2;

    public IntPtr LoaderData;

    public IntPtr ProcessParameters;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 520)]
    public byte[] Reserved3;

    public IntPtr PostProcessInitRoutine;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 136)]
    public byte[] Reserved4;

    public uint SessionId;
}
