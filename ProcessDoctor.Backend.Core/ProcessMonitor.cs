using System.Reactive.Linq;
using JetBrains.Diagnostics;
using ProcessDoctor.Backend.Core.Enums;
using ProcessDoctor.Backend.Core.Interfaces;

namespace ProcessDoctor.Backend.Core;

public sealed class ProcessMonitor(ILog logger, IProcessProvider provider)
{
    public IObservable<SystemProcess> LaunchedProcesses
    {
        get
        {
            using var snapshot = provider.CreateSnapshot();

            return provider
                .ObserveProcesses(ObservationTarget.Launched)
                .Do(process => logger.Info("Process (PID: {0}) has been launched", process.Id))
                .StartWith(snapshot.EnumerateProcesses());
        }
    }

    public IObservable<SystemProcess> TerminatedProcesses
        => provider
            .ObserveProcesses(ObservationTarget.Terminated)
            .Do(process => logger.Info("Process (PID: {0}) has been terminated", process.Id));
}
