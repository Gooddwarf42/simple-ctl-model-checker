using JetBrains.Annotations;
using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Visitors;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.Visitors;

[TestSubject(typeof(StringCtlFormulaVisitor))]
public class StringCtlFormulaVisitorTest
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
        // Arrange
        var formula = CtlFormula.Unary
        (
            UnaryOperator.AG,
            CtlFormula.Unary(UnaryOperator.Not, CtlFormula.Atomic("ReactorGoesCritical"))
        );
        var visitor = new StringCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal("AG !'ReactorGoesCritical'", result);
    }

    [Fact]
    public void PersistenceWorks()
    {
        // Arrange
        var formula = CtlFormula.Unary
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
        var visitor = new StringCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal("AG ('BossIsDead' -> AG 'BossIsDead')", result);
    }

    [Fact]
    public void RecurrenceWorks()
    {
        // Arrange
        var formula = CtlFormula.Unary
        (
            UnaryOperator.AG,
            CtlFormula.Unary
            (
                UnaryOperator.AF,
                CtlFormula.Atomic("GoodThing")
            )
        );
        var visitor = new StringCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal("AG AF 'GoodThing'", result);
    }
    
    [Fact]
    public void ResponseWorks()
    {
        // Arrange
        var formula = CtlFormula.Unary
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
        var visitor = new StringCtlFormulaVisitor();

        // Act
        var result = visitor.Visit(formula);

        // Assert
        Assert.Equal("AG ('RequestSent' -> AF 'ResponseReceived')", result);
    }
}