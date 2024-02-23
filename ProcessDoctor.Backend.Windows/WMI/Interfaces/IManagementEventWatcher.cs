namespace ProcessDoctor.Backend.Windows.WMI.Interfaces;

public interface IManagementEventWatcher : IDisposable
{
    IObservable<EventArrivedEventArgsAdapter> ArrivedEvents { get; }

    void Start();

    void Stop();
}
