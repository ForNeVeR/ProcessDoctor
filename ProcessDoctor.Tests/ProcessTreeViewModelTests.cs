using System.Collections.ObjectModel;
using FluentAssertions;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.TestFramework;
using ProcessDoctor.TestFramework.Logging;
using ProcessDoctor.ViewModels;
using Xunit.Abstractions;

namespace ProcessDoctor.Tests;

public class ProcessTreeViewModelTests(ITestOutputHelper output)
{
    [Fact]
    public void Should_handle_parent_processes_properly()
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessTreeViewModelTests), output);
        using var lifetimeScope = new LifetimeDefinition();
        var models = new ObservableCollection<SystemProcess>();
        using var processMonitor = new ObservableCollectionProcessMonitor(models);
        var tree = new ProcessTreeViewModel(logger, lifetimeScope.Lifetime, processMonitor).Processes;

        var foo = new FakeProcess(1u, null, "foo", "foo 1", null);
        var bar = new FakeProcess(2u, null, "bar", "bar 2", null);

        // Act
        models.Add(foo);
        models.Add(bar);

        // Assert
        tree.Should()
            .BeEquivalentTo(
                new[]
                {
                    ProcessViewModel.Of(foo),
                    ProcessViewModel.Of(bar)
                },
                options => options.WithStrictOrdering());

        // Act
        models.Remove(foo);

        // Assert
        tree.Should()
            .BeEquivalentTo(
                new[] { ProcessViewModel.Of(bar) },
                options => options.WithStrictOrdering());
    }

    [Fact]
    public void Should_handle_child_processes_properly()
    {
        // Arrange
        using var logger = new ThrowingLoggerAdapter(nameof(ProcessTreeViewModelTests), output);
        using var lifetimeScope = new LifetimeDefinition();
        var models = new ObservableCollection<SystemProcess>();
        using var processMonitor = new ObservableCollectionProcessMonitor(models);
        var tree = new ProcessTreeViewModel(logger, lifetimeScope.Lifetime, processMonitor).Processes;

        var foo1 = new FakeProcess(1u, null, "foo1", "foo1 1", null);
        var foo2 = new FakeProcess(2u, foo1.Id, "foo2", "foo2 2", null);
        var foo3 = new FakeProcess(3u, foo2.Id, "foo3", "foo3 3", null);
        var bar = new FakeProcess(4u, null, "bar", "bar 3", null);

        // Act
        models.Add(foo1);
        models.Add(bar);

        // Assert
        tree.Should()
            .BeEquivalentTo(
                new[]
                {
                    ProcessViewModel.Of(foo1),
                    ProcessViewModel.Of(bar)
                },
                options => options.WithStrictOrdering());

        // Act
        models.Add(foo2);

        // Assert
        tree.Should()
            .BeEquivalentTo(
                new[]
                {
                    ProcessViewModel.Of(foo1) with
                    {
                        Children = [ProcessViewModel.Of(foo2)]
                    },
                    ProcessViewModel.Of(bar)
                },
                options => options.WithStrictOrdering());

        // Act
        models.Remove(foo1);

        // Assert
        tree.Should()
            .BeEquivalentTo(
                new[]
                {
                    ProcessViewModel.Of(bar),
                    ProcessViewModel.Of(foo2)
                },
                options => options.WithStrictOrdering());

        // Act
        models.Add(foo3);
        models.Add(foo1);

        // Assert
        tree.Should()
            .BeEquivalentTo(
                new[]
                {
                    ProcessViewModel.Of(bar),
                    ProcessViewModel.Of(foo1) with
                    {
                        Children = [ProcessViewModel.Of(foo2) with
                        {
                            Children = [ProcessViewModel.Of(foo3)]
                        }]
                    }
                },
                options => options.WithStrictOrdering());
    }
}
