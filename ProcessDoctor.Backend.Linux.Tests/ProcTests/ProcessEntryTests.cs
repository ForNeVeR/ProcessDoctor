using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using ProcessDoctor.Backend.Linux.Proc;
using ProcessDoctor.Backend.Linux.Proc.Exceptions;

namespace ProcessDoctor.Backend.Linux.Tests.ProcTests;

public sealed class ProcessEntryTests(ProcFileSystemFixture procFileSystem) : IClassFixture<ProcFileSystemFixture>
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
        var process = procFileSystem.CreateProcess(expectedId);
        var sut = ProcessEntry.Create(process.Directory);

        // Assert
        sut.Id
            .Should()
            .Be(expectedId);
    }

    [Theory]
    [InlineData("cmdline")]
    [InlineData(@"C:\NET\ProcessDoctor\ProcessDoctor.exe")]
    public void Should_read_process_command_line_properly(string expectedCommandLine)
    {
        // Arrange
        var process = procFileSystem.CreateProcess(123u);
        using (var writer = process.CommandLineFile.CreateText())
            writer.Write(expectedCommandLine);

        // Act
        var sut = ProcessEntry.Create(process.Directory);

        // Assert
        sut.CommandLine
            .Should()
            .Be(expectedCommandLine);
    }

    [Fact]
    public void Command_line_should_be_null_if_file_is_empty()
    {
        // Arrange
        var process = procFileSystem.CreateProcess(123u);

        // Act
        var sut = ProcessEntry.Create(process.Directory);

        // Assert
        sut.CommandLine
            .Should()
            .BeNull();
    }

    [Fact]
    public void Should_read_process_status_section_properly()
    {
        // Arrange & Act
        var process = procFileSystem.CreateProcess(123u);
        using (var writer = process.StatusFile.CreateText())
            writer.Write(
                """
                    Name: ProcessDoctor
                    ...
                    ...
                """);

        var sut = ProcessEntry.Create(process.Directory);

        // Assert
        sut.Status
            .Should()
            .NotBeNull();
    }
}
