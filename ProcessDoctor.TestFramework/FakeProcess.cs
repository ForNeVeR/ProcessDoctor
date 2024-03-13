using ProcessDoctor.Backend.Core;
using SkiaSharp;

namespace ProcessDoctor.TestFramework;

public sealed record FakeProcess : SystemProcess
{
    /// <inheritdoc />
    public FakeProcess(uint id, uint? parentId, string name, string? commandLine, string? executablePath)
        : base(id, parentId, name, commandLine, executablePath)
    { }

    public override Task<SKBitmap?> ExtractIconAsync()
        => throw new NotImplementedException();
}
