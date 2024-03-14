namespace ProcessDoctor.Backend.Linux.Proc.Exceptions;

public sealed class InvalidStatusFileException(string fileName)
    : Exception($"File is not a process file: {fileName}");
