using ProcessDoctor.Backend.Linux.Proc.Interfaces;
using ProcessDoctor.Backend.Linux.Proc.StatusFile.Enums;

namespace ProcessDoctor.Backend.Linux.Tests.Fakes;

public sealed class FakeProcessStatus : IProcessStatus
{
    public static FakeProcessStatus Create(
        string? name = null,
        uint? parentId = null,
        ProcessState? state = null)
        => new(
            name ?? "ProcessDoctor",
            parentId,
            state ?? ProcessState.Running);

    public string Name { get; }
    
    public uint? ParentId { get; }
    
    public ProcessState State { get; }
    
    private FakeProcessStatus(
        string name, 
        uint? parentId, 
        ProcessState state)
    {
        Name = name;
        ParentId = parentId;
        State = state;
    }
}
