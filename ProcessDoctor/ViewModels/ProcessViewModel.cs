using System.Collections.Immutable;
using ProcessDoctor.Backend.Core;

namespace ProcessDoctor.ViewModels;

public record ProcessViewModel(
    uint Id,
    string Name,
    string CommandLine,
    ImmutableArray<ProcessViewModel> Children)
{
    public static ProcessViewModel Of(ProcessModel model) => new(
        model.Id,
        model.Name,
        model.CommandLine,
        ImmutableArray.Create<ProcessViewModel>());
}
