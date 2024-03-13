using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProcessDoctor.Backend.Core;
using SkiaSharp;

namespace ProcessDoctor.ViewModels;

public record ProcessViewModel(
    uint Id,
    string Name,
    string? CommandLine,
    Task<SKBitmap?> Image,
    ObservableCollection<ProcessViewModel> Children)
{
    public static ProcessViewModel Of(SystemProcess model) => new(
        model.Id,
        model.Name,
        model.CommandLine,
        model.ExtractIconAsync(),
        []);
}
