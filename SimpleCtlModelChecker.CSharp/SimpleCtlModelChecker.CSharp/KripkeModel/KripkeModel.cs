using System.Collections.Immutable;

namespace SimpleCtlModelChecker.CSharp.KripkeModel;

public sealed class KripkeModel(ImmutableHashSet<string> atoms, ImmutableList<State> states)
{
    public IReadOnlySet<string> Atoms => atoms;
    public IReadOnlyList<State> States => states;
}
