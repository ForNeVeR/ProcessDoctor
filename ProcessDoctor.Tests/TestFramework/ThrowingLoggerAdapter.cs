using JetBrains.Diagnostics;
using Xunit.Abstractions;

namespace ProcessDoctor.Tests.TestFramework;

public class ThrowingLoggerAdapter(string category, ITestOutputHelper output) : ILog, IDisposable
{
    public string Category { get; } = category;
    public bool IsEnabled(LoggingLevel level) => level >= LoggingLevel.INFO;

    private readonly List<Exception> _exceptions = [];

    public void Log(LoggingLevel level, string? message, Exception? exception = null)
    {
        output.WriteLine($"{level} [{Category}]: {message} {exception}");
        if (level >= LoggingLevel.ERROR)
        {
            lock (_exceptions)
                _exceptions.Add(exception ?? new Exception(message ?? "<unknown error>"));
        }
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
