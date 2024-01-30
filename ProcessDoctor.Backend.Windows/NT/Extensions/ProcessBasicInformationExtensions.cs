using PInvoke;
using ProcessDoctor.Backend.Windows.Exceptions;

namespace ProcessDoctor.Backend.Windows.NT.Extensions;

internal static class ProcessBasicInformationExtensions
{
    internal static unsafe
#if TARGET_64BIT
        PEB_64
#else
        PEB_32
#endif
        ReadPeb(this NTDll.PROCESS_BASIC_INFORMATION basicInformation, ProcessMemoryReader memoryReader)
    {
#if TARGET_64BIT
        var peb = memoryReader.ReadStructure<PEB_64>(basicInformation.PebBaseAddress);
#else
        var peb = memoryReader.ReadStructure<PEB_32>(basicInformation.PebBaseAddress);
#endif

        if (peb is null)
        {
            throw new CorruptedProcessHandleException();
        }

        return peb.Value;
    }
}