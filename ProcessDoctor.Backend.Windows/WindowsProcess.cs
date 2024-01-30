using System.Management;
using PInvoke;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Windows.NT.Extensions;

namespace ProcessDoctor.Backend.Windows;

internal sealed record WindowsProcess : SystemProcess
{
    public static WindowsProcess Create(ManagementBaseObject processInstance)
    {
        var processId = Convert.ToUInt32(processInstance.Properties["ProcessId"].Value);
        var parentId = Convert.ToUInt32(processInstance.Properties["ParentProcessId"].Value);
        var processName = Convert.ToString(processInstance.Properties["Name"].Value);
        var commandLine = Convert.ToString(processInstance.Properties["CommandLine"].Value);
        var executablePath = Convert.ToString(processInstance.Properties["ExecutablePath"].Value);

        return new WindowsProcess(
            id: processId,
            parentId,
            name: processName ?? "<unknown>",
            commandLine: commandLine ?? string.Empty,
            executablePath: executablePath);
    }

    public static WindowsProcess Create(Kernel32.PROCESSENTRY32 processEntry)
    {
        using var processHandle = Kernel32.OpenProcess(
            Kernel32.ProcessAccess.PROCESS_VM_READ | Kernel32.ProcessAccess.PROCESS_QUERY_INFORMATION,
            bInheritHandle: false,
            processEntry.th32ProcessID);

        if (processHandle is null || processHandle.IsInvalid)
        {
            return new WindowsProcess(
                id: (uint)processEntry.th32ProcessID,
                parentId: (uint)processEntry.th32ParentProcessID,
                name: processEntry.ExeFile,
                commandLine: string.Empty,
                executablePath: null);
        }

        return Create(processEntry, processHandle);
    }

    private static WindowsProcess Create(Kernel32.PROCESSENTRY32 processEntry, Kernel32.SafeObjectHandle processHandle)
    {
        var memoryReader = new ProcessMemoryReader(processHandle);
        var basicInformation = processHandle.GetBasicInformation();
        var peb = basicInformation.ReadPeb(memoryReader);
        var parameters = peb.ReadParameters(memoryReader);

        return new WindowsProcess(
            id: (uint)processEntry.th32ProcessID,
            parentId: (uint)processEntry.th32ParentProcessID,
            name: processEntry.ExeFile,
            commandLine: parameters.CommandLine.ToManagedString(memoryReader) ?? string.Empty,
            executablePath: parameters.ImagePathName.ToManagedString(memoryReader));
    }

    /// <inheritdoc />
    private WindowsProcess(uint id, uint? parentId, string name, string commandLine, string? executablePath)
        : base(id, parentId, name, commandLine, executablePath)
    { }
}
