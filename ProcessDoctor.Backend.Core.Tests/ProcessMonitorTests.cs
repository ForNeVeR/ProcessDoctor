using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using ProcessDoctor.Backend.Core.Enums;
using ProcessDoctor.Backend.Core.Interfaces;
using ProcessDoctor.TestFramework.Extensions;
using ProcessDoctor.TestFramework.Logging;
using Xunit.Abstractions;

namespace ProcessDoctor.Backend.Core.Tests;

public sealed class ProcessMonitorTests(ITestOutputHelper output)
{
    [Fact]
    public void Should_take_a_snapshot_at_the_beginning_of_the_observation_of_launched_processes()
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessMonitorTests), output);
        var expectedProcesses = EnumerateFakeProcesses().ToArray();

        var scanner = Substitute.For<IProcessProvider>();
        scanner.ObserveProcesses(Arg.Any<ObservationTarget>())
            .Returns(Observable.Empty<SystemProcess>());

        var snapshot = Substitute.For<IProcessListSnapshot>();
        snapshot.EnumerateProcesses()
            .Returns(expectedProcesses);

        scanner.CreateSnapshot()
            .Returns(snapshot);

        var testObserver = new TestScheduler()
            .CreateObserver<SystemProcess>();

        // Act
        new ProcessMonitor(logger, scanner)
            .LaunchedProcesses
            .Subscribe(testObserver);

        // Arrange
        scanner
            .Received(Quantity.Exactly(number: 1))
            .CreateSnapshot();

        testObserver
            .EnumerateMessages()
            .Should()
            .BeEquivalentTo(expectedProcesses);
    }

    [Fact]
    public void Should_observe_launched_processes()
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessMonitorTests), output);
        var expectedProcesses = EnumerateFakeProcesses().ToArray();

        var scanner = Substitute.For<IProcessProvider>();
        scanner.ObserveProcesses(Arg.Any<ObservationTarget>())
            .Returns(expectedProcesses.ToObservable());

        var snapshot = Substitute.For<IProcessListSnapshot>();
        snapshot.EnumerateProcesses()
            .Returns(Enumerable.Empty<SystemProcess>());

        scanner.CreateSnapshot()
            .Returns(snapshot);

        var testObserver = new TestScheduler()
            .CreateObserver<SystemProcess>();

        // Act
        new ProcessMonitor(logger, scanner)
            .LaunchedProcesses
            .Subscribe(testObserver);

        // Assert
        scanner
            .Received(Quantity.Exactly(number: 1))
            .ObserveProcesses(ObservationTarget.Launched);

        testObserver
            .EnumerateMessages()
            .Should()
            .BeEquivalentTo(expectedProcesses);
    }

    [Fact]
    public void Should_observe_terminated_processes()
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessMonitorTests), output);
        var expectedProcesses = EnumerateFakeProcesses().ToArray();

        var scanner = Substitute.For<IProcessProvider>();
        scanner.ObserveProcesses(Arg.Any<ObservationTarget>())
            .Returns(expectedProcesses.ToObservable());

        var snapshot = Substitute.For<IProcessListSnapshot>();
        snapshot.EnumerateProcesses()
            .Returns(Enumerable.Empty<SystemProcess>());

        scanner.CreateSnapshot()
            .Returns(snapshot);

        var testObserver = new TestScheduler()
            .CreateObserver<SystemProcess>();

        // Act
        new ProcessMonitor(logger, scanner)
            .TerminatedProcesses
            .Subscribe(testObserver);

        // Assert
        scanner
            .Received(Quantity.Exactly(number: 1))
            .ObserveProcesses(ObservationTarget.Terminated);

        testObserver
            .EnumerateMessages()
            .Should()
            .BeEquivalentTo(expectedProcesses);
    }

    private static IEnumerable<FakeProcess> EnumerateFakeProcesses()
    {
        yield return new FakeProcess(
            id: 1,
            parentId: null,
            name: "Process #1",
            commandLine: null,
            executablePath: null);

        yield return new FakeProcess(
            id: 2,
            parentId: null,
            name: "Process #2",
            commandLine: null,
            executablePath: null);
    }
}
