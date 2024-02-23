using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Core.Interfaces;

namespace ProcessDoctor.Tests;

internal sealed class ObservableCollectionProcessMonitor : IProcessMonitor, IDisposable
{
    private readonly ObservableCollection<SystemProcess> _processes;

    private readonly Subject<SystemProcess> _terminatedProcesses = new();

    private readonly Subject<SystemProcess> _launchedProcesses = new();

    internal ObservableCollectionProcessMonitor(ObservableCollection<SystemProcess> processes)
    {
        _processes = processes;

        _processes.CollectionChanged += OnCollectionChanged;
    }

    /// <inheritdoc />
    public IObservable<SystemProcess> TerminatedProcesses
        => _terminatedProcesses.AsObservable();

    /// <inheritdoc />
    public IObservable<SystemProcess> LaunchedProcesses
        => _launchedProcesses.AsObservable();

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        switch (eventArgs.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var launchedProcess = eventArgs
                    .NewItems?
                    .OfType<SystemProcess>()
                    .FirstOrDefault()
                        ?? throw new InvalidOperationException();

                _launchedProcesses.OnNext(launchedProcess);
                return;

            case NotifyCollectionChangedAction.Remove:
                var terminatedProcess = eventArgs
                    .OldItems?
                    .OfType<SystemProcess>()
                    .FirstOrDefault()
                        ?? throw new InvalidOperationException();

                _terminatedProcesses.OnNext(terminatedProcess);
                return;

            default:
                throw new NotSupportedException();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _processes.CollectionChanged -= OnCollectionChanged;
        _terminatedProcesses.Dispose();
        _launchedProcesses.Dispose();
    }
}
