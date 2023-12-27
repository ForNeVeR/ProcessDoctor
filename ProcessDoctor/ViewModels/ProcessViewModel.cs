using System.Collections.ObjectModel;
using ProcessDoctor.Backend.Core;

namespace ProcessDoctor.ViewModels;

public record ProcessViewModel(
    uint Id,
    string Name,
    string CommandLine,
    ObservableCollection<ProcessViewModel> Children)
{
    public static ProcessViewModel Of(ProcessModel model) => new(
        model.Id,
        model.Name,
        model.CommandLine,
        []);
}
