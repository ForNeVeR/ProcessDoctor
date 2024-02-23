using System.Management;

namespace ProcessDoctor.Backend.Windows.WMI;

public sealed class EventArrivedEventArgsAdapter(EventArrivedEventArgs adaptee) : EventArgs
{
    public ManagementBaseObject TargetInstance { get; }
        = (ManagementBaseObject)adaptee.NewEvent[nameof(TargetInstance)];
}
