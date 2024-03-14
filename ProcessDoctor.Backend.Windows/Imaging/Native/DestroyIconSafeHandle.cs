using System.Runtime.InteropServices;

namespace ProcessDoctor.Backend.Windows.Imaging.Native;

internal sealed class DestroyIconSafeHandle : SafeHandle
{
    private static readonly IntPtr InvalidValue = new(-1L);

    internal DestroyIconSafeHandle()
        : base(InvalidValue, ownsHandle: true)
    { }

    internal DestroyIconSafeHandle(IntPtr preexistingHandle, bool ownsHandle = true)
        : base(InvalidValue, ownsHandle)
        => SetHandle(preexistingHandle);

    public override bool IsInvalid
        => handle.ToInt64() == -1L || handle.ToInt64() == 0L;

    protected override bool ReleaseHandle()
        => User32.DestroyIcon(handle);
}
