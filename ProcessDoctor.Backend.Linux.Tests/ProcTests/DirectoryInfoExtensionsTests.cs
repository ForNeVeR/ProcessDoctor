using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using ProcessDoctor.Backend.Linux.Proc;
using ProcessDoctor.Backend.Linux.Proc.Extensions;

namespace ProcessDoctor.Backend.Linux.Tests.ProcTests;

public sealed class DirectoryInfoExtensionsTests
{
    [Theory]
    [InlineData("124152")]
    [InlineData("245")]
    [InlineData("245612")]
    [InlineData("67")]
    public void Should_return_true_if_directory_is_process(string expectedProcessId)
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var path = fileSystem.Path.Combine(ProcPaths.Path, expectedProcessId);
        var sut = fileSystem.DirectoryInfo.New(path);

        // Act & Assert
        sut.IsProcess()
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("-523")]
    [InlineData("status")]
    [InlineData("cmdline")]
    [InlineData("123exe")]
    public void Should_return_false_if_directory_is_not_process(string expectedProcessId)
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var path = fileSystem.Path.Combine(ProcPaths.Path, expectedProcessId);
        var sut = fileSystem.DirectoryInfo.New(path);

        // Act & Assert
        sut.IsProcess()
            .Should()
            .BeFalse();
    }
}
