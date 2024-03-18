using System.IO.Abstractions;
using ProcessDoctor.Backend.Linux.Proc.Exceptions;
using ProcessDoctor.Backend.Linux.Proc.Extensions;
using ProcessDoctor.Backend.Linux.Proc.Interfaces;
using ProcessDoctor.Backend.Linux.Proc.StatusFile;

namespace ProcessDoctor.Backend.Linux.Proc;

public sealed class ProcessEntry : IProcessEntry
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

    public IProcessStatus Status { get; }

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
        var linkPath = directory
            .FileSystem
            .Path
            .Combine(directory.FullName, ProcPaths.ExecutablePath.FileName);

        if (!directory.FileSystem.Path.Exists(linkPath))
        {
            return null;
        }

        try
        {
            var linkTarget = directory
                .FileSystem
                .File
                .ResolveLinkTarget(linkPath, returnFinalTarget: false);

            return linkTarget?.FullName;
        }

        catch (UnauthorizedAccessException)
        {
            return null;
        }
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
