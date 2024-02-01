using System.Runtime.InteropServices;

namespace ProcessDoctor.Backend.Windows.NT;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct PEB_64
{
    public fixed byte Reserved1[2];

    public byte BeingDebugged;

    public fixed byte Reserved2[21];

    public IntPtr LoaderData;

    public IntPtr ProcessParameters;

    public fixed byte Reserved3[520];

    public IntPtr PostProcessInitRoutine;

    public fixed byte Reserved4[136];

    public uint SessionId;
}
