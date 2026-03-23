using System.Collections.Immutable;

namespace SimpleCtlModelChecker.CSharp.KripkeModel;

public sealed class State(string name, ImmutableHashSet<string> atoms, ImmutableHashSet<string> transitions, bool isInitial = false)
{
    public string Name { get; } = name;
    public bool IsInitial { get; } = isInitial;
    public IEnumerable<string> Atoms => atoms.AsEnumerable();
    public IEnumerable<string> Transitions => transitions.AsEnumerable();
}
