using System;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
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
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new Backend.Windows.ProcessProvider(
                lifetime,
                Log.GetLog<Backend.Windows.ProcessProvider>(),
                new ManagementEventWatcherAdapterFactory());

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return new Backend.Linux.ProcessProvider(
                Log.GetLog<Backend.Linux.ProcessProvider>(),
                new ProcFolderEntry(
                    new FileSystem()));

        throw new PlatformNotSupportedException();
    }
}
