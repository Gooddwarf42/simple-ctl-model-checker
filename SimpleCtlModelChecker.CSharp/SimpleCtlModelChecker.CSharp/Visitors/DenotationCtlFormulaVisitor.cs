using System.Collections.Immutable;
using SimpleCtlModelChecker.CSharp.Extensions;
using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Model;
using SimpleCtlModelChecker.CSharp.Visitors.Abstractions;

namespace SimpleCtlModelChecker.CSharp.Visitors;

public class DenotationCtlFormulaVisitor(KripkeModel model) : CtlFormulaVisitor<ImmutableHashSet<string>>
{
    private readonly KripkeModel _model = model;
    private ImmutableHashSet<string> StateSet => _model.States.Select(s => s.Name).ToImmutableHashSet();


    protected override ImmutableHashSet<string> VisitBottom(BottomCtlFormula bottomCtlFormula)
        => [];

    protected override ImmutableHashSet<string> VisitAtomic(AtomicCtlFormula atomicCtlFormula)
        => _model
            .States
            .Where(s => s.Atoms.Contains(atomicCtlFormula.Atom))
            .Select(s => s.Name)
            .ToImmutableHashSet();

    protected override ImmutableHashSet<string> VisitUnary(UnaryCtlFormula unaryCtlFormula)
    {
        var operandDenotation = Visit(unaryCtlFormula.Operand);

        return unaryCtlFormula.Operator switch
        {
            UnaryOperator.Not => StateSet.SetDifference(operandDenotation).ToImmutableHashSet(),
            UnaryOperator.AG => VisitAG(operandDenotation),
            UnaryOperator.AF => VisitAF(operandDenotation),
            UnaryOperator.EG => VisitEG(operandDenotation),
            UnaryOperator.EF => VisitEF(operandDenotation),
            UnaryOperator.AX => VisitAX(operandDenotation),
            UnaryOperator.EX => VisitEX(operandDenotation),
            _ => throw new ArgumentOutOfRangeException(nameof(unaryCtlFormula), "Operator case not implemented"),
        };
    }

    protected override ImmutableHashSet<string> VisitBinary(BinaryCtlFormula binaryCtlFormula)
    {
        var leftDenotation = Visit(binaryCtlFormula.Left);
        var rightDenotation = Visit(binaryCtlFormula.Right);

        return binaryCtlFormula.Operator switch
        {
            BinaryOperator.And => leftDenotation.Intersect(rightDenotation),
            BinaryOperator.Or => leftDenotation.Union(rightDenotation),
            BinaryOperator.Implies => rightDenotation.Union(StateSet.SetDifference(leftDenotation)), // p->q is the same as !q \/p
            BinaryOperator.AU => VisitAU(leftDenotation, rightDenotation),
            BinaryOperator.EU => VisitEU(leftDenotation, rightDenotation),
            _ => throw new ArgumentOutOfRangeException(nameof(binaryCtlFormula), "Operator case not implemented"),
        };
    }

// For now I write them all. Probably I can just abstract a fixpoint operator or something and
// save a lot of code duplication that way

#region UNARY TEMPORAL OPERATORS

    private ImmutableHashSet<string> VisitAG(ImmutableHashSet<string> operand)
    {
        throw new NotImplementedException();
    }

    private ImmutableHashSet<string> VisitEG(ImmutableHashSet<string> operand)
    {
        throw new NotImplementedException();
    }

    private ImmutableHashSet<string> VisitAF(ImmutableHashSet<string> operand)
    {
        throw new NotImplementedException();
    }

    private ImmutableHashSet<string> VisitEF(ImmutableHashSet<string> operand)
    {
        throw new NotImplementedException();
    }

    private ImmutableHashSet<string> VisitAX(ImmutableHashSet<string> operand)
    {
        throw new NotImplementedException();
    }

    private ImmutableHashSet<string> VisitEX(ImmutableHashSet<string> operand)
    {
        throw new NotImplementedException();
    }

#endregion

#region BINARY TEMPORAL OPERATORS

    private ImmutableHashSet<string> VisitAU(ImmutableHashSet<string> leftDenotation, ImmutableHashSet<string> rightDenotation)
    {
        throw new NotImplementedException();
    }

    private ImmutableHashSet<string> VisitEU(ImmutableHashSet<string> leftDenotation, ImmutableHashSet<string> rightDenotation)
    {
        throw new NotImplementedException();
    }

#endregion
}