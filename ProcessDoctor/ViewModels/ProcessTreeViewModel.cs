using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;
using ReactiveUI;

namespace ProcessDoctor.ViewModels;

public static class ProcessTreeViewModel
{
    public static ReadOnlyObservableCollection<ProcessViewModel> ConvertToTree(
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
