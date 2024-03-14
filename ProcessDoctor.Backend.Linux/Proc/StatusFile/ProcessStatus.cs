using System.IO.Abstractions;
using ProcessDoctor.Backend.Linux.Proc.Exceptions;
using ProcessDoctor.Backend.Linux.Proc.StatusFile.Enums;

namespace ProcessDoctor.Backend.Linux.Proc.StatusFile;

public sealed class ProcessStatus
{
    private const char Separator = ':';
    private readonly string[] _lines;

    public static ProcessStatus Create(IFileInfo statusFile)
    {
        if (statusFile.Name != ProcPaths.Status.FileName)
            throw new InvalidStatusFileException(statusFile.Name);

        var lines = statusFile
            .FileSystem
            .File
            .ReadAllLines(statusFile.FullName);

        return new ProcessStatus(lines);
    }

    public string Name
        => ReadPropertyValue(StatusProperty.Name)
            ?? throw new InvalidStatusFilePropertyException(StatusProperty.Name);

    public uint? ParentId
    {
        get
        {
            var value = ReadPropertyValue(StatusProperty.ParentId);

            if (!uint.TryParse(value, out var parentId))
                throw new InvalidStatusFilePropertyException(StatusProperty.ParentId);

            if (parentId is 0)
                return null;

            return parentId;
        }
    }

    public ProcessState State
    {
        get
        {
            var value = ReadPropertyValue(StatusProperty.State)?
                .First();

            var state = value switch
            {
                'R' => ProcessState.Running,
                'S' => ProcessState.Sleeping,
                'D' => ProcessState.UninterruptibleWait,
                'Z' => ProcessState.Zombie,
                'T' => ProcessState.TracedOrStopped,
                _ => default(ProcessState?)
            };

            if (state is null)
                throw new InvalidStatusFilePropertyException(StatusProperty.State);

            return state.Value;
        }
    }

    private string? ReadPropertyValue(StatusProperty property)
    {
        var lineIndex = (int)property;

        if (lineIndex < 0)
            throw new ArgumentException(
                "Line index cannot be less than 0",
                nameof(lineIndex));

        if (_lines.Length < lineIndex)
            throw new InvalidStatusFilePropertyException(property);

        var line = _lines
            .Skip(lineIndex - 1)
            .FirstOrDefault()
                ?? throw new InvalidStatusFilePropertyException(property);

        var separatorIndex = line.IndexOf(Separator, StringComparison.Ordinal);

        if (separatorIndex is -1)
            throw new InvalidStatusFilePropertyException(property);

        var value = line
            .Substring(separatorIndex + 1, line.Length - separatorIndex - 1)
            .Trim();

        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value;
    }

    private ProcessStatus(string[] lines)
        => _lines = lines;
}
