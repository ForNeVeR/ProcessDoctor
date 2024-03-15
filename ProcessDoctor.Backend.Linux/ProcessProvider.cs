using System.Reactive.Linq;
using JetBrains.Diagnostics;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Core.Enums;
using ProcessDoctor.Backend.Core.Interfaces;
using ProcessDoctor.Backend.Linux.Proc;
using ProcessDoctor.Backend.Linux.Proc.Interfaces;

namespace ProcessDoctor.Backend.Linux;

public sealed class ProcessProvider(ILog logger, IProcFolderEntry procFolderEntry) : IProcessProvider
{
    public IObservable<SystemProcess> ObserveProcesses(ObservationTarget observationTarget)
    {
        var cachedProcesses = CreateSnapshot()
            .EnumerateProcesses()
            .ToDictionary(process => process.Id);

        logger.Info(
            "File system event watcher has been started. Event type: {0}",
            observationTarget);

        return Observable
            .Create<SystemProcess>(async (observer, token) =>
            {
                token.Register(observer.OnCompleted);

                var timer = new PeriodicTimer(TimeSpan.FromSeconds(0.5));

                while (!token.IsCancellationRequested)
                {
                    var processes = procFolderEntry
                        .EnumerateProcessDirectories()
                        .ToDictionary(processDirectory => uint.Parse(processDirectory.Name));

                    foreach (var launchedProcess in processes.Where(pair => !cachedProcesses.ContainsKey(pair.Key)))
                    {
                        var processEntry = ProcessEntry.Create(launchedProcess.Value);
                        var linuxProcess = LinuxProcess.Create(processEntry);

                        cachedProcesses.Add(linuxProcess.Id, linuxProcess);

                        if (observationTarget is ObservationTarget.Launched)
                        {
                            observer.OnNext(linuxProcess);
                        }
                    }

                    var terminatedProcesses = cachedProcesses
                        .Where(cachedProcess => !processes.ContainsKey(cachedProcess.Key))
                        .ToArray();

                    foreach (var terminatedProcess in terminatedProcesses)
                    {
                        cachedProcesses.Remove(terminatedProcess.Key);

                        if (observationTarget is ObservationTarget.Terminated)
                        {
                            observer.OnNext(terminatedProcess.Value);
                        }
                    }

                    await timer.WaitForNextTickAsync(token);
                }
            })
            .Finally(() => logger.Info("File system event watcher has been stopped. Event type: {0}", observationTarget))
            .Catch((Exception exception) =>
            {
                logger.Error(exception, "An error occurred while processing an event received from the file system");

                return ObserveProcesses(observationTarget);
            });
    }

    public IProcessListSnapshot CreateSnapshot()
        => new ProcessListSnapshot(
            Log.GetLog<ProcessListSnapshot>(),
            procFolderEntry);
}
