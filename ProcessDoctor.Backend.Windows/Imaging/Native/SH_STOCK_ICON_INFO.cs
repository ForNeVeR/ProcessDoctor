using System.Runtime.InteropServices;

namespace ProcessDoctor.Backend.Windows.Imaging.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SH_STOCK_ICON_INFO
{
    public uint cbSize;

    public IntPtr hIcon;

    public int iSysIconIndex;

    public int iIcon;

    public fixed char szPath[260];
}
