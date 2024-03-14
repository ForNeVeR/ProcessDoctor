namespace ProcessDoctor.Backend.Linux.Proc.Exceptions;

public sealed class StatusFileNotFoundException(string processPath)
    : Exception($"Status file not found: {processPath}");
