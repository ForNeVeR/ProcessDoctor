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

        var file = FileFactory.NewForPath(ExecutablePath);

        using var fileMetadata = file.QueryInfo(
            IconAttributes.Standard,
            FileQueryInfoFlags.None,
            cancellable: null);

        using var iconMetadata = iconTheme.LookupIcon(
            fileMetadata.Icon,
            size: 16,
            IconLookupFlags.UseBuiltin);

        return SKBitmap.Decode(iconMetadata.Filename);
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
