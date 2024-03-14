using System.Runtime.InteropServices;

namespace ProcessDoctor.Backend.Windows.Imaging.Native;

internal static class Shell32
{
    [DllImport("shell32.dll")]
    public static extern HRESULT SHGetStockIconInfo(IconType siid, IconFlags uFlags, ref SH_STOCK_ICON_INFO psii);
}
