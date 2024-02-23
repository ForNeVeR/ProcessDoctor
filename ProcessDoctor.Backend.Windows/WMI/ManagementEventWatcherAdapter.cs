using System.Management;
using System.Reactive.Linq;
using JetBrains.Diagnostics;
using ProcessDoctor.Backend.Windows.WMI.Interfaces;

namespace ProcessDoctor.Backend.Windows.WMI;

public sealed class ManagementEventWatcherAdapter(ILog logger, WqlEventQuery query) : IManagementEventWatcher
{
    private bool _isDisposed;

    private readonly ManagementEventWatcher _adaptee = new(query);

    /// <inheritdoc />
    public IObservable<EventArrivedEventArgsAdapter> ArrivedEvents
    {
        get
        {
            ThrowIfDisposed();

            return Observable
                .FromEventPattern<EventArrivedEventHandler, EventArrivedEventArgs>(
                    eventHandler => _adaptee.EventArrived += eventHandler,
                    eventHandler => _adaptee.EventArrived -= eventHandler)
                .Select(arrivedEvent => new EventArrivedEventArgsAdapter(arrivedEvent.EventArgs));
        }
    }

    /// <inheritdoc />
    public void Start()
    {
        ThrowIfDisposed();

        _adaptee.Start();

        logger.Info("WMI event watcher has been started");
    }

    /// <inheritdoc />
    public void Stop()
    {
        ThrowIfDisposed();

        _adaptee.Stop();

        logger.Info("WMI event watcher has been stopped");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_isDisposed)
            return;

        Stop();

        _adaptee.Dispose();

        _isDisposed = true;

        logger.Info("WMI event watcher has been disposed");
    }

    private void ThrowIfDisposed()
    {
        if (!_isDisposed)
            return;

        throw new ObjectDisposedException(
            nameof(ManagementEventWatcherAdapter));
    }
}
