using ProcessDoctor.Backend.Linux.Proc.StatusFile.Enums;

namespace ProcessDoctor.Backend.Linux.Proc.Exceptions;

public sealed class InvalidStatusFilePropertyException(StatusProperty property, Exception? innerException = null)
    : Exception(
        $"An error occurred while reading status file property. Property: {property}. Line index: {(int)property}",
        innerException);
