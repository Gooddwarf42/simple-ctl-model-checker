using System.Collections.Immutable;
using SimpleCtlModelChecker.CSharp.KripkeModel.Descriptors;

namespace SimpleCtlModelChecker.CSharp.KripkeModel.Builder;

public sealed class KripkeModelBuilder
{
    private readonly HashSet<string> _atoms = [];
    private readonly Dictionary<string, StateBuilder> _states = [];
    internal IReadOnlySet<string> Atoms => _atoms;
    internal IReadOnlyCollection<string> States => _states.Keys;
    
    public KripkeModelBuilder() { }

    public KripkeModelBuilder(KripkeModelDescriptor descriptor)
    {
        descriptor.Validate();

        foreach (var atom in descriptor.Atoms)
        {
            HasAtom(atom);
        }

        foreach (var state in descriptor.States)
        {
            HasState(state.Name);
        }

        foreach (var state in descriptor.States)
        {
            var stateBuilder = State(state.Name);

            if (state.IsInitial)
            {
                stateBuilder.IsInitial();
            }

            foreach (var transitionState in state.Transitions)
            {
                stateBuilder.HasTransition(transitionState);
            }
        }
    }

    public StateBuilder State(string name) => _states.TryGetValue(name, out var stateBuilder)
        ? stateBuilder
        : throw new Exception($"State {name} not found in the {nameof(KripkeModelBuilder)}");

    public StateBuilder HasState(string name)
    {
        if (_states.ContainsKey(name))
        {
            throw new Exception($"State {name} already present in the {nameof(KripkeModelBuilder)}");
        }

        var stateBuilder = new StateBuilder(name, this);
        _states.Add(name, stateBuilder);

        return stateBuilder;
    }

    public KripkeModelBuilder HasAtom(string name)
    {
        // ReSharper disable once CanSimplifySetAddingWithSingleCall
        if (_atoms.Contains(name))
        {
            throw new Exception($"Atom {name} already present in the {nameof(KripkeModelBuilder)}");
        }

        _atoms.Add(name);

        return this;
    }

    public KripkeModel BuildModel()
    {
        // TODO validate (anche se in teoria i metodi che fanno buildare dovrebbero essere
        // abbastanza validevoli di loro)
        // Qualcosa bisogna comunque validare, tipo che tutti gli stati abbiano almeno una transazione...
        var atoms = _atoms.ToImmutableHashSet();
        var states = _states.Values.Select(builder => builder.Build()).ToImmutableList();

        return new KripkeModel(atoms, states);
    }
}