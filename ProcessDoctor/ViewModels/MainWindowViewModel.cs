using System;
using System.Reactive.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
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
                    BuildNameColumn(),
                    childSelector: p => p.Children
                ),
                new TextColumn<ProcessViewModel, uint>("PID", x => x.Id),
                new TextColumn<ProcessViewModel, string>("Command Line", x => x.CommandLine),
            },
        };
    }

    private static TemplateColumn<ProcessViewModel> BuildNameColumn()
    {
        var cellTemplate = new FuncDataTemplate<ProcessViewModel?>((viewModel, _) =>
            BuildNameControl(viewModel));

        var options = new TemplateColumnOptions<ProcessViewModel>
        {
            CompareAscending = (p1, p2) => string.Compare(p1?.Name, p2?.Name, StringComparison.Ordinal),
            CompareDescending = (p1, p2) => string.Compare(p2?.Name, p1?.Name, StringComparison.Ordinal)
        };

        return new TemplateColumn<ProcessViewModel>(
            header: "Name",
            cellTemplate: cellTemplate,
            options: options);

        static Grid BuildNameControl(ProcessViewModel? viewModel)
        {
            var grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                ColumnDefinitions =
                [
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(5.0, GridUnitType.Pixel),
                    new ColumnDefinition(GridLength.Star)
                ]
            };

            if (viewModel is null)
            {
                return grid;
            }

            var controls = new Control[]
            {
                BuildImageControl(viewModel),
                new TextBlock
                {
                    Text = viewModel.Name,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    [Grid.ColumnProperty] = 2
                }
            };

            grid.Children.AddRange(controls);

            return grid;
        }

        static Image BuildImageControl(ProcessViewModel? viewModel)
        {
            var image = new Image
            {
                Width = 16.0,
                Height = 16.0,
                Stretch = Stretch.Fill,
                [Grid.ColumnProperty] = 0
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
