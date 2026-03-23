namespace SimpleCtlModelChecker.CSharp.KripkeModel.Descriptors;

public sealed class StateDescriptor
{
    public string Name { get; set; } = Guid.NewGuid().ToString();
    public bool IsInitial { get; set; }
    public HashSet<string> Atoms { get; set; } = [];
    public HashSet<string> Transitions { get; set; } = [];
}