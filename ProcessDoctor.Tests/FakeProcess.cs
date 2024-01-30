using ProcessDoctor.Backend.Core;

namespace ProcessDoctor.Tests;

internal sealed record FakeProcess : SystemProcess
{
    /// <inheritdoc />
    public FakeProcess(uint id, uint? parentId, string name, string commandLine, string? executablePath)
        : base(id, parentId, name, commandLine, executablePath)
    { }
}
