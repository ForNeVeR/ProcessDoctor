using System.Drawing;
using System.Drawing.Imaging;
using SkiaSharp;

namespace ProcessDoctor.Backend.Windows.Imaging.Extensions;

public static class IconExtensions
{
    public static SKBitmap ToSkBitmap(this Icon icon)
    {
        using var nativeBitmap = icon.ToBitmap();

        using var stream = new MemoryStream();
        nativeBitmap.Save(stream, ImageFormat.Bmp);
        stream.Position = 0;

        return SKBitmap.Decode(stream);
    }
}
