using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using ProcessDoctor.Backend.Linux.Proc;

namespace ProcessDoctor.Backend.Linux.Tests.Fixtures;

public sealed class ProcessFixture
{
    public IDirectoryInfo Directory { get; }

    public IFileInfo CommandLineFile { get; }

    public IFileInfo ExecutablePathFile { get; }

    public IFileInfo StatusFile { get; }

    public ProcessFixture(MockFileSystem fileSystem, uint id)
    {
        var directoryPath = fileSystem.Path.Combine(ProcPaths.Path, id.ToString());
        var processDirectory = fileSystem.DirectoryInfo.New(directoryPath);
        fileSystem.AddDirectory(directoryPath);

        var exePath = fileSystem.Path.Combine(processDirectory.FullName, ProcPaths.ExecutablePath.FileName);
        var exeFile = fileSystem.FileInfo.New(exePath);
        fileSystem.AddEmptyFile(exeFile);

        var commandLinePath = fileSystem.Path.Combine(processDirectory.FullName, ProcPaths.CommandLine.FileName);
        var commandLineFile = fileSystem.FileInfo.New(commandLinePath);
        fileSystem.AddEmptyFile(commandLineFile);

        var statusPath = fileSystem.Path.Combine(processDirectory.FullName, ProcPaths.Status.FileName);
        var statusFile = fileSystem.FileInfo.New(statusPath);
        fileSystem.AddEmptyFile(statusFile);

        Directory = processDirectory;
        CommandLineFile = commandLineFile;
        ExecutablePathFile = exeFile;
        StatusFile = statusFile;
    }
}
