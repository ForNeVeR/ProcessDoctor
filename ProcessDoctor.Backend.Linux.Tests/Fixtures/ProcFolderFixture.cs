using System.IO.Abstractions.TestingHelpers;
using JetBrains.Annotations;

namespace ProcessDoctor.Backend.Linux.Tests.Fixtures;

[UsedImplicitly]
public sealed class ProcFolderFixture
{
    public ProcessFixture CreateProcess(uint id)
        => new(new MockFileSystem(), id);
}
