using ProcessDoctor.Backend.Core;

namespace ProcessDoctor.ViewModels;

public record ProcessViewModel(
    uint Id,
    string Name,
    string CommandLine)
{
    public static ProcessViewModel Of(ProcessModel model) => new(model.Id, model.Name, model.CommandLine);
}
