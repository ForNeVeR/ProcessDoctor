using System.Collections.Immutable;
using System.Collections.ObjectModel;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Tests.TestFramework;
using ProcessDoctor.ViewModels;
using Xunit.Abstractions;

namespace ProcessDoctor.Tests;

public class ProcessTreeViewModelTests(ITestOutputHelper output)
{
    [Fact]
    public void ListBehaviorTest()
    {
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessTreeViewModelTests), output);
        using var ld = new LifetimeDefinition();
        var models = new ObservableCollection<ProcessModel>();
        var tree = new ProcessTreeViewModel(logger, ld.Lifetime, models).Processes;

        ProcessModel foo = new(1u, null, "foo", "foo 1");
        ProcessModel bar = new(2u, null, "bar", "bar 2");

        models.Add(foo);
        models.Add(bar);

        Assert.Collection(
            tree,
            x => Assert.Equivalent(ProcessViewModel.Of(foo), x),
            x => Assert.Equivalent(ProcessViewModel.Of(bar), x));

        models.Remove(foo);
        Assert.Collection(
            tree,
            x => Assert.Equivalent(ProcessViewModel.Of(bar), x));
    }

    [Fact]
    public void TreeBehaviorTest()
    {
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessTreeViewModelTests), output);
        using var ld = new LifetimeDefinition();
        var models = new ObservableCollection<ProcessModel>();
        var tree = new ProcessTreeViewModel(logger, ld.Lifetime, models).Processes;

        ProcessModel foo1 = new(1u, null, "foo1", "foo1 1");
        ProcessModel foo2 = new(2u, foo1.Id, "foo2", "foo2 2");
        ProcessModel bar = new(3u, null, "bar", "bar 3");

        models.Add(foo1);
        models.Add(bar);

        Assert.Collection(
            tree,
            x => Assert.Equivalent(ProcessViewModel.Of(foo1), x),
            x => Assert.Equivalent(ProcessViewModel.Of(bar), x));

        models.Add(foo2);

        Assert.Collection(
            tree,
            x =>
            {
                Assert.Equivalent(ProcessViewModel.Of(foo1) with
                {
                    Children = ImmutableArray.Create(ProcessViewModel.Of(foo2))
                }, x, strict: true);
                Assert.Equivalent(ProcessViewModel.Of(foo2), Assert.Single(x.Children));
            },
            x => Assert.Equivalent(x, ProcessViewModel.Of(bar)));

        models.Remove(foo1);
        Assert.Collection(
            tree,
            x => Assert.Equivalent(ProcessViewModel.Of(bar), x),
            x => Assert.Equivalent(ProcessViewModel.Of(foo2), x));
    }
}
