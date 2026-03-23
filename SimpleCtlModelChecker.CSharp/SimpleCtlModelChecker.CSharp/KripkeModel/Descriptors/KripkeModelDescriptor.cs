using SimpleCtlModelChecker.CSharp.Extensions;

namespace SimpleCtlModelChecker.CSharp.KripkeModel.Descriptors;

/// <summary>
/// This class is used for serialization and deserialization
/// of Kripke Models, so that storage is easily doable,
/// while retaining a strict readonly mature of the
/// kripke model itself when it gets created
/// </summary>
public sealed class KripkeModelDescriptor
{
    public HashSet<string> Atoms { get; set; } = [];
    public List<StateDescriptor> States { get; set; } = [];

    public void Validate()
    {
        var atomsInStates = States.SelectMany(s => s.Atoms).ToHashSet();

        var invalidAtoms = atomsInStates.SetDifference(Atoms);

        if (invalidAtoms.Count > 0)
        {
            // TODO exception
            throw new Exception($"Some states contain invalid atoms: [{string.Join(", ", invalidAtoms)}]");
        }

        var statesWithoutTransitions = States.Where(s => s.Transitions.Count == 0).ToArray();

        if (statesWithoutTransitions.Length > 0)
        {
            // TODO exception
            throw new Exception($"Some states don't have any transition: [{string.Join(", ", statesWithoutTransitions.Select(s => s.Name))}]");
        }

        var statesInTransitions = States.SelectMany(s => s.Transitions).ToHashSet();
        var validStates = States.Select(s => s.Name).ToHashSet();

        if (validStates.Count < States.Count)
        {
            var statesWithDuplicateName = States
                .GroupBy(s => s.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            // TODO exception
            throw new Exception($"States with duplicate name: [{string.Join(", ", statesWithDuplicateName)}]");
        }

        var invalidStatesInTransitions = statesInTransitions.SetDifference(validStates);

        if (invalidStatesInTransitions.Count > 0)
        {
            // TODO exception
            throw new Exception($"Some states contain invalid transitions to the following nonexisting states: [{string.Join(", ", invalidStatesInTransitions)}]");
        }
    }
}