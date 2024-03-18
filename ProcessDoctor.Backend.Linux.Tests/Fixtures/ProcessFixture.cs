using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using ProcessDoctor.Backend.Linux.Proc;

namespace ProcessDoctor.Backend.Linux.Tests.Fixtures;

public sealed class ProcessFixture
{
    public MockFileSystem FileSystem { get; }

    public IDirectoryInfo Directory { get; }

    public IFileInfo CommandLineFile { get; }

    public IFileInfo ExecutableLinkFile { get; }

    public IFileInfo StatusFile { get; }

    public ProcessFixture(MockFileSystem fileSystem, uint id)
    {
        var directoryPath = fileSystem.Path.Combine(ProcPaths.Path, id.ToString());
        var processDirectory = fileSystem.DirectoryInfo.New(directoryPath);
        fileSystem.AddDirectory(directoryPath);

        var executableLinkPath = fileSystem.Path.Combine(processDirectory.FullName, ProcPaths.ExecutablePath.FileName);
        var executableLinkFile = fileSystem.FileInfo.New(executableLinkPath);

        var commandLinePath = fileSystem.Path.Combine(processDirectory.FullName, ProcPaths.CommandLine.FileName);
        var commandLineFile = fileSystem.FileInfo.New(commandLinePath);
        fileSystem.AddEmptyFile(commandLineFile);

        var statusPath = fileSystem.Path.Combine(processDirectory.FullName, ProcPaths.Status.FileName);
        var statusFile = fileSystem.FileInfo.New(statusPath);
        fileSystem.AddEmptyFile(statusFile);

        FileSystem = fileSystem;
        Directory = processDirectory;
        CommandLineFile = commandLineFile;
        ExecutableLinkFile = executableLinkFile;
        StatusFile = statusFile;
    }
}
