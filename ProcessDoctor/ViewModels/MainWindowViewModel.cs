using System.Reactive.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Windows;
using ReactiveUI;
using Image = Avalonia.Controls.Image;

namespace ProcessDoctor.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public static MainWindowViewModel DesignInstance { get; } = new(
        Lifetime.Eternal,
        Log.GetLog<MainWindowViewModel>(),
        new WmiProcessMonitor(Lifetime.Eternal, Log.GetLog<WmiProcessMonitor>()));

    public HierarchicalTreeDataGridSource<ProcessViewModel> ItemSource { get; }

    public MainWindowViewModel(Lifetime lifetime, ILog logger, IProcessMonitor processManager)
    {
        var processTree = new ProcessTreeViewModel(logger, lifetime, processManager.Processes);
        ItemSource = new(processTree.Processes)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<ProcessViewModel>(
                    new TextColumn<ProcessViewModel, uint>("Id", x => x.Id),
                    childSelector: p => p.Children
                ),
                BuildImageColumn(),
                new TextColumn<ProcessViewModel, string>("Name", x => x.Name),
                new TextColumn<ProcessViewModel, string>("Command Line", x => x.CommandLine),
            },
        };
    }

    private static TemplateColumn<ProcessViewModel> BuildImageColumn()
    {
        return new TemplateColumn<ProcessViewModel>(
            header: string.Empty,
            cellTemplate: new FuncDataTemplate<ProcessViewModel?>((viewModel, _) =>
                BuildImageControl(viewModel)));

        static Image BuildImageControl(ProcessViewModel? viewModel)
        {
            var image = new Image
            {
                Stretch = Stretch.Fill
            };

            if (viewModel is null)
            {
                return image;
            }

            image.Bind(
                Image.SourceProperty,
                viewModel.Image.ToObservable(RxApp.TaskpoolScheduler));

            return image;
        }
    }
}
