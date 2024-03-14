namespace ProcessDoctor.Backend.Linux.Proc.Exceptions;

public sealed class InvalidProcessDirectoryException(string directoryPath)
    : Exception($"Directory is not a process: {directoryPath}");
