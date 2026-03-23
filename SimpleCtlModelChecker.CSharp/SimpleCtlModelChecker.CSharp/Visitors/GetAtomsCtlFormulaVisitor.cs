using System.Collections.Immutable;
using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Visitors.Abstractions;

namespace SimpleCtlModelChecker.CSharp.Visitors;

public class GetAtomsCtlFormulaVisitor : CtlFormulaVisitor<ImmutableHashSet<string>>
{
    protected override ImmutableHashSet<string> VisitBottom(BottomCtlFormula bottomCtlFormula)
        => [];

    protected override ImmutableHashSet<string> VisitAtomic(AtomicCtlFormula atomicCtlFormula)
        => [atomicCtlFormula.Atom];

    protected override ImmutableHashSet<string> VisitUnary(UnaryCtlFormula unaryCtlFormula)
        => Visit(unaryCtlFormula.Operand);

    protected override ImmutableHashSet<string> VisitBinary(BinaryCtlFormula binaryCtlFormula)
    {
        var leftAtoms = Visit(binaryCtlFormula.Left);
        var rightAtoms = Visit(binaryCtlFormula.Right);

        return leftAtoms.Union(rightAtoms);
    }
}