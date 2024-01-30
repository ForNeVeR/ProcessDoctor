using System.Runtime.InteropServices;

namespace ProcessDoctor.Backend.Windows.NT;

[StructLayout(LayoutKind.Sequential)]
public struct PEB_32
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Reserved1;

    public byte BeingDebugged;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public byte[] Reserved2;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public IntPtr[] Reserved3;

    public IntPtr Ldr;

    public IntPtr ProcessParameters;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public IntPtr[] Reserved4;

    public IntPtr AtlThunkSListPtr;

    public IntPtr Reserved5;

    public uint Reserved6;

    public IntPtr Reserved7;

    public uint Reserved8;

    public uint AtlThunkSListPtr32;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 45)]
    public IntPtr[] Reserved9;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
    public byte[] Reserved10;

    public IntPtr PostProcessInitRoutine;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
    public byte[] Reserved11;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public IntPtr[] Reserved12;

    public uint SessionId;
}
