using Gdk;
using GLib;
using Gtk;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Linux.Imaging;
using ProcessDoctor.Backend.Linux.Proc.Interfaces;
using SkiaSharp;

namespace ProcessDoctor.Backend.Linux;

public sealed record LinuxProcess : SystemProcess
{
    public static LinuxProcess Create(IProcessEntry processEntry)
        => new(
            processEntry.Id,
            processEntry.Status.ParentId,
            processEntry.Status.Name,
            processEntry.CommandLine,
            processEntry.ExecutablePath);

    /// <inheritdoc />
    private LinuxProcess(uint Id, uint? ParentId, string Name, string? CommandLine, string? ExecutablePath)
        : base(Id, ParentId, Name, CommandLine, ExecutablePath)
    { }

    /// <inheritdoc />
    public override SKBitmap ExtractIcon()
    {
        using var iconTheme = new IconTheme();

        if (string.IsNullOrWhiteSpace(ExecutablePath))
        {
            return ExtractStockIcon(iconTheme);
        }

        // TODO[#27]: Understand how to bind ExecutablePath and data from AppInfo
        var application = AppInfoAdapter
            .GetAll()
            .FirstOrDefault(application => application.Executable.Contains(ExecutablePath));

        if (application is null)
        {
            return ExtractStockIcon(iconTheme);
        }

        using var icon = iconTheme.LookupIcon(
            application.Icon,
            IconAttributes.DefaultIconSize,
            IconLookupFlags.UseBuiltin);

        using var buffer = icon.LoadIcon();
        return ExtractIconUsingPixbuf(buffer);
    }

    private SKBitmap ExtractStockIcon(IconTheme iconTheme)
    {
        using var icon = iconTheme.LookupIcon(
            ContentType.GetIcon(MimeTypes.Executable),
            IconAttributes.DefaultIconSize,
            IconLookupFlags.UseBuiltin);

        using var buffer = icon.LoadIcon();
        return ExtractIconUsingPixbuf(buffer);
    }

    private SKBitmap ExtractIconUsingPixbuf(Pixbuf buffer)
    {
        var iconProperties = new SKImageInfo(
            width: buffer.Width,
            height: buffer.Height,
            SKColorType.Rgba8888,
            SKAlphaType.Unpremul);

        return SKBitmap.FromImage(
            SKImage.FromPixels(
                iconProperties,
                buffer.Pixels));
    }
}
