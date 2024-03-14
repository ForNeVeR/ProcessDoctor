using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Linux.Proc;
using SkiaSharp;

namespace ProcessDoctor.Backend.Linux;

public sealed record LinuxProcess : SystemProcess
{
    public static LinuxProcess Create(ProcessEntry processEntry)
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
        => throw new NotImplementedException();
}
