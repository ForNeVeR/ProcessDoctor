using System.Management;
using PInvoke;
using ProcessDoctor.Backend.Core;
using ProcessDoctor.Backend.Windows.NT;
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
            parentId: processId != parentId ? parentId : null,
            name: processName ?? "<unknown>",
            commandLine: commandLine ?? string.Empty,
            executablePath: executablePath);
    }

    public static WindowsProcess Create(Kernel32.PROCESSENTRY32 processEntry)
    {
        var processId = processEntry.th32ProcessID;
        var parentId = Convert.ToUInt32(processEntry.th32ParentProcessID);

        using var processHandle = Kernel32.OpenProcess(
            Kernel32.ProcessAccess.PROCESS_VM_READ | Kernel32.ProcessAccess.PROCESS_QUERY_INFORMATION,
            bInheritHandle: false,
            processId);

        if (processHandle is not null && !processHandle.IsInvalid)
        {
            return Create(processEntry, processHandle);
        }

        return new WindowsProcess(
            id: Convert.ToUInt32(processId),
            parentId: processId != parentId ? parentId : null,
            name: processEntry.ExeFile,
            commandLine: string.Empty,
            executablePath: null);
    }

    private static unsafe WindowsProcess Create(Kernel32.PROCESSENTRY32 processEntry, Kernel32.SafeObjectHandle processHandle)
    {
        var processId = Convert.ToUInt32(processEntry.th32ProcessID);
        var parentId = Convert.ToUInt32(processEntry.th32ParentProcessID);

        var basicInformation = processHandle.GetBasicInformation();
        var peb = processHandle.ReadStructure<PEB_64>(basicInformation.PebBaseAddress);
        var parameters = processHandle.ReadStructure<RTL_USER_PROCESS_PARAMETERS>(peb.ProcessParameters);

        return new WindowsProcess(
            id: processId,
            parentId: processId != parentId ? parentId : null,
            name: processEntry.ExeFile,
            commandLine: parameters.CommandLine.ToManagedString(processHandle),
            executablePath: parameters.ImagePathName.ToManagedString(processHandle));
    }

    /// <inheritdoc />
    private WindowsProcess(uint id, uint? parentId, string name, string? commandLine, string? executablePath)
        : base(id, parentId, name, commandLine, executablePath)
    { }
}
