using System.Reactive.Linq;
using JetBrains.Lifetimes;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Core.Enums;
using ProcessDoctor.Backend.Windows.WMI;
using ProcessDoctor.Backend.Windows.WMI.Interfaces;
using ProcessDoctor.TestFramework;
using ProcessDoctor.TestFramework.Logging;
using Xunit.Abstractions;

namespace ProcessDoctor.Backend.Windows.Tests;

public sealed class ProcessProviderTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData(ObservationTarget.Launched)]
    [InlineData(ObservationTarget.Terminated)]
    public void Should_restart_observing_if_error_occurrs(ObservationTarget observationTarget)
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessProviderTests), output);
        var watcher = Substitute.For<IManagementEventWatcher>();

        watcher
            .ArrivedEvents
            .Returns(
                new OnErrorObservable<EventArrivedEventArgsAdapter>(new FakeException()),
                Observable.Never<EventArrivedEventArgsAdapter>());

        var watcherFactory = Substitute.For<IManagementEventWatcherFactory>();

        watcherFactory
            .Create(Arg.Any<ObservationTarget>())
            .Returns(watcher);

        var testObserver = new TestScheduler()
            .CreateObserver<SystemProcess>();

        // Act
        new ProcessProvider(Lifetime.Eternal, logger, watcherFactory)
            .ObserveProcesses(observationTarget)
            .Subscribe(testObserver);

        // Assert
        watcherFactory
            .Received(Quantity.Exactly(number: 2))
            .Create(Arg.Any<ObservationTarget>());
    }

    [Theory]
    [InlineData(ObservationTarget.Launched)]
    [InlineData(ObservationTarget.Terminated)]
    public void Should_dispose_old_watcher_if_error_occurrs(ObservationTarget observationTarget)
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessProviderTests), output);
        var watcher = Substitute.For<IManagementEventWatcher>();

        watcher
            .ArrivedEvents
            .Returns(
                new OnErrorObservable<EventArrivedEventArgsAdapter>(new FakeException()),
                Observable.Never<EventArrivedEventArgsAdapter>());

        var watcherFactory = Substitute.For<IManagementEventWatcherFactory>();

        watcherFactory
            .Create(Arg.Any<ObservationTarget>())
            .Returns(watcher);

        var testObserver = new TestScheduler()
            .CreateObserver<SystemProcess>();

        // Act
        new ProcessProvider(Lifetime.Eternal, logger, watcherFactory)
            .ObserveProcesses(observationTarget)
            .Subscribe(testObserver);

        // Assert
        watcher
            .Received(Quantity.Exactly(number: 1))
            .Dispose();
    }

    [Theory]
    [InlineData(ObservationTarget.Launched)]
    [InlineData(ObservationTarget.Terminated)]
    public void Should_dispose_old_watcher_if_subscription_is_disposed(ObservationTarget observationTarget)
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessProviderTests), output);
        var watcher = Substitute.For<IManagementEventWatcher>();

        watcher
            .ArrivedEvents
            .Returns(Observable.Never<EventArrivedEventArgsAdapter>());

        var watcherFactory = Substitute.For<IManagementEventWatcherFactory>();

        watcherFactory
            .Create(Arg.Any<ObservationTarget>())
            .Returns(watcher);

        var testObserver = new TestScheduler()
            .CreateObserver<SystemProcess>();

        var subscription = new ProcessProvider(Lifetime.Eternal, logger, watcherFactory)
            .ObserveProcesses(observationTarget)
            .Subscribe(testObserver);

        // Act
        subscription.Dispose();

        // Assert
        watcher
            .Received(Quantity.Exactly(number: 1))
            .Dispose();
    }

    [Theory]
    [InlineData(ObservationTarget.Launched)]
    [InlineData(ObservationTarget.Terminated)]
    public void Should_dispose_old_watcher_if_lifetime_is_terminated(ObservationTarget observationTarget)
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessProviderTests), output);
        var watcher = Substitute.For<IManagementEventWatcher>();

        watcher
            .ArrivedEvents
            .Returns(Observable.Never<EventArrivedEventArgsAdapter>());

        var watcherFactory = Substitute.For<IManagementEventWatcherFactory>();

        watcherFactory
            .Create(Arg.Any<ObservationTarget>())
            .Returns(watcher);

        var lifetimeScope = new LifetimeDefinition();

        var testObserver = new TestScheduler()
            .CreateObserver<SystemProcess>();

        new ProcessProvider(lifetimeScope.Lifetime, logger, watcherFactory)
            .ObserveProcesses(observationTarget)
            .Subscribe(testObserver);

        // Act
        lifetimeScope.Terminate();

        // Assert
        watcher
            .Received(Quantity.Exactly(number: 1))
            .Dispose();
    }
}
