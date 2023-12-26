using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Windows;

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
                new TextColumn<ProcessViewModel, string>("Name", x => x.Name),
                new TextColumn<ProcessViewModel, string>("Command Line", x => x.CommandLine),
            },
        };
    }
}
