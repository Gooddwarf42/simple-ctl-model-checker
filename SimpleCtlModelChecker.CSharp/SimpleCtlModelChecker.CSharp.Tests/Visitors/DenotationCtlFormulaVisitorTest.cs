using FluentAssertions;
using JetBrains.Annotations;
using SimpleCtlModelChecker.CSharp.Formulae;
using SimpleCtlModelChecker.CSharp.Model.Builder;
using SimpleCtlModelChecker.CSharp.Visitors;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.Visitors;

[TestSubject(typeof(DenotationCtlFormulaVisitor))]
public class DenotationCtlFormulaVisitorTest
{
    [Fact]
    public void DenotationsAreComputedCorrectly1()
    {
        // Arrange
        var forumla = CtlFormula.Response(CtlFormula.Atomic("p"), CtlFormula.Atomic("q"));

        var model = BuildTestModel();
        var visitor = new DenotationCtlFormulaVisitor(model);

        // Act
        var result = visitor.Visit(forumla);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void DenotationsAreComputedCorrectly2()
    {
        // Arrange
        var forumla = CtlFormula.Unary(UnaryOperator.AF, CtlFormula.Atomic("q"));

        var model = BuildTestModel();
        var visitor = new DenotationCtlFormulaVisitor(model);

        // Act
        var result = visitor.Visit(forumla);

        // Assert
        result.Should().BeEquivalentTo("1", "2");
    }
    
    [Fact]
    public void DenotationsAreComputedCorrectly3()
    {
        // Arrange
        var forumla = CtlFormula.Binary(BinaryOperator.Implies, CtlFormula.Atomic("p"), CtlFormula.Unary(UnaryOperator.AF, CtlFormula.Atomic("q")));

        var model = BuildTestModel();
        var visitor = new DenotationCtlFormulaVisitor(model);

        // Act
        var result = visitor.Visit(forumla);

        // Assert
        result.Should().BeEquivalentTo("1", "2", "4");
    }


    private static CSharp.Model.KripkeModel BuildTestModel()
    {
        var modelBuilder = new KripkeModelBuilder();

        modelBuilder
            .HasAtom("p")
            .HasAtom("q");

        modelBuilder
            .HasState("1")
            .WithAtom("p")
            .SetInitial();

        modelBuilder
            .HasState("2")
            .WithAtom("q");

        modelBuilder
            .HasState("3")
            .WithAtom("p");

        modelBuilder
            .HasState("4")
            .WithTransition("3")
            .WithTransition("2");

        modelBuilder
            .State("3")
            .WithTransition("4");

        modelBuilder
            .State("2")
            .WithTransition("3");

        modelBuilder
            .State("1")
            .WithTransition("2");

        return modelBuilder.BuildModel();
    }
}