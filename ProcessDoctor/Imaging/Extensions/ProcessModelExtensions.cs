using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ProcessDoctor.Backend.Core;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace ProcessDoctor.Imaging.Extensions;

public static class ProcessModelExtensions
{
    public static Task<Bitmap?> ExtractAssociatedBitmapAsync(this ProcessModel process)
    {
        if (string.IsNullOrWhiteSpace(process.ExecutablePath))
        {
            return Task.FromResult<Bitmap?>(null);
        }

        return Task.Run(() =>
        {
            var nativeBitmap = Icon
                .ExtractAssociatedIcon(process.ExecutablePath)?
                .ToBitmap();

            if (nativeBitmap is null)
            {
                return null;
            }

            using var stream = new MemoryStream();
            nativeBitmap.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;

            return new Bitmap(stream);
        });
    }
}
