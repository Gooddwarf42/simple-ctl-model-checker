using System.Collections.Immutable;

namespace SimpleCtlModelChecker.CSharp.KripkeModel;

public sealed class KripkeModel(ImmutableHashSet<string> atoms, ImmutableList<State> states)
{
    public IEnumerable<string> Atoms => atoms.AsEnumerable();
    public IEnumerable<State> States => states.AsEnumerable();
}
