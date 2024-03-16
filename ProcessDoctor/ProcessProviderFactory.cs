using System;
using System.IO.Abstractions;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core.Interfaces;
using ProcessDoctor.Backend.Linux.Proc;
using ProcessDoctor.Backend.Windows.WMI;

namespace ProcessDoctor;

public sealed class ProcessProviderFactory
{
    public IProcessProvider Create(Lifetime lifetime)
    {
        if (OperatingSystem.IsWindows())
            return new Backend.Windows.ProcessProvider(
                lifetime,
                Log.GetLog<Backend.Windows.ProcessProvider>(),
                new ManagementEventWatcherAdapterFactory());

        if (OperatingSystem.IsLinux())
            return new Backend.Linux.ProcessProvider(
                Log.GetLog<Backend.Linux.ProcessProvider>(),
                new ProcFolderEntry(
                    new FileSystem()));

        throw new PlatformNotSupportedException();
    }
}
