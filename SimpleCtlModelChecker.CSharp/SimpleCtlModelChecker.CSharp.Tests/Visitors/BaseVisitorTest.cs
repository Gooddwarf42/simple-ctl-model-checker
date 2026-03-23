using System.Net;
using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Visitors.Abstractions;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.Visitors;

public abstract class BaseVisitorTest
{
    protected static readonly CtlFormula Safety = CtlFormula.Unary
    (
        UnaryOperator.AG,
        CtlFormula.Unary(UnaryOperator.Not, CtlFormula.Atomic("ReactorGoesCritical"))
    );
    
    protected static readonly CtlFormula Persistence = CtlFormula.Unary
    (
        UnaryOperator.AG,
        CtlFormula.Binary
        (
            BinaryOperator.Implies,
            CtlFormula.Atomic("BossIsDead"),
            CtlFormula.Unary
            (
                UnaryOperator.AG,
                CtlFormula.Atomic("BossIsDead")
            )
        )
    );
    
    protected static readonly CtlFormula Recurrence = CtlFormula.Unary
    (
        UnaryOperator.AG,
        CtlFormula.Unary
        (
            UnaryOperator.AF,
            CtlFormula.Atomic("GoodThing")
        )
    );
    
    protected static readonly CtlFormula Response = CtlFormula.Unary
    (
        UnaryOperator.AG,
        CtlFormula.Binary
        (
            BinaryOperator.Implies,
            CtlFormula.Atomic("RequestSent"),
            CtlFormula.Unary
            (
                UnaryOperator.AF,
                CtlFormula.Atomic("ResponseReceived")
            )
        )
    );
    
    // TODO I would like to have an easy way to define here
    // the simple formulae too, so that derived classes just have to
    // say what the expected results are
    protected static void TestVisitor<T>(CtlFormulaVisitor<T> visitor, CtlFormula formula, T expected)
    {
        var result = visitor.Visit(formula);
        Assert.Equal(expected, result);
    }
}