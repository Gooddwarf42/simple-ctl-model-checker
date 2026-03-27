using JetBrains.Annotations;
using SimpleCtlModelChecker.CSharp.Exceptions;
using SimpleCtlModelChecker.CSharp.Model.Builder;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.KripkeModel.Builder;

[TestSubject(typeof(StateBuilder))]
public class StateBuilderTest
{

    [Fact]
    public void Should_Throw_When_AddingInvalidAtom()
    {
        // Arrange
        var modelBuilder = new KripkeModelBuilder();

        modelBuilder
            .HasAtom("A")
            .HasAtom("B")
            .HasAtom("C");

        modelBuilder
            .HasState("S1")
            .SetInitial();

        var stateBuilder = modelBuilder.State("S1");

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => stateBuilder.WithAtom("D"));
    }
    
    
    [Fact]
    public void Should_Throw_When_AddingInvalidTransition()
    {
        // Arrange
        var modelBuilder = new KripkeModelBuilder();

        modelBuilder
            .HasAtom("A")
            .HasAtom("B")
            .HasAtom("C");

        modelBuilder
            .HasState("S1")
            .SetInitial();

        var stateBuilder = modelBuilder.State("S1");

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => stateBuilder.WithTransition("S3"));
    }
}
