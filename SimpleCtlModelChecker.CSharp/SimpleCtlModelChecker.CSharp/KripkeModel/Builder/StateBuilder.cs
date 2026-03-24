using System.Collections.Immutable;
using SimpleCtlModelChecker.CSharp.Exceptions;
using SimpleCtlModelChecker.CSharp.Extensions;

namespace SimpleCtlModelChecker.CSharp.KripkeModel.Builder;

public class StateBuilder
{
    private readonly KripkeModelBuilder _modelBuilder;
    private readonly string _name;
    private readonly HashSet<string> _atoms = [];
    private readonly HashSet<string> _transitions = [];
    public bool IsInitial { get; private set; }

    internal StateBuilder(string name, KripkeModelBuilder modelBuilder)
    {
        _name = name;
        _modelBuilder = modelBuilder;
    }

    internal StateBuilder HasAtom(string name)
    {
        if (!_modelBuilder.Atoms.Contains(name))
        {
            throw new CtlModelCheckerException($"Atom {name} is not a valid atom for the {nameof(KripkeModelBuilder)}");
        }

        // ReSharper disable once CanSimplifySetAddingWithSingleCall
        if (_atoms.Contains(name))
        {
            throw new CtlModelCheckerException($"Atom {name} already present in the {nameof(StateBuilder)} for state {_name}");
        }

        _atoms.Add(name);

        return this;
    }

    internal StateBuilder HasTransition(string name)
    {
        if (!_modelBuilder.States.Contains(name))
        {
            throw new CtlModelCheckerException($"State {name} is not a valid state for the {nameof(KripkeModelBuilder)}");
        }

        // ReSharper disable once CanSimplifySetAddingWithSingleCall
        if (_transitions.Contains(name))
        {
            throw new CtlModelCheckerException($"Transition to state {name} already present in the {nameof(StateBuilder)} for state {_name}");
        }

        _transitions.Add(name);

        return this;
    }

    internal StateBuilder SetInitial()
    {
        IsInitial = true;

        return this;
    }

    internal State Build()
    {
        var atoms = _atoms.ToImmutableHashSet();
        var transitions = _transitions.ToImmutableHashSet();

        return new State(_name, atoms, transitions, IsInitial);
    }

    internal void Validate()
    {
        if (_transitions.Count == 0)
        {
            throw new CtlModelCheckerException($"{nameof(StateBuilder)}.{nameof(Validate)}: State {_name} doesn't have any transition");
        }

        var invalidAtoms = _atoms.SetDifference(_modelBuilder.Atoms);

        if (invalidAtoms.Count > 0)
        {
            throw new CtlModelCheckerException($"{nameof(StateBuilder)}.{nameof(Validate)}: State {_name} contains invalid atoms: [{string.Join(", ", invalidAtoms)}]");
        }

        var invalidStatesInTransitions = _transitions.SetDifference(_modelBuilder.States);

        if (invalidStatesInTransitions.Count > 0)
        {
            throw new CtlModelCheckerException($"{nameof(StateBuilder)}.{nameof(Validate)}: State {_name} contains invalid transitions to the following nonexisting states: [{string.Join(", ", invalidStatesInTransitions)}]");
        }
    }
}
