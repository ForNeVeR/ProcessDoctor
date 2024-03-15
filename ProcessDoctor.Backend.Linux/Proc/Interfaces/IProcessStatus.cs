using ProcessDoctor.Backend.Linux.Proc.StatusFile.Enums;

namespace ProcessDoctor.Backend.Linux.Proc.Interfaces;

public interface IProcessStatus
{
    string Name { get; }

    uint? ParentId { get; }

    ProcessState State { get; }
}
