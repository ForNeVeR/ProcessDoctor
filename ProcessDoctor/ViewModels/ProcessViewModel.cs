using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ProcessDoctor.Backend.Core;
using ReactiveUI;
using SkiaSharp;

namespace ProcessDoctor.ViewModels;

public record ProcessViewModel(
    uint Id,
    string Name,
    string? CommandLine,
    IObservable<SKBitmap?> Image,
    ObservableCollection<ProcessViewModel> Children)
{
    public static ProcessViewModel Of(SystemProcess model) => new(
        model.Id,
        model.Name,
        model.CommandLine,
        Observable.FromAsync(() => Task.Run(model.ExtractIcon)),
        []);
}
