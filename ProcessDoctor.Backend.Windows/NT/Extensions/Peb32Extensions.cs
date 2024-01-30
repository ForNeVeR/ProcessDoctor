using ProcessDoctor.Backend.Windows.Exceptions;

namespace ProcessDoctor.Backend.Windows.NT.Extensions;

internal static class Peb32Extensions
{
    internal static RTL_USER_PROCESS_PARAMETERS ReadParameters(this PEB_32 peb, ProcessMemoryReader memoryReader)
    {
        var parameters = memoryReader.ReadStructure<RTL_USER_PROCESS_PARAMETERS>(peb.ProcessParameters);

        if (parameters is null)
        {
            throw new CorruptedProcessHandleException();
        }

        return parameters.Value;
    }
}