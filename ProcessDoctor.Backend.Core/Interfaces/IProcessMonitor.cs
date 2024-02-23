namespace ProcessDoctor.Backend.Core.Interfaces;

public interface IProcessMonitor
{
    IObservable<SystemProcess> LaunchedProcesses { get; }

    IObservable<SystemProcess> TerminatedProcesses { get; }
}
