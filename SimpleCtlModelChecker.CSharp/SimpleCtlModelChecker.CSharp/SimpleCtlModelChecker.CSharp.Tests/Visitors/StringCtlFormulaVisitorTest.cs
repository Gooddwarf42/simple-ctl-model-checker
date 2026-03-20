using JetBrains.Annotations;
using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Visitors;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.Visitors;

[TestSubject(typeof(StringCtlFormulaVisitor))]
public class StringCtlFormulaVisitorTest : BaseVisitorTest
{
    [Fact]
    public void BottomWorks()
    {
        // Arrange
        var formula = CtlFormula.Bottom();
        var visitor = new StringCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal("_|_", result);
    }

    [Fact]
    public void AtomicWorks()
    {
        // Arrange
        var formula = CtlFormula.Atomic("ReactorIsSafe");
        var visitor = new StringCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal("'ReactorIsSafe'", result);
    }

    [Theory]
    [InlineData(UnaryOperator.Not, "p", "!'p'")]
    [InlineData(UnaryOperator.AG, "p", "AG 'p'")]
    [InlineData(UnaryOperator.EG, "p", "EG 'p'")]
    [InlineData(UnaryOperator.AF, "p", "AF 'p'")]
    [InlineData(UnaryOperator.EF, "p", "EF 'p'")]
    [InlineData(UnaryOperator.AX, "p", "AX 'p'")]
    [InlineData(UnaryOperator.EX, "p", "EX 'p'")]
    public void UnaryWorks(UnaryOperator @operator, string atom, string expected)
    {
        var formula = CtlFormula.Unary(@operator, CtlFormula.Atomic(atom));
        var visitor = new StringCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal(expected, result);
    }


    [Theory]
    [InlineData(BinaryOperator.And, "p", "q", "('p' /\\ 'q')")]
    [InlineData(BinaryOperator.Or, "p", "q", "('p' \\/ 'q')")]
    [InlineData(BinaryOperator.Implies, "p", "q", "('p' -> 'q')")]
    [InlineData(BinaryOperator.AU, "p", "q", "A['p' U 'q']")]
    [InlineData(BinaryOperator.EU, "p", "q", "E['p' U 'q']")]
    public void BinaryWorks(BinaryOperator @operator, string leftAtom, string rightAtom, string expected)
    {
        var formula = CtlFormula.Binary(@operator, CtlFormula.Atomic(leftAtom), CtlFormula.Atomic(rightAtom));
        var visitor = new StringCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void SafetyWorks()
    {
        TestVisitor(new StringCtlFormulaVisitor(), Safety, "AG !'ReactorGoesCritical'");
    }

    [Fact]
    public void PersistenceWorks()
    {
        TestVisitor(new StringCtlFormulaVisitor(), Persistence, "AG ('BossIsDead' -> AG 'BossIsDead')");
    }

    [Fact]
    public void RecurrenceWorks()
    {
        TestVisitor(new StringCtlFormulaVisitor(), Recurrence, "AG AF 'GoodThing'");
    }

    [Fact]
    public void ResponseWorks()
    {
        TestVisitor(new StringCtlFormulaVisitor(), Response, "AG ('RequestSent' -> AF 'ResponseReceived')");
    }
}