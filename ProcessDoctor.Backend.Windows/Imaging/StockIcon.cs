using System.Drawing;
using System.Runtime.InteropServices;
using ProcessDoctor.Backend.Windows.Imaging.Native;

namespace ProcessDoctor.Backend.Windows.Imaging;

internal static class StockIcon
{
    private const string ErrorMessage = "An error occured while creating the stock icon: {0}";

    internal static Icon Create(IconType type)
    {
        var iconInformation = new SH_STOCK_ICON_INFO();
        iconInformation.cbSize = (uint)Marshal.SizeOf(iconInformation);

        var result = Shell32.SHGetStockIconInfo(type, IconFlags.Icon | IconFlags.SmallSize, ref iconInformation);

        if (result.Failed)
        {
            throw new InvalidOperationException(
                string.Format(ErrorMessage, type));
        }

        using var iconHandle = new DestroyIconSafeHandle(iconInformation.hIcon, ownsHandle: true);

        if (iconHandle.IsInvalid)
        {
            throw new InvalidOperationException(
                string.Format(ErrorMessage, type));
        }

        return (Icon)Icon.FromHandle(iconInformation.hIcon).Clone();
    }
}
