namespace ProcessDoctor.Backend.Linux.Proc.Interfaces;

public interface IProcessEntry
{
    uint Id { get; }

    string? CommandLine { get; }

    string? ExecutablePath { get; }

    IProcessStatus Status { get; }
}
