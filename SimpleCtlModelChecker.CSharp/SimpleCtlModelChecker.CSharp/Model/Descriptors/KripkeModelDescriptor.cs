using SimpleCtlModelChecker.CSharp.Exceptions;

namespace SimpleCtlModelChecker.CSharp.Model.Descriptors;

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
        var stateNames = States.Select(s => s.Name).ToHashSet();

        if (stateNames.Count < States.Count)
        {
            var statesWithDuplicateName = States
                .GroupBy(s => s.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            throw new CtlModelCheckerException($"{nameof(KripkeModelDescriptor)}.{nameof(Validate)}: States with duplicate name: [{string.Join(", ", statesWithDuplicateName)}]");
        }

        var hasInitialState = States.Any(s => s.IsInitial);

        if (!hasInitialState)
        {
            throw new CtlModelCheckerException($"{nameof(KripkeModelDescriptor)}.{nameof(Validate)}: at least one initial state should be specified");
        }

        foreach (var state in States)
        {
            state.Validate(Atoms, States.Select(s => s.Name).ToHashSet());
        }
    }
}
