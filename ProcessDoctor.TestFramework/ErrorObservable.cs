using System.Reactive.Disposables;

namespace ProcessDoctor.TestFramework;

public class OnErrorObservable<T>(Exception exception) : IObservable<T>
{
    /// <inheritdoc />
    public IDisposable Subscribe(IObserver<T> observer)
    {
        observer.OnError(
            exception);

        return Disposable.Empty;
    }
}
