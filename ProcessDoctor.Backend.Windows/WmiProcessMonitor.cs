﻿using System.Collections.ObjectModel;
using System.Management;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;

namespace ProcessDoctor.Backend.Windows;

public class WmiProcessMonitor : IProcessMonitor
{
    private readonly object _locker = new();
    public ObservableCollection<ProcessModel> Processes { get; } = [];

    public WmiProcessMonitor(Lifetime lifetime, ILog logger)
    {
        WatchEvents(lifetime,
            logger,
            "select * from __InstanceDeletionEvent within 1 where TargetInstance isa 'Win32_Process'",
            e =>
            {
                // TODO: Optimize this
                var process = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                var processId = Convert.ToUInt32(process.Properties["ProcessID"].Value);
                lock (_locker)
                {
                    var processModel = Processes.FirstOrDefault(x => x.Id == processId);
                    if (processModel != null)
                        Processes.Remove(processModel);
                }
            });
        WatchEvents(
            lifetime,
            logger,
            "select * from __InstanceCreationEvent within 1 where TargetInstance isa 'Win32_Process'",
            e =>
            {
                var process = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                var processId = Convert.ToUInt32(process.Properties["ProcessID"].Value);
                var processName = Convert.ToString(process.Properties["Name"].Value);
                var commandLine = Convert.ToString(process.Properties["CommandLine"].Value);
                var processModel = new ProcessModel(
                    Id: processId,
                    Name: processName ?? "<unknown>",
                    CommandLine: commandLine ?? "");
                lock (_locker)
                    Processes.Add(processModel);
            });
    }

    private static void WatchEvents(Lifetime lifetime, ILog logger, string query, Action<EventArrivedEventArgs> handler)
    {
        var watcher = new ManagementEventWatcher(new WqlEventQuery(query));
        lifetime.AddDispose(watcher);

        lifetime.Bracket(
            () => watcher.EventArrived += OnWatcherOnEventArrived,
            () => watcher.EventArrived -= OnWatcherOnEventArrived);

        watcher.Start();

        void OnWatcherOnEventArrived(object _, EventArrivedEventArgs e)
        {
            logger.Catch(() =>
            {
                handler(e);
            });
        }

        watcher.Start();
    }
}
