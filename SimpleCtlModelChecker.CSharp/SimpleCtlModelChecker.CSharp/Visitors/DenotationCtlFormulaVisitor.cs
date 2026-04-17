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
            UnaryOperator.AG => // AGp <-> p /\ AXAGp
                // we want to compute a maximum fixpoint of p /\ AXp basically
                Fixpoint(operandDenotation, set => set.Intersect(VisitAX(set))),
            UnaryOperator.AF => // AFp <-> p \/ AXAFp 
                // we want to compute a minimum fixpoint of p \/ AXp basically
                Fixpoint(operandDenotation, set => set.Union(VisitAX(set))),
            UnaryOperator.EG => // EGp <-> p /\ EXEGp 
                // we want to compute a maximum fixpoint of p /\ EXp basically
                Fixpoint(operandDenotation, set => set.Intersect(VisitEX(set))),
            UnaryOperator.EF => // EFp <-> p \/ EXEFp 
                // we want to compute a minimum fixpoint of p \/ EXp basically
                Fixpoint(operandDenotation, set => set.Union(VisitEX(set))),
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
            BinaryOperator.AU => // A[pUq] <-> q \/ (p /\ AXA[pUq]) 
                // we want to compute a minimum fixpoint of q \/ (p /\ AXq) basically
                Fixpoint(rightDenotation, set => set.Union(leftDenotation.Intersect(VisitAX(set)))),
            BinaryOperator.EU => // E[pUq] <-> q \/ (p /\ EXE[pUq]) 
                // we want to compute a minimum fixpoint of q \/ (p /\ EXq) basically
                Fixpoint(rightDenotation, set => set.Union(leftDenotation.Intersect(VisitEX(set)))),
            _ => throw new ArgumentOutOfRangeException(nameof(binaryCtlFormula), "Operator case not implemented"),
        };
    }

#region X TEMPORAL OPERATORS

    private ImmutableHashSet<string> VisitAX(ImmutableHashSet<string> operandDenotation)
        => model.States.Where(s => s.Transitions.All(operandDenotation.Contains))
            .Select(s => s.Name)
            .ToImmutableHashSet();

    private ImmutableHashSet<string> VisitEX(ImmutableHashSet<string> operandDenotation)
        => model.States.Where(s => s.Transitions.Any(operandDenotation.Contains))
            .Select(s => s.Name)
            .ToImmutableHashSet();

#endregion

    /// <summary>
    /// This method requires the function to be monotone!
    /// Yes I am too lazy to add a check for that in here...
    /// </summary>
    /// <param name="start"></param>
    /// <param name="monotoneFunction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="CtlModelCheckerException"></exception>
    private ImmutableHashSet<T> Fixpoint<T>(ImmutableHashSet<T> start, Func<ImmutableHashSet<T>, ImmutableHashSet<T>> monotoneFunction)
    {
        var count = 0;
        var setToIterateOn = new HashSet<T>(start).ToImmutableHashSet();
        var elementsCount = setToIterateOn.Count;

        while (count < MaxFixpointIterations)
        {
            setToIterateOn = monotoneFunction(setToIterateOn);

            if (setToIterateOn.Count == elementsCount)
            {
                // fixpoint reached!
                return setToIterateOn;
            }

            elementsCount = setToIterateOn.Count;
            count++;
        }

        throw new CtlModelCheckerException("Max number of fixpoint iterations exceeded");
    }
}
