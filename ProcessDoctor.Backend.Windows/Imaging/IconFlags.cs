namespace ProcessDoctor.Backend.Windows.Imaging;

[Flags]
internal enum IconFlags : uint
{
    LargeSize = 0x0,
    SmallSize = 0x1,
    Icon = 0x100
}
