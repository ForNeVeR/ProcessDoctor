using System.Reactive.Linq;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using PInvoke;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Core.Enums;
using ProcessDoctor.Backend.Core.Interfaces;
using ProcessDoctor.Backend.Windows.WMI.Interfaces;

namespace ProcessDoctor.Backend.Windows;

public sealed class ProcessProvider(Lifetime lifetime, ILog logger, IManagementEventWatcherFactory watcherFactory) : IProcessProvider
{
    /// <inheritdoc />
    public IObservable<SystemProcess> ObserveProcesses(ObservationTarget targetState)
    {
        var watcher = watcherFactory.Create(targetState);
        var lifetimeScope = lifetime.CreateNested();

        lifetimeScope
            .Lifetime
            .AddDispose(watcher);

        watcher.Start();

        return watcher
            .ArrivedEvents
            .Select(arrivedEvent => WindowsProcess.Create(arrivedEvent.TargetInstance))
            .Finally(lifetimeScope.Terminate)
            .Catch((Exception exception) =>
            {
                logger.Error(exception, "An error occurred while processing an event received from WMI");

                return ObserveProcesses(targetState);
            });
    }

    /// <inheritdoc />
    public IProcessListSnapshot CreateSnapshot()
    {
        var nativeHandle = Kernel32.CreateToolhelp32Snapshot(
            Kernel32.CreateToolhelp32SnapshotFlags.TH32CS_SNAPPROCESS,
            th32ProcessID: 0);

        return new ProcessListSnapshot(Log.GetLog<ProcessListSnapshot>(), nativeHandle);
    }
}
