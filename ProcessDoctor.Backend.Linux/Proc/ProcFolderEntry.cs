using System.IO.Abstractions;
using ProcessDoctor.Backend.Linux.Proc.Extensions;
using ProcessDoctor.Backend.Linux.Proc.Interfaces;

namespace ProcessDoctor.Backend.Linux.Proc;

public sealed class ProcFolderEntry(IFileSystem fileSystem) : IProcFolderEntry
{
    public IEnumerable<IDirectoryInfo> EnumerateProcessDirectories()
        => fileSystem
            .Directory
            .EnumerateDirectories(ProcPaths.Path)
            .Select(directoryPath => fileSystem.DirectoryInfo.New(directoryPath))
            .Where(DirectoryInfoExtensions.IsProcess);
}
