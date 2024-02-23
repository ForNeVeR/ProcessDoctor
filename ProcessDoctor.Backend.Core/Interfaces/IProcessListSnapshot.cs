namespace ProcessDoctor.Backend.Core.Interfaces;

public interface IProcessListSnapshot : IDisposable
{
    IEnumerable<SystemProcess> EnumerateProcesses();
}
