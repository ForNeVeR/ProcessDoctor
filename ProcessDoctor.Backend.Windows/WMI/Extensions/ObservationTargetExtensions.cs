using System.Management;
using ProcessDoctor.Backend.Core.Enums;

namespace ProcessDoctor.Backend.Windows.WMI.Extensions;

internal static class ObservationTargetExtensions
{
    internal static WqlEventQuery ToWqlQuery(this ObservationTarget observationTarget)
        => observationTarget switch
        {
            ObservationTarget.Launched
                => new WqlEventQuery(
                    "select * from __InstanceCreationEvent within 1 where TargetInstance isa 'Win32_Process'"),

            ObservationTarget.Terminated
                => new WqlEventQuery(
                    "select * from __InstanceDeletionEvent within 1 where TargetInstance isa 'Win32_Process'"),

            _ => throw new ArgumentOutOfRangeException(
                nameof(observationTarget),
                observationTarget,
                $"Process state {observationTarget} is not supported")
        };
}
