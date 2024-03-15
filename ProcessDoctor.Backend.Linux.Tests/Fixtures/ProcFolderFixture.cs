using System.IO.Abstractions.TestingHelpers;
using JetBrains.Annotations;

namespace ProcessDoctor.Backend.Linux.Tests.Fixtures;

[UsedImplicitly]
public sealed class ProcFolderFixture
{
    public MockFileSystem FileSystem { get; } = new();

    public ProcessFixture CreateProcess(uint id)
        => new(FileSystem, id);
}
