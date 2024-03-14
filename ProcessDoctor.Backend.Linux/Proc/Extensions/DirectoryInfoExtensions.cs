using System.IO.Abstractions;

namespace ProcessDoctor.Backend.Linux.Proc.Extensions;

public static class DirectoryInfoExtensions
{
    public static bool IsProcess(this IDirectoryInfo directory)
        => directory.Name.Length > 0 && directory.Name.All(char.IsDigit);
}
