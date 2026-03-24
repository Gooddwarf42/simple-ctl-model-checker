using SimpleCtlModelChecker.CSharp.Exceptions;
using SimpleCtlModelChecker.CSharp.Extensions;

namespace SimpleCtlModelChecker.CSharp.KripkeModel.Descriptors;

public sealed class StateDescriptor
{
    public string Name { get; set; } = Guid.NewGuid().ToString();
    public bool IsInitial { get; set; }
    public HashSet<string> Atoms { get; set; } = [];
    public HashSet<string> Transitions { get; set; } = [];

    public void Validate(IReadOnlySet<string> modelAtoms, IReadOnlySet<string> modelStates)
    {
        if (Transitions.Count == 0)
        {
            throw new CtlModelCheckerException($"{nameof(StateDescriptor)}.{nameof(Validate)}: State {Name} doesn't have any transition");
        }

        var invalidAtoms = Atoms.SetDifference(modelAtoms);

        if (invalidAtoms.Count > 0)
        {
            throw new CtlModelCheckerException($"{nameof(StateDescriptor)}.{nameof(Validate)}: State {Name} contains invalid atoms: [{string.Join(", ", invalidAtoms)}]");
        }

        var invalidStatesInTransitions = Transitions.SetDifference(modelStates);

        if (invalidStatesInTransitions.Count > 0)
        {
            throw new CtlModelCheckerException($"{nameof(StateDescriptor)}.{nameof(Validate)}: State {Name} contains invalid transitions to the following nonexisting states: [{string.Join(", ", invalidStatesInTransitions)}]");
        }
    }
}
