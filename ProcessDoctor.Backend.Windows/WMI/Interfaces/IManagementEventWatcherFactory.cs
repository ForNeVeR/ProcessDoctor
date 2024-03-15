using ProcessDoctor.Backend.Core.Enums;

namespace ProcessDoctor.Backend.Windows.WMI.Interfaces;

public interface IManagementEventWatcherFactory
{
    IManagementEventWatcher Create(ObservationTarget observationTarget);
}
