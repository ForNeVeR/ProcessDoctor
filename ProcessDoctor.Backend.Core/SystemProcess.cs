using SkiaSharp;

namespace ProcessDoctor.Backend.Core;

public abstract record SystemProcess(
    uint Id,
    uint? ParentId,
    string Name,
    string? CommandLine,
    string? ExecutablePath)
{
    public abstract SKBitmap ExtractIcon();
}
