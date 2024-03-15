using JetBrains.Diagnostics;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Core.Interfaces;
using ProcessDoctor.Backend.Linux.Proc;
using ProcessDoctor.Backend.Linux.Proc.Interfaces;

namespace ProcessDoctor.Backend.Linux;

public sealed class ProcessListSnapshot(ILog logger, IProcFolderEntry procFolderEntry) : IProcessListSnapshot
{
    public IEnumerable<SystemProcess> EnumerateProcesses()
        => logger.Catch(() =>
            procFolderEntry
                .EnumerateProcessDirectories()
                .Select(ProcessEntry.Create)
                .Select(LinuxProcess.Create))
                    ?? Enumerable.Empty<SystemProcess>();

    public void Dispose()
    { }
}
