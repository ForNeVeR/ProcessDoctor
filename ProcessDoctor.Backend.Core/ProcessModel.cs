namespace ProcessDoctor.Backend.Core;

public record ProcessModel(
    uint Id,
    string Name,
    string CommandLine,
    string? ExecutablePath
);
