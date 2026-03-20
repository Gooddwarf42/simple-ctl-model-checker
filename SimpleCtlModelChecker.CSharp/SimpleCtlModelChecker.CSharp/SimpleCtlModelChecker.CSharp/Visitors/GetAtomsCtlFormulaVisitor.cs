using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Visitors.Abstractions;

namespace SimpleCtlModelChecker.CSharp.Visitors;

public class GetAtomsCtlFormulaVisitor : CtlFormulaVisitor<HashSet<string>>
{
    protected override HashSet<string> VisitBottom(BottomCtlFormula bottomCtlFormula)
        => [];

    protected override HashSet<string> VisitAtomic(AtomicCtlFormula atomicCtlFormula)
        => [atomicCtlFormula.Atom];

    protected override HashSet<string> VisitUnary(UnaryCtlFormula unaryCtlFormula)
        => Visit(unaryCtlFormula.Operand);

    protected override HashSet<string> VisitBinary(BinaryCtlFormula binaryCtlFormula)
    {
        var leftAtoms = Visit(binaryCtlFormula.Left);
        var rightAtoms = Visit(binaryCtlFormula.Right);

        return leftAtoms.Union(rightAtoms).ToHashSet();
    }
}