using System.Drawing;
using ProcessDoctor.Backend.Core;

namespace ProcessDoctor.Imaging.Extensions;

public static class ProcessModelExtensions
{
    public static Bitmap? ExtractAssociatedBitmap(this ProcessModel process)
    {
        if (string.IsNullOrWhiteSpace(process.ExecutablePath))
        {
            return null;
        }

        return Icon
            .ExtractAssociatedIcon(process.ExecutablePath)?
            .ToBitmap();
    }
}
