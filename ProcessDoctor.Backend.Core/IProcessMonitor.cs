using System.Collections.ObjectModel;

namespace ProcessDoctor.Backend.Core;

public interface IProcessMonitor
{
    ObservableCollection<SystemProcess> Processes { get; }
}
