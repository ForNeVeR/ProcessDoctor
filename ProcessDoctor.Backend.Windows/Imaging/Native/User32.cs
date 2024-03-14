using System.Runtime.InteropServices;

namespace ProcessDoctor.Backend.Windows.Imaging.Native;

internal static class User32
{
    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    internal static extern bool DestroyIcon(IntPtr hIcon);
}
