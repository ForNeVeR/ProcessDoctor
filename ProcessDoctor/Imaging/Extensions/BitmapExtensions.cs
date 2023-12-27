using System.Drawing.Imaging;
using System.IO;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace ProcessDoctor.Imaging.Extensions;

public static class BitmapExtensions
{
    public static Bitmap? ToAvaloniaBitmap(this System.Drawing.Bitmap? nativeBitmap)
    {
        if (nativeBitmap is null)
        {
            return null;
        }

        using var stream = new MemoryStream();
        nativeBitmap.Save(stream, ImageFormat.Bmp);
        stream.Position = 0;

        return new Bitmap(stream);
    }
}
