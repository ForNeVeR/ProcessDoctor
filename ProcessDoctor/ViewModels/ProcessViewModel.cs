using System;
using System.Collections.Immutable;
using System.Linq;
using ProcessDoctor.Backend.Core;

namespace ProcessDoctor.ViewModels;

public record ProcessViewModel(
    uint Id,
    uint? ParentId,
    string Name,
    string CommandLine,
    ImmutableArray<ProcessViewModel> Children)
{
    public static ProcessViewModel Of(ProcessModel model) => new(
        model.Id,
        model.ParentId,
        model.Name,
        model.CommandLine,
        ImmutableArray.Create<ProcessViewModel>());

    public override int GetHashCode()
    {
        // Let's ignore the children here, there should barely be any collisions in practice because of that.
        return HashCode.Combine(Id, ParentId, Name, CommandLine);
    }

    public virtual bool Equals(ProcessViewModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id
               && ParentId == other.ParentId
               && Name == other.Name
               && CommandLine == other.CommandLine
               && Children.SequenceEqual(other.Children);
    }
}
