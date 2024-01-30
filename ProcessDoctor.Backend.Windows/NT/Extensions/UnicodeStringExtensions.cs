using System.Text;
using PInvoke;

namespace ProcessDoctor.Backend.Windows.NT.Extensions;

internal static class UnicodeStringExtensions
{
    internal static string? ToManagedString(this NTDll.UNICODE_STRING unmanagedString, ProcessMemoryReader memoryReader)
    {
        if (unmanagedString.Length == 0 || unmanagedString.MaximumLength == 0)
        {
            return null;
        }

        var bytes = memoryReader.ReadBytes(
            unmanagedString.Buffer_IntPtr,
            unmanagedString.Length);

        return Encoding.Unicode.GetString(bytes);
    }
}
