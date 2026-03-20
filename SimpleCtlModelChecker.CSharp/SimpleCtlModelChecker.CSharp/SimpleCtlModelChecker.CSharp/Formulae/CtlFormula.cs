namespace SimpleCtlModelChecker.CSharp.Formulae;

public abstract class CtlFormula
{
    public static BottomCtlFormula Bottom() => new();
    public static AtomicCtlFormula Atomic(string atom) => new(atom);
    public static UnaryCtlFormula Unary(UnaryOperator @operator, CtlFormula operand) => new(@operator, operand);
    public static BinaryCtlFormula Binary(BinaryOperator @operator, CtlFormula left, CtlFormula right) => new(@operator, left, right);
}

public sealed class BottomCtlFormula : CtlFormula;

public sealed class AtomicCtlFormula(string atom) : CtlFormula
{
    public string Atom { get; } = atom;
}

public sealed class UnaryCtlFormula(UnaryOperator @operator, CtlFormula operand) : CtlFormula
{
    public UnaryOperator Operator { get; } = @operator;
    public CtlFormula Operand { get; } = operand;
}

public sealed class BinaryCtlFormula(BinaryOperator @operator, CtlFormula left, CtlFormula right) : CtlFormula
{
    public BinaryOperator Operator { get; } = @operator;
    public CtlFormula Left { get; } = left;
    public CtlFormula Right { get; } = right;
}