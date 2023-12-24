using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Windows;
using ProcessDoctor.ViewModels;
using ProcessDoctor.Views;

namespace ProcessDoctor;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var lifetime = CreateAppLifetime();
        var backend = new WmiProcessMonitor(lifetime, Log.GetLog<WmiProcessMonitor>());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(lifetime, backend)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private Lifetime CreateAppLifetime()
    {
        var avaloniaLifetime = ApplicationLifetime as IControlledApplicationLifetime;
        if (avaloniaLifetime == null)
        {
            // Designer mode detected.
            return Lifetime.Eternal;
        }

        var lifetime = new LifetimeDefinition();
        lifetime.Lifetime.Bracket(
            () => avaloniaLifetime.Exit += OnExit,
            () => avaloniaLifetime.Exit -= OnExit);

        void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            lifetime.Terminate();
        }

        return lifetime.Lifetime;
    }
}
