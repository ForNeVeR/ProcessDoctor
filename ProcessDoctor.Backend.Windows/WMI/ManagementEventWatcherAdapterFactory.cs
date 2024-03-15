using JetBrains.Diagnostics;
using ProcessDoctor.Backend.Core.Enums;
using ProcessDoctor.Backend.Windows.WMI.Extensions;
using ProcessDoctor.Backend.Windows.WMI.Interfaces;

namespace ProcessDoctor.Backend.Windows.WMI;

public sealed class ManagementEventWatcherAdapterFactory : IManagementEventWatcherFactory
{
    /// <inheritdoc />
    public IManagementEventWatcher Create(ObservationTarget observationTarget)
        => new ManagementEventWatcherAdapter(
            Log.GetLog<ManagementEventWatcherAdapter>(),
            observationTarget.ToWqlQuery());
}
