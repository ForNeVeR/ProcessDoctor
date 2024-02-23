using System.Reactive;
using Microsoft.Reactive.Testing;

namespace ProcessDoctor.TestFramework.Extensions;

public static class TestableObserverExtensions
{
    public static IEnumerable<T> EnumerateMessages<T>(this ITestableObserver<T> observer)
        => observer
            .Messages
            .Where(message => message.Value.Kind == NotificationKind.OnNext)
            .Select(message => message.Value.Value);
}
