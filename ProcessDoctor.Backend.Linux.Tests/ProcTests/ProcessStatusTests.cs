using FluentAssertions;
using ProcessDoctor.Backend.Linux.Proc.Exceptions;
using ProcessDoctor.Backend.Linux.Proc.StatusFile;
using ProcessDoctor.Backend.Linux.Proc.StatusFile.Enums;
using ProcessDoctor.Backend.Linux.Tests.Fixtures;

namespace ProcessDoctor.Backend.Linux.Tests.ProcTests;

public sealed class ProcessStatusTests(ProcFolderFixture procFolderFixture) : IClassFixture<ProcFolderFixture>
{
    [Theory]
    [InlineData("stat")]
    [InlineData("123")]
    [InlineData("cmdline")]
    [InlineData("exe")]
    public void Should_throw_exception_if_status_file_name_is_invalid(string statusFileName)
    {
        // Arrange
        var statusFile = procFolderFixture
            .FileSystem
            .FileInfo
            .New(statusFileName);

        // Act & Assert
        this.Invoking(_ => ProcessStatus.Create(statusFile))
            .Should()
            .Throw<InvalidStatusFileException>();
    }

    [Theory]
    [InlineData("ProcessDoctor")]
    [InlineData("Rider")]
    public void Should_read_name_properly(string expectedName)
    {
        // Arrange
        var process = procFolderFixture.CreateProcess(123u);
        using (var writer = process.StatusFile.CreateText())
            writer.Write(
                $"""
                    Name: {expectedName}
                    ...
                    ...
                """);

        var sut = ProcessStatus.Create(process.StatusFile);

        // Act & Assert
        sut.Name
            .Should()
            .Be(expectedName);
    }

    [Fact]
    public void Should_throw_exception_if_name_is_invalid()
    {
        // Arrange
        var process = procFolderFixture.CreateProcess(123u);
        using (var writer = process.StatusFile.CreateText())
            writer.Write(
                """
                     Name:
                     ...
                     ...
                 """);

        var sut = ProcessStatus.Create(process.StatusFile);

        // Act & Assert
        sut.Invoking(status => status.Name)
            .Should()
            .Throw<InvalidStatusFilePropertyException>();
    }

    [Theory]
    [InlineData(123u)]
    [InlineData(1234u)]
    public void Should_read_parent_id_properly(uint expectedParentId)
    {
        // Arrange
        var process = procFolderFixture.CreateProcess(123u);
        using (var writer = process.StatusFile.CreateText())
            writer.Write(
                $"""
                     Name: ProcessDoctor
                     ...
                     ...
                     ...
                     ...
                     ...
                     PPid: {expectedParentId}
                 """);

        var sut = ProcessStatus.Create(process.StatusFile);

        // Act & Assert
        sut.ParentId
            .Should()
            .Be(expectedParentId);
    }

    [Fact]
    public void Parent_id_should_be_null_if_value_was_zero()
    {
        // Arrange
        var process = procFolderFixture.CreateProcess(123u);
        using (var writer = process.StatusFile.CreateText())
            writer.Write(
                """
                     Name: ProcessDoctor
                     ...
                     ...
                     ...
                     ...
                     ...
                     PPid: 0
                 """);

        var sut = ProcessStatus.Create(process.StatusFile);

        // Act & Assert
        sut.ParentId
            .Should()
            .BeNull();
    }

    [Theory]
    [InlineData("-123")]
    [InlineData("exe")]
    [InlineData("")]
    public void Should_throw_exception_if_parent_id_is_invalid(string expectedParentId)
    {
        // Arrange
        var process = procFolderFixture.CreateProcess(123u);
        using (var writer = process.StatusFile.CreateText())
            writer.Write(
                $"""
                     Name: ProcessDoctor
                     ...
                     ...
                     ...
                     ...
                     ...
                     PPid: {expectedParentId}
                 """);

        var sut = ProcessStatus.Create(process.StatusFile);

        // Act & Assert
        sut.Invoking(status => status.ParentId)
            .Should()
            .Throw<InvalidStatusFilePropertyException>();
    }

    [Theory]
    [InlineData("R (running)", ProcessState.Running)]
    [InlineData("S (sleeping)", ProcessState.Sleeping)]
    [InlineData("D", ProcessState.UninterruptibleWait)]
    [InlineData("Z", ProcessState.Zombie)]
    [InlineData("T", ProcessState.TracedOrStopped)]
    public void Should_read_state_properly(string rawState, ProcessState expectedState)
    {
        // Arrange
        var process = procFolderFixture.CreateProcess(123u);
        using (var writer = process.StatusFile.CreateText())
            writer.Write(
                $"""
                    Name: ProcessDoctor
                    ...
                    State: {rawState}
                """);

        var sut = ProcessStatus.Create(process.StatusFile);

        // Act & Assert
        sut.State
            .Should()
            .Be(expectedState);
    }

    [Fact]
    public void Should_throw_exception_if_state_is_invalid()
    {
        // Arrange
        var process = procFolderFixture.CreateProcess(123u);
        using (var writer = process.StatusFile.CreateText())
            writer.Write(
                """
                     Name: ProcessDoctor
                     ...
                     State:
                 """);

        var sut = ProcessStatus.Create(process.StatusFile);

        // Act & Assert
        sut.Invoking(status => status.State)
            .Should()
            .Throw<InvalidStatusFilePropertyException>();
    }
}
