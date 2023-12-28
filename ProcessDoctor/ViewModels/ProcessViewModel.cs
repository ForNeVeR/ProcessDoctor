using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Imaging.Extensions;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace ProcessDoctor.ViewModels;

public record ProcessViewModel(
    uint Id,
    string Name,
    string CommandLine,
    Task<Bitmap?> Image,
    ObservableCollection<ProcessViewModel> Children)
{
    public static ProcessViewModel Of(ProcessModel model) => new(
        model.Id,
        model.Name,
        model.CommandLine,
        model.ExtractAssociatedBitmapAsync(),
        []);
}
