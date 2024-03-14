namespace ProcessDoctor.Backend.Windows.Imaging.Native;

internal readonly struct HRESULT
{
    private readonly int _value;

    internal HRESULT(int value)
        => _value = value;

    internal bool Succeeded
        => _value >= 0;


    internal bool Failed
        => _value < 0;
}
