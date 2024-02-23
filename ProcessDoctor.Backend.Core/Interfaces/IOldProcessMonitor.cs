using System.Collections.ObjectModel;

namespace ProcessDoctor.Backend.Core.Interfaces;

public interface IOldProcessMonitor
{
    ObservableCollection<SystemProcess> Processes { get; }
}
