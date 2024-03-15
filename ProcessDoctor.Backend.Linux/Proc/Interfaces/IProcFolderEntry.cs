using System.IO.Abstractions;

namespace ProcessDoctor.Backend.Linux.Proc.Interfaces;

public interface IProcFolderEntry
{
    public IEnumerable<IDirectoryInfo> EnumerateProcessDirectories();
}
