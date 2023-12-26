using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Concurrency;
using DynamicData;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;
using ReactiveUI;

namespace ProcessDoctor.ViewModels;

public class ProcessTreeViewModel
{
    private readonly ILog _logger;

    private readonly ObservableCollection<ProcessViewModel> _viewModels = [];
    public ReadOnlyObservableCollection<ProcessViewModel> Processes { get; }

    // TODO: Deal with temporary duplicates having same id
    private readonly Dictionary<uint, ProcessViewModel> _allProcesses = new();
    // parent id => child list
    private readonly Dictionary<uint, List<ProcessViewModel>> _orphanedModels = new();

    public ProcessTreeViewModel(
        ILog logger,
        Lifetime lifetime,
        ObservableCollection<ProcessModel> models)
    {
        _logger = logger;
        var scheduler = RxApp.MainThreadScheduler;

        Processes = new ReadOnlyObservableCollection<ProcessViewModel>(_viewModels);

        lifetime.Bracket(
            () => models.CollectionChanged += ModelsOnCollectionChanged,
            () => models.CollectionChanged -= ModelsOnCollectionChanged);
        var currentModels = models.ToList();
        scheduler.Schedule(() => logger.Catch(() => OnReset(currentModels)));

        void ModelsOnCollectionChanged(object? _, NotifyCollectionChangedEventArgs e)
        {
            logger.Catch(() =>
            {
                Action? action = e.Action switch
                {
                    NotifyCollectionChangedAction.Add => () => OnAdd(e.NewItems!.Cast<ProcessModel>()),
                    NotifyCollectionChangedAction.Remove => () => OnRemove(e.OldItems!.Cast<ProcessModel>()),
                    NotifyCollectionChangedAction.Reset => () => OnReset(models.ToList()),
                    NotifyCollectionChangedAction.Replace => null,
                    NotifyCollectionChangedAction.Move => null,
                    _ => throw new ArgumentOutOfRangeException(null, $"Invalid action: {e.Action}")
                };
                if (action != null)
                    scheduler.Schedule(() => logger.Catch(action));
            });
        }
    }

    private void OnReset(List<ProcessModel> processes)
    {
        _viewModels.Clear();
        OnAdd(processes);
    }

    private void OnAdd(IEnumerable<ProcessModel> processes)
    {
        foreach (var process in processes)
        {
            var children = (IList<ProcessViewModel>?)_orphanedModels.GetValueOrDefault(process.Id)
                           ?? Array.Empty<ProcessViewModel>();
            foreach (var child in children)
            {
                // TODO: Optimize
                _viewModels.RemoveAt(_viewModels.IndexOf(child));
            }
            var viewModel = ProcessViewModel.Of(process) with
            {
                Children = children.ToImmutableArray()
            };
            _allProcesses.Add(process.Id, viewModel);

            if (process.ParentId is null)
            {
                _viewModels.Add(viewModel);
                continue;
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
                _viewModels[_viewModels.IndexOf(parent)] = _allProcesses[parentId] = parent with
                {
                    Children = parent.Children.Add(viewModel)
                };
            }
        }
    }

    private void OnRemove(IEnumerable<ProcessModel> processes)
    {
        foreach (var process in processes)
        {
            var viewModel = _allProcesses[process.Id];

            // 1. Remove from the process collection.
            _allProcesses.Remove(process.Id);

            // 2. Process' children are now orphans.
            if (viewModel.Children.Length > 0)
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
                    _allProcesses[parentId] = _viewModels[_viewModels.IndexOf(parent)] = parent with
                    {
                        Children = parent.Children.Remove(viewModel)
                    };

                    // As process cannot be an orphan or be in the root collection, terminate here.
                    continue;
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
}
