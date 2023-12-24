using System.Collections.ObjectModel;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.ViewModels;

namespace ProcessDoctor.Tests;

public class ProcessTreeViewModelTests
{
    [Fact]
    public void ListBehaviorTest()
    {
        var models = new ObservableCollection<ProcessModel>();
        var tree = ProcessTreeViewModel.ConvertToTree(Lifetime.Eternal, models);
        ProcessModel foo = new(1u, "foo", "foo 1");
        ProcessModel bar = new(2u, "bar", "bar 2");

        models.Add(foo);
        models.Add(bar);

        Assert.Collection(
            tree,
            x => Assert.Equal(x, ProcessViewModel.Of(foo)),
            x => Assert.Equal(x, ProcessViewModel.Of(bar)));

        models.Remove(foo);
        Assert.Collection(
            tree,
            x => Assert.Equal(x, ProcessViewModel.Of(bar)));
    }
}
