using System.Collections.Immutable;

namespace SimpleCtlModelChecker.CSharp.Model;

public sealed class KripkeModel(ImmutableHashSet<string> atoms, ImmutableList<State> states)
{
    public IReadOnlySet<string> Atoms => atoms;
    public IReadOnlyList<State> States => states;
    public IEnumerable<State> InitialStates => states.Where(s => s.IsInitial);
}
