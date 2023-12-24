using System.Collections.ObjectModel;

namespace ProcessDoctor.Backend.Core;

public interface IProcessMonitor
{
    ObservableCollection<ProcessModel> Processes { get; }
}
