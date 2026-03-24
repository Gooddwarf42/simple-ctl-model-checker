namespace SimpleCtlModelChecker.CSharp.Formulae;

public abstract class CtlFormula
{
    public static BottomCtlFormula Bottom() => new();
    public static AtomicCtlFormula Atomic(string atom) => new(atom);
    public static UnaryCtlFormula Unary(UnaryOperator @operator, CtlFormula operand) => new(@operator, operand);
    public static BinaryCtlFormula Binary(BinaryOperator @operator, CtlFormula left, CtlFormula right) => new(@operator, left, right);

#region COMMON CTL FORMULAE

    /// <summary>
    /// Bad things can never happen.
    /// AG(!badThing)
    /// </summary>
    /// <param name="badThing"></param>
    /// <returns></returns>
    public static CtlFormula Safety(CtlFormula badThing) => Unary(UnaryOperator.AG, Unary(UnaryOperator.Not, badThing));

    /// <summary>
    /// If something happens, it will always stay that way
    /// AG(stableThing -> AG stableThing)
    /// </summary>
    /// <param name="stableThing"></param>
    /// <returns></returns>
    public static CtlFormula Persistence(CtlFormula stableThing) => Unary(UnaryOperator.AG, Binary(BinaryOperator.Implies, stableThing, Unary(UnaryOperator.AG, stableThing)));

    /// <summary>
    /// Something happens infinitely often
    /// AGAF(recurringThing)
    /// </summary>
    /// <param name="recurringThing"></param>
    /// <returns></returns>
    public static CtlFormula Recurrence(CtlFormula recurringThing) => Unary(UnaryOperator.AG, Unary(UnaryOperator.AF, recurringThing));

    /// <summary>
    /// Whenever something happens, something else
    /// will eventually happen in response
    /// AG(trigger -> AF(consequence))
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="consequence"></param>
    /// <returns></returns>
    public static CtlFormula Response(CtlFormula trigger, CtlFormula consequence) => Unary(UnaryOperator.AG, Binary(BinaryOperator.Implies, trigger, Unary(UnaryOperator.AF, consequence)));

#endregion
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
