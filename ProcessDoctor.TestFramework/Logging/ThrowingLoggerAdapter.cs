using JetBrains.Diagnostics;
using Xunit.Abstractions;

namespace ProcessDoctor.TestFramework.Logging;

public sealed class ThrowingLoggerAdapter(string category, ITestOutputHelper output) : ILog, IDisposable
{
    private readonly List<Exception> _exceptions = [];

    public string Category { get; } = category;

    public bool IsEnabled(LoggingLevel level)
        => level >= LoggingLevel.INFO;

    public void Log(LoggingLevel level, string? message, Exception? exception = null)
    {
        output.WriteLine($"{level} [{Category}]: {message} {exception}");

        if (level > LoggingLevel.ERROR)
            return;

        if (exception is FakeException)
            return;

        lock (_exceptions)
            _exceptions.Add(exception ?? new Exception(message ?? "<unknown error>"));
    }

    public void Dispose()
    {
        lock (_exceptions)
        {
            if (_exceptions.Count > 0)
                throw new AggregateException(_exceptions);
        }
    }
}
