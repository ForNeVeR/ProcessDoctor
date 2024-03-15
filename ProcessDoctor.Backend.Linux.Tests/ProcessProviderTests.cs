using System.IO.Abstractions;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Core.Enums;
using ProcessDoctor.Backend.Linux.Proc.Interfaces;
using ProcessDoctor.Backend.Linux.Tests.Fixtures;
using ProcessDoctor.TestFramework;
using ProcessDoctor.TestFramework.Extensions;
using ProcessDoctor.TestFramework.Logging;
using Xunit.Abstractions;

namespace ProcessDoctor.Backend.Linux.Tests;

public sealed class ProcessProviderTests(ProcFolderFixture procFolderFixture, ITestOutputHelper output) : IClassFixture<ProcFolderFixture>
{
    [Theory]
    [InlineData(ObservationTarget.Launched)]
    [InlineData(ObservationTarget.Terminated)]
    public void Should_restart_observing_if_error_occurrs(ObservationTarget observationTarget)
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessProviderTests), output);
        var procFolderEntry = Substitute.For<IProcFolderEntry>();

        procFolderEntry
            .EnumerateProcessDirectories()
            .Returns(
                _ => Enumerable.Empty<IDirectoryInfo>(),
                _ => throw new FakeException(),
                _ => Enumerable.Empty<IDirectoryInfo>());

        var testObserver = new TestScheduler()
            .CreateObserver<SystemProcess>();

        var sut = new ProcessProvider(logger, procFolderEntry);

        // Act
        sut.ObserveProcesses(observationTarget)
            .Subscribe(testObserver)
            .Dispose();

        // Assert
        /*
         * This is an indirect assertion.
         * First, a snapshot is taken (this is the first call).
         * Then an error occurs (this is the second call).
         * Next, the observing is restarted and the new snapshot is taken (this is the third call).
         * Finally, the processes in the loop are retrieved once and subscription is disposed (this is the fourth call).
         *
         * This test can be improved in the future.
         */
        procFolderEntry
            .Received(Quantity.Exactly(number: 4))
            .EnumerateProcessDirectories();
    }

    [Fact]
    public void Should_observe_launched_processes_properly()
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessProviderTests), output);
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
            .Returns(
                Enumerable.Empty<IDirectoryInfo>(),
                processDirectories);

        var testObserver = new TestScheduler()
            .CreateObserver<SystemProcess>();

        var sut = new ProcessProvider(logger, procFolderEntry);

        // Act
        using var _ = sut.ObserveProcesses(ObservationTarget.Launched)
            .Subscribe(testObserver);

        // Assert
        testObserver
            .EnumerateMessages()
            .Select(process => process.Id)
            .Should()
            .ContainInOrder(expectedProcesses);
    }

    [Fact]
    public void Should_observe_terminated_processes_properly()
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessProviderTests), output);
        var procFolderEntry = Substitute.For<IProcFolderEntry>();

        var expectedProcesses = new[]
        {
            567u,
            879u
        };

        var processDirectories = new[]
        {
            123u
        }
        .Concat(expectedProcesses)
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
            .Returns(
                processDirectories,
                processDirectories.ExceptBy(
                    expectedProcesses,
                    processDirectory => uint.Parse(processDirectory.Name)));

        var testObserver = new TestScheduler()
            .CreateObserver<SystemProcess>();

        var sut = new ProcessProvider(logger, procFolderEntry);

        // Act
        using var _ = sut.ObserveProcesses(ObservationTarget.Terminated)
            .Subscribe(testObserver);

        // Assert
        testObserver
            .EnumerateMessages()
            .Select(process => process.Id)
            .Should()
            .ContainInOrder(expectedProcesses);
    }

    [Fact]
    public void Should_create_process_snapshot_list_properly()
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessListSnapshotTests), output);
        var procFolderEntry = Substitute.For<IProcFolderEntry>();
        var sut = new ProcessProvider(logger, procFolderEntry);

        // Act
        var snapshot = sut.CreateSnapshot();

        // Assert
        snapshot
            .Should()
            .NotBeNull()
            .And
            .BeOfType<ProcessListSnapshot>();
    }
}
