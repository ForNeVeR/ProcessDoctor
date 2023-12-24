using System;
using Avalonia;
using Avalonia.ReactiveUI;
using JetBrains.Annotations;

namespace ProcessDoctor;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    [UsedImplicitly] // used by previewer
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
