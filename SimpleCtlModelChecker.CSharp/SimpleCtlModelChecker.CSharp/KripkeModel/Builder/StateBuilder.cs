using System.Collections.Immutable;

namespace SimpleCtlModelChecker.CSharp.KripkeModel.Builder;

public class StateBuilder
{
    private readonly KripkeModelBuilder _modelBuilder;
    private readonly string _name;
    private bool _isInitial;
    private readonly HashSet<string> _atoms = [];
    private readonly HashSet<string> _transitions = [];

    internal StateBuilder(string name, KripkeModelBuilder modelBuilder)
    {
        _name = name;
        _modelBuilder = modelBuilder;
    }

    internal StateBuilder HasAtom(string name)
    {
        if (!_modelBuilder.Atoms.Contains(name))
        {
            throw new Exception($"Atom {name} is not a valid atom for the {nameof(KripkeModelBuilder)}");
        }

        // ReSharper disable once CanSimplifySetAddingWithSingleCall
        if (_atoms.Contains(name))
        {
            throw new Exception($"Atom {name} already present in the {nameof(StateBuilder)} for state {_name}");
        }

        _atoms.Add(name);

        return this;
    }

    internal StateBuilder HasTransition(string name)
    {
        if (!_modelBuilder.States.Contains(name))
        {
            throw new Exception($"State {name} is not a valid state for the {nameof(KripkeModelBuilder)}");
        }

        // ReSharper disable once CanSimplifySetAddingWithSingleCall
        if (_transitions.Contains(name))
        {
            throw new Exception($"Transition to state {name} already present in the {nameof(StateBuilder)} for state {_name}");
        }

        _transitions.Add(name);

        return this;
    }

    internal StateBuilder IsInitial()
    {
        _isInitial = true;

        return this;
    }

    internal State Build()
    {
        var atoms = _atoms.ToImmutableHashSet();
        var transitions = _transitions.ToImmutableHashSet();

        return new State(_name, atoms, transitions, _isInitial);
    }
}