using System.Collections.Generic;
using JetBrains.Annotations;
using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Visitors;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.Visitors;

[TestSubject(typeof(GetAtomsCtlFormulaVisitor))]
public class GetAtomsCtlFormulaVisitorTest : BaseVisitorTest
{
    [Fact]
    public void BottomWorks()
    {
        // Arrange
        var formula = CtlFormula.Bottom();
        var visitor = new GetAtomsCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal([], result);
    }

    [Fact]
    public void AtomicWorks()
    {
        // Arrange
        var formula = CtlFormula.Atomic("ReactorIsSafe");
        var visitor = new GetAtomsCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal(["ReactorIsSafe"], result);
    }

    [Theory]
    [InlineData(UnaryOperator.Not, "p")]
    [InlineData(UnaryOperator.AG, "p")]
    [InlineData(UnaryOperator.EG, "p")]
    [InlineData(UnaryOperator.AF, "p")]
    [InlineData(UnaryOperator.EF, "p")]
    [InlineData(UnaryOperator.AX, "p")]
    [InlineData(UnaryOperator.EX, "p")]
    public void UnaryWorks(UnaryOperator @operator, string atom)
    {
        var formula = CtlFormula.Unary(@operator, CtlFormula.Atomic(atom));
        var visitor = new GetAtomsCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal(["p"], result);
    }


    [Theory]
    [InlineData(BinaryOperator.And, "p", "q")]
    [InlineData(BinaryOperator.Or, "p", "q")]
    [InlineData(BinaryOperator.Implies, "p", "q")]
    [InlineData(BinaryOperator.AU, "p", "q")]
    [InlineData(BinaryOperator.EU, "p", "q")]
    public void BinaryWorks(BinaryOperator @operator, string leftAtom, string rightAtom)
    {
        var formula = CtlFormula.Binary(@operator, CtlFormula.Atomic(leftAtom), CtlFormula.Atomic(rightAtom));
        var visitor = new GetAtomsCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal(["p", "q"], result);
    }

    [Fact]
    public void SafetyWorks()
    {
        TestVisitor(new GetAtomsCtlFormulaVisitor(), Safety, ["ReactorGoesCritical"]);
    }

    [Fact]
    public void PersistenceWorks()
    {
        TestVisitor(new GetAtomsCtlFormulaVisitor(), Persistence, ["BossIsDead"]);
    }

    [Fact]
    public void RecurrenceWorks()
    {
        TestVisitor(new GetAtomsCtlFormulaVisitor(), Recurrence, ["GoodThing"]);
    }

    [Fact]
    public void ResponseWorks()
    {
        TestVisitor(new GetAtomsCtlFormulaVisitor(), Response, ["RequestSent", "ResponseReceived"]);
    }
}