using SimpleCtlModelChecker.CSharp.Formulae;

namespace SimpleCtlModelChecker.CSharp.Visitors.Abstractions;

public abstract class CtlFormulaVisitor<TResult>
{
    public TResult Visit(CtlFormula formula) => formula switch
    {
        BottomCtlFormula bottomCtlFormula => VisitBottom(bottomCtlFormula),
        AtomicCtlFormula atomicCtlFormula => VisitAtomic(atomicCtlFormula),
        UnaryCtlFormula unaryCtlFormula => VisitUnary(unaryCtlFormula),
        BinaryCtlFormula binaryCtlFormula => VisitBinary(binaryCtlFormula),
        _ => throw new ArgumentOutOfRangeException(nameof(formula), "Case not implemented"),
    };

    protected abstract TResult VisitBottom(BottomCtlFormula bottomCtlFormula);
    protected abstract TResult VisitAtomic(AtomicCtlFormula atomicCtlFormula);

    protected abstract TResult VisitUnary(UnaryCtlFormula unaryCtlFormula);

    protected abstract TResult VisitBinary(BinaryCtlFormula binaryCtlFormula);
}