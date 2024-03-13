using System.Runtime.InteropServices;

namespace ProcessDoctor.Backend.Windows.Imaging.Native;

internal static class Shell32
{
    // For some reason this method is not supported by CsWin32: https://github.com/microsoft/CsWin32/issues/1159
    [DllImport("shell32.dll")]
    public static extern HRESULT SHGetStockIconInfo(IconType siid, IconFlags uFlags, ref SH_STOCK_ICON_INFO psii);
}
