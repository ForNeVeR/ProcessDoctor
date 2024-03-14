using JetBrains.Annotations;

namespace ProcessDoctor.Backend.Linux.Tests;

[UsedImplicitly]
public sealed class ProcFileSystemFixture
{
    public ProcessFixture CreateProcess(uint id)
        => new(id);
}
