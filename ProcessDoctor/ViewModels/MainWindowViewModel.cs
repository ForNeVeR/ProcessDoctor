using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

namespace ProcessDoctor.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ObservableCollection<ProcessViewModel> _processes = [];
    public HierarchicalTreeDataGridSource<ProcessViewModel> ItemSource { get; }

    public MainWindowViewModel()
    {
        _processes.Add(new ProcessViewModel
        {
            Id = 1,
            Name = "Process 1"
        });
        _processes.Add(new ProcessViewModel
        {
            Id = 2,
            Name = "Process 2"
        });

        ItemSource = new(_processes)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<ProcessViewModel>(
                    new TextColumn<ProcessViewModel, uint>("Id", x => x.Id),
                    childSelector: _ => [] // no children for now
                ),
                new TextColumn<ProcessViewModel, string>("Name", x => x.Name),
            },
        };
    }
}
