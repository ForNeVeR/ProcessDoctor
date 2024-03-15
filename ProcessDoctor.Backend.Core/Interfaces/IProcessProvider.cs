using ProcessDoctor.Backend.Core.Enums;

namespace ProcessDoctor.Backend.Core.Interfaces;

public interface IProcessProvider
{
    IObservable<SystemProcess> ObserveProcesses(ObservationTarget observationTarget);

    IProcessListSnapshot CreateSnapshot();
}
