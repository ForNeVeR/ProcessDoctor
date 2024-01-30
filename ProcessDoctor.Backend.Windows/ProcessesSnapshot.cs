using PInvoke;
using ProcessDoctor.Backend.Core;

namespace ProcessDoctor.Backend.Windows;

internal sealed class ProcessesSnapshot : IDisposable
{
    private readonly Kernel32.SafeObjectHandle _snapshotHandle;

    public static ProcessesSnapshot Create()
    {
        var nativeHandle = Kernel32.CreateToolhelp32Snapshot(
            Kernel32.CreateToolhelp32SnapshotFlags.TH32CS_SNAPPROCESS,
            th32ProcessID: 0);

        return new ProcessesSnapshot(nativeHandle);
    }

    private ProcessesSnapshot(Kernel32.SafeObjectHandle snapshotHandle)
        => _snapshotHandle = snapshotHandle;

    public IEnumerable<SystemProcess> EnumerateProcesses()
    {
        if (_snapshotHandle.IsInvalid)
        {
            yield break;
        }

        var processEntry = Kernel32.PROCESSENTRY32.Create();

        if (!Kernel32.Process32First(_snapshotHandle, ref processEntry))
        {
            yield break;
        }

        do
        {
            yield return WindowsProcess.Create(processEntry);
        }
        while (Kernel32.Process32Next(_snapshotHandle, ref processEntry));
    }

    /// <inheritdoc />
    public void Dispose()
        => _snapshotHandle.Dispose();
}
