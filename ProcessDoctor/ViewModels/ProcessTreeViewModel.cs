using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Core.Interfaces;
using ReactiveUI;

namespace ProcessDoctor.ViewModels;

// TODO[#5]: optimize all IndexOf/RemoveAt/linear Remove calls in this class, perhaps by removing them
public class ProcessTreeViewModel
{
    private readonly ILog _logger;

    private readonly ObservableCollection<ProcessViewModel> _viewModels = [];
    public ReadOnlyObservableCollection<ProcessViewModel> Processes { get; }

    // TODO[#14]: Deal with temporary duplicates having same id
    private readonly Dictionary<uint, ProcessViewModel> _allProcesses = new();
    // parent id => child list
    private readonly Dictionary<uint, List<ProcessViewModel>> _orphanedModels = new();

    public ProcessTreeViewModel(
        ILog logger,
        Lifetime lifetime,
        IProcessMonitor processMonitor)
    {
        _logger = logger;

        Processes = new ReadOnlyObservableCollection<ProcessViewModel>(_viewModels);

        lifetime.AddDispose(
            processMonitor
                .TerminatedProcesses
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(process =>
                    _logger.Catch(() => OnProcessTerminated(process))));

        lifetime.AddDispose(
            processMonitor
                .LaunchedProcesses
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(process =>
                    _logger.Catch(() => OnProcessLaunched(process))));
    }

    private void OnProcessLaunched(SystemProcess process)
    {
        var children = (IList<ProcessViewModel>?)_orphanedModels.GetValueOrDefault(process.Id)
            ?? Array.Empty<ProcessViewModel>();

        foreach (var child in children)
        {
            // TODO[#5]: Optimize
            _viewModels.RemoveAt(_viewModels.IndexOf(child));
        }

        _orphanedModels.Remove(process.Id);
        var viewModel = ProcessViewModel.Of(process);
        viewModel.Children.AddRange(children);
        _allProcesses.Add(process.Id, viewModel);

        if (process.ParentId is null)
        {
            _viewModels.Add(viewModel);
            return;
        }

        var parentId = process.ParentId.Value;
        var parent = _allProcesses.GetValueOrDefault(parentId);
        if (parent is null)
        {
            var list = _orphanedModels.GetValueOrDefault(parentId);
            if (list is null)
            {
                list = [];
                _orphanedModels.Add(parentId, list);
            }

            list.Add(viewModel);
            _viewModels.Add(viewModel);
        }
        else
        {
            parent.Children.Add(viewModel);
        }
    }

    private void OnProcessTerminated(SystemProcess process)
    {
        var viewModel = _allProcesses[process.Id];

        // 1. Remove from the process collection.
        _allProcesses.Remove(process.Id);

        // 2. Process' children are now orphans.
        if (viewModel.Children.Count > 0)
        {
            if (_orphanedModels.Remove(process.Id))
                _logger.Error($"Orphaned list not empty for process {process.Id}.");

            List<ProcessViewModel> orphanedList = [];
            _orphanedModels.Add(process.Id, orphanedList);

            orphanedList.AddRange(viewModel.Children);
            _viewModels.AddRange(viewModel.Children);
        }

        // 3. Remove from the orphan list, if there was an orphan list containing the current process.
        if (process.ParentId is { } parentId)
        {
            var parent = _allProcesses.GetValueOrDefault(parentId);
            if (parent is not null)
            {
                // Process has a real parent: remove from the parent's children.
                parent.Children.Remove(viewModel);

                // As process cannot be an orphan or be in the root collection, terminate here.
                return;
            }

            // Process has no real parent: it has to be an orphan.
            var orphanList = _orphanedModels[parentId];
            orphanList.Remove(viewModel);
            if (orphanList.Count == 0)
            {
                _orphanedModels.Remove(parentId);
            }
        }

        // The process is either an orphan or a root process with parentId = null, so remove it from _viewModels:
        _viewModels.Remove(viewModel);
    }
}
