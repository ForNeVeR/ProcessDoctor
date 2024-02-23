using JetBrains.Diagnostics;
using PInvoke;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Core.Interfaces;

namespace ProcessDoctor.Backend.Windows;

internal sealed class ProcessListSnapshot : IProcessListSnapshot
{
    private readonly ILog _logger;

    private readonly Kernel32.SafeObjectHandle _snapshotHandle;

    internal ProcessListSnapshot(ILog logger, Kernel32.SafeObjectHandle snapshotHandle)
    {
        _logger = logger;
        _snapshotHandle = snapshotHandle;
    }

    public IEnumerable<SystemProcess> EnumerateProcesses()
    {
        if (_snapshotHandle.IsInvalid)
        {
            _logger.Warn("Invalid snapshot handle received");

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
