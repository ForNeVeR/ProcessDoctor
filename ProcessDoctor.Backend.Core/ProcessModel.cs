namespace ProcessDoctor.Backend.Core;

public record ProcessModel(
    uint Id,
    uint? ParentId,
    string Name,
    string CommandLine
);
