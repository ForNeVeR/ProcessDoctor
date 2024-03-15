using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProcessDoctor.Backend.Linux.Proc.Interfaces;
using ProcessDoctor.Backend.Linux.Tests.Fixtures;
using ProcessDoctor.TestFramework;
using ProcessDoctor.TestFramework.Logging;
using Xunit.Abstractions;

namespace ProcessDoctor.Backend.Linux.Tests;

public sealed class ProcessListSnapshotTests(ProcFolderFixture procFolderFixture, ITestOutputHelper output) : IClassFixture<ProcFolderFixture>
{
    [Fact]
    public void Should_returns_empty_snapshot_if_any_error_occured()
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessListSnapshotTests), output);
        var procFolderEntry = Substitute.For<IProcFolderEntry>();

        procFolderEntry
            .EnumerateProcessDirectories()
            .Throws<FakeException>();

        var sut = new ProcessListSnapshot(logger, procFolderEntry);

        // Act & Assert
        sut.EnumerateProcesses()
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Should_returns_snapshot_properly()
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessListSnapshotTests), output);
        var procFolderEntry = Substitute.For<IProcFolderEntry>();

        var expectedProcesses = new[]
        {
            123u,
            567u,
            879u
        };

        var processDirectories = expectedProcesses
            .Select(id =>
            {
                var processFixture = procFolderFixture.CreateProcess(id);
                using (var writer = processFixture.StatusFile.CreateText())
                    writer.Write(
                        $"""
                            Name: ProcessDoctor
                            ...
                            State: R
                            ...
                            ...
                            Pid: {id}
                            PPid: 1
                        """);

                return processFixture.Directory;
            })
            .ToArray();

        procFolderEntry
            .EnumerateProcessDirectories()
            .Returns(processDirectories);

        var sut = new ProcessListSnapshot(logger, procFolderEntry);

        // Act & Assert
        sut.EnumerateProcesses()
            .Select(process => process.Id)
            .Should()
            .ContainInOrder(expectedProcesses);
    }
}
