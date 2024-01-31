using System.Collections.ObjectModel;
using System.Management;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using ProcessDoctor.Backend.Core;

namespace ProcessDoctor.Backend.Windows;

public class WmiProcessMonitor : IProcessMonitor
{
    private readonly object _locker = new();
    public ObservableCollection<SystemProcess> Processes { get; } = [];

    public WmiProcessMonitor(Lifetime lifetime, ILog logger)
    {
        using var snapshot = ProcessesSnapshot.Create();

        foreach (var process in snapshot.EnumerateProcesses())
        {
            Processes.Add(process);
        }

        WatchEvents(lifetime,
            logger,
            "select * from __InstanceDeletionEvent within 1 where TargetInstance isa 'Win32_Process'",
            e =>
            {
                // TODO[#5]: Optimize this
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
                var processModel = WindowsProcess.Create((ManagementBaseObject)e.NewEvent["TargetInstance"]);

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
        return;

        void OnWatcherOnEventArrived(object _, EventArrivedEventArgs e)
        {
            logger.Catch(() =>
            {
                handler(e);
            });
        }
    }
}
