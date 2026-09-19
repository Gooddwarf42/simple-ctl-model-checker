using System.Collections.Immutable;

namespace SimpleCtlModelChecker.CSharp.Model;

public sealed class State(string name, ImmutableHashSet<string> atoms, ImmutableHashSet<string> transitions, bool isInitial = false)
{
    public string Name { get; } = name;
    public bool IsInitial { get; } = isInitial;
    public IReadOnlySet<string> Atoms => atoms;
    public IReadOnlySet<string> Transitions => transitions;
}