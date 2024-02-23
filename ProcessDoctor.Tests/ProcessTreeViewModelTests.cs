using System.Collections.ObjectModel;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.TestFramework.Logging;
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
        var models = new ObservableCollection<SystemProcess>();
        var tree = new ProcessTreeViewModel(logger, ld.Lifetime, models).Processes;

        FakeProcess foo = new(1u, null, "foo", "foo 1", null);
        FakeProcess bar = new(2u, null, "bar", "bar 2", null);

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
        var models = new ObservableCollection<SystemProcess>();
        var tree = new ProcessTreeViewModel(logger, ld.Lifetime, models).Processes;

        FakeProcess foo1 = new(1u, null, "foo1", "foo1 1", null);
        FakeProcess foo2 = new(2u, foo1.Id, "foo2", "foo2 2", null);
        FakeProcess foo3 = new(3u, foo2.Id, "foo3", "foo3 3", null);
        FakeProcess bar = new(4u, null, "bar", "bar 3", null);

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
                    Children = [ProcessViewModel.Of(foo2)]
                }, x, strict: true);
            },
            x => Assert.Equivalent(x, ProcessViewModel.Of(bar)));

        models.Remove(foo1);
        Assert.Collection(
            tree,
            x => Assert.Equivalent(ProcessViewModel.Of(bar), x),
            x => Assert.Equivalent(ProcessViewModel.Of(foo2), x));

        models.Add(foo3);
        models.Add(foo1);

        Assert.Collection(
            tree,
            x => Assert.Equivalent(x, ProcessViewModel.Of(bar)),
            x =>
            {
                Assert.Equivalent(ProcessViewModel.Of(foo1) with
                {
                    Children = [ProcessViewModel.Of(foo2) with
                    {
                        Children = [ProcessViewModel.Of(foo3)]
                    }]
                }, x, strict: true);
            });
    }
}
