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

        // TODO[#28]: Fix quality, color, size
        using var icon = iconTheme.LookupIcon(
            application.Icon,
            size: 16,
            IconLookupFlags.UseBuiltin);

        using var buffer = icon.LoadIcon();

        return SKBitmap.FromImage(
            SKImage.FromPixels(
                new SKImageInfo(width: 16, height: 16),
                buffer.Pixels));
    }

    private SKBitmap ExtractStockIcon(IconTheme iconTheme)
    {
        var icon = ContentType.GetIcon(MimeTypes.Executable);

        using var iconMetadata = iconTheme.LookupIcon(
            icon,
            size: 16,
            IconLookupFlags.UseBuiltin);

        return SKBitmap.Decode(iconMetadata.Filename);
    }
}
