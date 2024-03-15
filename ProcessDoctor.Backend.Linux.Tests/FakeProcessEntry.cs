using ProcessDoctor.Backend.Linux.Proc.Interfaces;

namespace ProcessDoctor.Backend.Linux.Tests;

public sealed class FakeProcessEntry : IProcessEntry
{
    public static FakeProcessEntry Create(
        uint id,
        string? commandLine = null,
        string? executablePath = null,
        IProcessStatus? status = null)
        => new(
            id,
            commandLine,
            executablePath,
            status ?? FakeProcessStatus.Create());

    public uint Id { get; }

    public string? CommandLine { get; }

    public string? ExecutablePath { get; }

    public IProcessStatus Status { get; }

    public FakeProcessEntry(
        uint id,
        string? commandLine,
        string? executablePath,
        IProcessStatus status)
    {
        Id = id;
        CommandLine = commandLine;
        ExecutablePath = executablePath;
        Status = status;
    }
}
