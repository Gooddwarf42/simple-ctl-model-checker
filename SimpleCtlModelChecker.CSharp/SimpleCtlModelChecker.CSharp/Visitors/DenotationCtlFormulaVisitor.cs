using System.Collections.Immutable;
using SimpleCtlModelChecker.CSharp.Exceptions;
using SimpleCtlModelChecker.CSharp.Extensions;
using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Model;
using SimpleCtlModelChecker.CSharp.Visitors.Abstractions;

namespace SimpleCtlModelChecker.CSharp.Visitors;

public class DenotationCtlFormulaVisitor(KripkeModel model) : CtlFormulaVisitor<ImmutableHashSet<string>>
{
    private const int MaxFixpointIterations = 10000;
    private ImmutableHashSet<string> StateSet => model.States.Select(s => s.Name).ToImmutableHashSet();


    protected override ImmutableHashSet<string> VisitBottom(BottomCtlFormula bottomCtlFormula)
        => [];

    protected override ImmutableHashSet<string> VisitAtomic(AtomicCtlFormula atomicCtlFormula)
        => model
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

    private ImmutableHashSet<string> VisitAG(ImmutableHashSet<string> operandDenotation)
    {
        // AGp <-> p /\ AXAGp 
        // we want to compute a maximum fixpoint of p /\ AXp basically
        var count = 0;
        var setToIterateOn = new HashSet<string>(operandDenotation); // we can keep this mutable to save allocations, if we care of that
        var elementsCount = setToIterateOn.Count;

        while (count < MaxFixpointIterations)
        {
            setToIterateOn.IntersectWith(VisitAX(setToIterateOn.ToImmutableHashSet())); // so much for saving the allocations lmao

            if (setToIterateOn.Count == elementsCount)
            {
                // fixpoint reached!
                return setToIterateOn.ToImmutableHashSet();
            }

            elementsCount = setToIterateOn.Count;
            count++;
        }

        throw new CtlModelCheckerException("Max number of fixpoint iterations exceeded");
    }

    private ImmutableHashSet<string> VisitEG(ImmutableHashSet<string> operandDenotation)
    {
        // EGp <-> p /\ EXEGp 
        // we want to compute a maximum fixpoint of p /\ EXp basically
        var count = 0;
        var setToIterateOn = new HashSet<string>(operandDenotation); // we can keep this mutable to save allocations, if we care of that
        var elementsCount = setToIterateOn.Count;

        while (count < MaxFixpointIterations)
        {
            setToIterateOn.IntersectWith(VisitEX(setToIterateOn.ToImmutableHashSet())); // so much for saving the allocations lmao

            if (setToIterateOn.Count == elementsCount)
            {
                // fixpoint reached!
                return setToIterateOn.ToImmutableHashSet();
            }

            elementsCount = setToIterateOn.Count;
            count++;
        }

        throw new CtlModelCheckerException("Max number of fixpoint iterations exceeded");
    }

    private ImmutableHashSet<string> VisitAF(ImmutableHashSet<string> operandDenotation)
    {
        // AFp <-> p \/ AXAFp 
        // we want to compute a minimum fixpoint of p \/ AXp basically
        var count = 0;
        var setToIterateOn = new HashSet<string>(operandDenotation); // we can keep this mutable to save allocations, if we care of that
        var elementsCount = setToIterateOn.Count;

        while (count < MaxFixpointIterations)
        {
            setToIterateOn.UnionWith(VisitAX(setToIterateOn.ToImmutableHashSet())); // so much for saving the allocations lmao

            if (setToIterateOn.Count == elementsCount)
            {
                // fixpoint reached!
                return setToIterateOn.ToImmutableHashSet();
            }

            elementsCount = setToIterateOn.Count;
            count++;
        }

        throw new CtlModelCheckerException("Max number of fixpoint iterations exceeded");
    }

    private ImmutableHashSet<string> VisitEF(ImmutableHashSet<string> operandDenotation)
    {
        // EFp <-> p \/ EXEFp 
        // we want to compute a minimum fixpoint of p \/ EXp basically
        var count = 0;
        var setToIterateOn = new HashSet<string>(operandDenotation); // we can keep this mutable to save allocations, if we care of that
        var elementsCount = setToIterateOn.Count;

        while (count < MaxFixpointIterations)
        {
            setToIterateOn.UnionWith(VisitEX(setToIterateOn.ToImmutableHashSet())); // so much for saving the allocations lmao

            if (setToIterateOn.Count == elementsCount)
            {
                // fixpoint reached!
                return setToIterateOn.ToImmutableHashSet();
            }

            elementsCount = setToIterateOn.Count;
            count++;
        }

        throw new CtlModelCheckerException("Max number of fixpoint iterations exceeded");
    }

    private ImmutableHashSet<string> VisitAX(ImmutableHashSet<string> operandDenotation)
        => model.States.Where(s => s.Transitions.All(operandDenotation.Contains))
            .Select(s => s.Name)
            .ToImmutableHashSet();

    private ImmutableHashSet<string> VisitEX(ImmutableHashSet<string> operandDenotation)
        => model.States.Where(s => s.Transitions.Any(operandDenotation.Contains))
            .Select(s => s.Name)
            .ToImmutableHashSet();

#endregion

#region BINARY TEMPORAL OPERATORS

    private ImmutableHashSet<string> VisitAU(ImmutableHashSet<string> leftDenotation, ImmutableHashSet<string> rightDenotation)
    {
        // A[pUq] <-> q \/ (p /\ AXA[pUq]) 
        // we want to compute a minimum fixpoint of q \/ (p /\ AXq) basically
        var count = 0;
        var setToIterateOn = new HashSet<string>(rightDenotation); // we can keep this mutable to save allocations, if we care of that
        var elementsCount = setToIterateOn.Count;

        while (count < MaxFixpointIterations)
        {
            setToIterateOn.UnionWith(leftDenotation.Intersect(VisitAX(setToIterateOn.ToImmutableHashSet()))); // so much for saving the allocations lmao

            if (setToIterateOn.Count == elementsCount)
            {
                // fixpoint reached!
                return setToIterateOn.ToImmutableHashSet();
            }

            elementsCount = setToIterateOn.Count;
            count++;
        }

        throw new CtlModelCheckerException("Max number of fixpoint iterations exceeded");
    }

    private ImmutableHashSet<string> VisitEU(ImmutableHashSet<string> leftDenotation, ImmutableHashSet<string> rightDenotation)
    {
        // E[pUq] <-> q \/ (p /\ EXE[pUq]) 
        // we want to compute a minimum fixpoint of q \/ (p /\ EXq) basically
        var count = 0;
        var setToIterateOn = new HashSet<string>(rightDenotation); // we can keep this mutable to save allocations, if we care of that
        var elementsCount = setToIterateOn.Count;

        while (count < MaxFixpointIterations)
        {
            setToIterateOn.UnionWith(leftDenotation.Intersect(VisitEX(setToIterateOn.ToImmutableHashSet()))); // so much for saving the allocations lmao

            if (setToIterateOn.Count == elementsCount)
            {
                // fixpoint reached!
                return setToIterateOn.ToImmutableHashSet();
            }

            elementsCount = setToIterateOn.Count;
            count++;
        }

        throw new CtlModelCheckerException("Max number of fixpoint iterations exceeded");
    }

#endregion
}