using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
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
        new WmiProcessMonitor(Lifetime.Eternal, Log.GetLog<WmiProcessMonitor>()));

    public HierarchicalTreeDataGridSource<ProcessViewModel> ItemSource { get; }

    public MainWindowViewModel(Lifetime lifetime, IProcessMonitor processManager)
    {
        var processes = ObserveProcesses(lifetime, processManager.Processes);
        ItemSource = new(processes)
        {
            Columns =
            {
                BuildImageColumn(),
                new HierarchicalExpanderColumn<ProcessViewModel>(
                    new TextColumn<ProcessViewModel, uint>("Id", x => x.Id),
                    childSelector: _ => [] // no children for now
                ),
                new TextColumn<ProcessViewModel, string>("Name", x => x.Name),
                new TextColumn<ProcessViewModel, string>("Command Line", x => x.CommandLine),
            }
        };

        return;

        TemplateColumn<ProcessViewModel> BuildImageColumn()
        {
            var template = new FuncDataTemplate<ProcessViewModel?>(
                (viewModel, _) => new Image
                {
                    Source = viewModel?.Image,
                    Stretch = Stretch.Fill
                });

            return new TemplateColumn<ProcessViewModel>(string.Empty, template);
        }
    }

    private static ReadOnlyObservableCollection<ProcessViewModel> ObserveProcesses(
        Lifetime lifetime,
        ObservableCollection<ProcessModel> models)
    {
        lifetime.AddDispose(
            models.ToObservableChangeSet()
                .Select(ProcessViewModel.Of)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var targetCollection)
                .Subscribe());
        return targetCollection;
    }
}
