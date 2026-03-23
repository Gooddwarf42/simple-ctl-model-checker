using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Visitors.Abstractions;

namespace SimpleCtlModelChecker.CSharp.Visitors;

// Using friendly to type symbols here, in the hope of one day making a parser which is
// an inverse of this
public class StringCtlFormulaVisitor : CtlFormulaVisitor<string>
{
    protected override string VisitBottom(BottomCtlFormula bottomCtlFormula)
        => "_|_";

    protected override string VisitAtomic(AtomicCtlFormula atomicCtlFormula)
        => $"\'{atomicCtlFormula.Atom}'";

    protected override string VisitUnary(UnaryCtlFormula unaryCtlFormula)
    {
        var operand = Visit(unaryCtlFormula.Operand);

        return unaryCtlFormula.Operator switch
        {
            UnaryOperator.Not => $"!{operand}",
            UnaryOperator.AG => $"AG {operand}",
            UnaryOperator.EG => $"EG {operand}",
            UnaryOperator.AF => $"AF {operand}",
            UnaryOperator.EF => $"EF {operand}",
            UnaryOperator.AX => $"AX {operand}",
            UnaryOperator.EX => $"EX {operand}",
            _ => throw new ArgumentOutOfRangeException(nameof(unaryCtlFormula), "Operator case not implemented"),
        };
    }

    protected override string VisitBinary(BinaryCtlFormula binaryCtlFormula)
    {
        var left = Visit(binaryCtlFormula.Left);
        var right = Visit(binaryCtlFormula.Right);

        return binaryCtlFormula.Operator switch
        {
            BinaryOperator.And => $"({left} /\\ {right})",
            BinaryOperator.Or => $"({left} \\/ {right})",
            BinaryOperator.Implies => $"({left} -> {right})",
            BinaryOperator.AU => $"A[{left} U {right}]",
            BinaryOperator.EU => $"E[{left} U {right}]",
            _ => throw new ArgumentOutOfRangeException(nameof(binaryCtlFormula), "Operator case not implemented"),
        };
    }
}