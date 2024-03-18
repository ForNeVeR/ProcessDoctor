using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using ProcessDoctor.Backend.Linux.Proc;
using ProcessDoctor.Backend.Linux.Proc.Exceptions;
using ProcessDoctor.Backend.Linux.Proc.StatusFile;
using ProcessDoctor.Backend.Linux.Tests.Fixtures;

namespace ProcessDoctor.Backend.Linux.Tests.ProcTests;

public sealed class ProcessEntryTests(ProcFolderFixture procFolderFixture) : IClassFixture<ProcFolderFixture>
{
    [Theory]
    [InlineData("dir")]
    [InlineData("123dir")]
    [InlineData("dir123")]
    [InlineData("-123")]
    public void Should_throw_exception_if_directory_is_not_process(string directoryName)
    {
        // Arrange
        var processDirectory = new MockFileSystem()
            .DirectoryInfo
            .New(directoryName);

        // Act & Assert
        this.Invoking(_ => ProcessEntry.Create(processDirectory))
            .Should()
            .Throw<InvalidProcessDirectoryException>();
    }

    [Theory]
    [InlineData(123u)]
    [InlineData(1234u)]
    [InlineData(0u)]
    public void Should_read_process_id_properly(uint expectedId)
    {
        // Arrange & Act
        var processFixture = procFolderFixture.CreateProcess(expectedId);
        var sut = ProcessEntry.Create(processFixture.Directory);

        // Assert
        sut.Id
            .Should()
            .Be(expectedId);
    }

    [Theory]
    [InlineData("/usr/sbin/cron -f - P")]
    [InlineData("/sbin/init splash")]
    public void Should_read_process_command_line_properly(string expectedCommandLine)
    {
        // Arrange
        var processFixture = procFolderFixture.CreateProcess(123u);
        using (var writer = processFixture.CommandLineFile.CreateText())
            writer.Write(expectedCommandLine);

        // Act
        var sut = ProcessEntry.Create(processFixture.Directory);

        // Assert
        sut.CommandLine
            .Should()
            .Be(expectedCommandLine);
    }

    [Fact]
    public void Command_line_should_be_null_if_file_is_empty()
    {
        // Arrange
        var processFixture = procFolderFixture.CreateProcess(123u);

        // Act
        var sut = ProcessEntry.Create(processFixture.Directory);

        // Assert
        sut.CommandLine
            .Should()
            .BeNull();
    }

    [Theory]
    [InlineData("/ProcessDoctor")]
    [InlineData("/NET/ProcessDoctor")]
    public void Should_read_process_executable_path_properly(string expectedExecutablePath)
    {
        // Arrange
        var processFixture = procFolderFixture.CreateProcess(123u);

        processFixture
            .FileSystem
            .AddEmptyFile(expectedExecutablePath);

        processFixture
            .FileSystem
            .File
            .CreateSymbolicLink(processFixture.ExecutableLinkFile.FullName, expectedExecutablePath);

        // Act
        var sut = ProcessEntry.Create(processFixture.Directory);

        // Assert
        sut.ExecutablePath
            .Should()
            .Be(expectedExecutablePath);
    }

    [Fact]
    public void Executable_path_should_be_null_if_link_is_not_exists()
    {
        // Arrange
        var processFixture = procFolderFixture.CreateProcess(123u);

        // Act
        var sut = ProcessEntry.Create(processFixture.Directory);

        // Assert
        sut.ExecutablePath
            .Should()
            .BeNull();
    }

    [Fact]
    public void Should_read_process_status_section_properly()
    {
        // Arrange & Act
        var processFixture = procFolderFixture.CreateProcess(123u);
        using (var writer = processFixture.StatusFile.CreateText())
            writer.Write(
                """
                    Name: ProcessDoctor
                    ...
                    ...
                """);

        var sut = ProcessEntry.Create(processFixture.Directory);

        // Assert
        sut.Status
            .Should()
            .NotBeNull()
            .And
            .BeOfType<ProcessStatus>();
    }
}
