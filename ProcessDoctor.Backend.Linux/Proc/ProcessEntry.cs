using System.IO.Abstractions;
using System.Text;
using ProcessDoctor.Backend.Linux.Proc.Exceptions;
using ProcessDoctor.Backend.Linux.Proc.Extensions;
using ProcessDoctor.Backend.Linux.Proc.Native;
using ProcessDoctor.Backend.Linux.Proc.StatusFile;

namespace ProcessDoctor.Backend.Linux.Proc;

public sealed class ProcessEntry
{
    public static ProcessEntry Create(IDirectoryInfo directory)
    {
        if (!directory.IsProcess())
            throw new InvalidProcessDirectoryException(directory.FullName);

        return new ProcessEntry(directory);
    }

    public uint Id { get; }

    public string? CommandLine { get; }

    public string? ExecutablePath { get; }

    public ProcessStatus Status { get; }

    private ProcessEntry(IDirectoryInfo directory)
    {
        Id = uint.Parse(directory.Name);
        CommandLine = ReadCommandLine(directory);
        ExecutablePath = ReadExecutablePath(directory);
        Status = ReadStatus(directory);
    }

    private static string? ReadCommandLine(IDirectoryInfo directory)
    {
        var path = directory
            .FileSystem
            .Path
            .Combine(directory.FullName, ProcPaths.CommandLine.FileName);

        var value = directory
            .FileSystem
            .File
            .ReadAllText(path)
            .Replace('\0', ' ');

        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value;
    }

    private static string? ReadExecutablePath(IDirectoryInfo directory)
    {
        var path = directory
            .FileSystem
            .Path
            .Combine(directory.FullName, ProcPaths.ExecutablePath.FileName);

        var buffer = new byte[ProcPaths.ExecutablePath.MaxSize + 1];
        var count = LibC.ReadLink(path, buffer, ProcPaths.ExecutablePath.MaxSize);

        if (count <= 0)
            return null;

        buffer[count] = 0x0;

        return Encoding.UTF8.GetString(buffer, index: 0, count);
    }

    private static ProcessStatus ReadStatus(IDirectoryInfo directory)
    {
        var statusFile = directory
            .EnumerateFiles(ProcPaths.Status.FileName)
            .FirstOrDefault()
                ?? throw new StatusFileNotFoundException(directory.FullName);

        return ProcessStatus.Create(statusFile);
    }
}
