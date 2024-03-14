namespace ProcessDoctor.Backend.Linux.Proc;

public static class ProcPaths
{
    public const string Path = "/proc";

    public static class Status
    {
        public const string FileName = "status";
    }

    public static class CommandLine
    {
        public const string FileName = "cmdline";
    }

    public static class ExecutablePath
    {
        public const int MaxSize = 2048;
        
        public const string FileName = "exe";
    }
}