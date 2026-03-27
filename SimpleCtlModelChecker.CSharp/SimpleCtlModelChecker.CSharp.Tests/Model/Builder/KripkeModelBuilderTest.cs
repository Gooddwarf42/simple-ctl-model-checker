using FluentAssertions;
using JetBrains.Annotations;
using SimpleCtlModelChecker.CSharp.Exceptions;
using SimpleCtlModelChecker.CSharp.Model;
using SimpleCtlModelChecker.CSharp.Model.Builder;
using SimpleCtlModelChecker.CSharp.Model.Descriptors;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.Model.Builder;

[TestSubject(typeof(KripkeModelBuilder))]
public class KripkeModelBuilderTest
{
    [Fact]
    public void BuildModelFromBuilder()
    {
        // Arrange
        var modelBuilder = new KripkeModelBuilder();

        modelBuilder
            .HasAtom("A")
            .HasAtom("B")
            .HasAtom("C");

        modelBuilder
            .HasState("S1")
            .WithAtom("A")
            .WithAtom("B")
            .SetInitial();

        modelBuilder
            .HasState("S2")
            .WithAtom("C");

        modelBuilder
            .State("S1")
            .WithTransition("S2");

        modelBuilder
            .State("S2")
            .WithTransition("S2");

        var expectedStates = new[]
        {
            new State("S1", ["A", "B"], ["S2"], true),
            new State("S2", ["C"], ["S2"]),
        };

        var expectedAtoms = new[] {"A", "B", "C"};

        // Act
        var model = modelBuilder.BuildModel();

        // Assert
        model.States.Should().BeEquivalentTo(expectedStates);
        model.Atoms.Should().BeEquivalentTo(expectedAtoms);
    }

    [Fact]
    public void BuildModelFromDescriptor()
    {
        // Arrange
        var modelDescriptor = new KripkeModelDescriptor
        {
            Atoms = ["A", "B", "C"],
            States =
            [
                new StateDescriptor
                {
                    Name = "S1",
                    IsInitial = true,
                    Atoms = ["A", "B"],
                    Transitions = ["S2"],
                },
                new StateDescriptor
                {
                    Name = "S2",
                    Atoms = ["C"],
                    Transitions = ["S2"],
                },
            ],
        };

        var modelBuilder = new KripkeModelBuilder(modelDescriptor);

        var expectedStates = new[]
        {
            new State("S1", ["A", "B"], ["S2"], true),
            new State("S2", ["C"], ["S2"]),
        };

        var expectedAtoms = new[] {"A", "B", "C"};

        // Act
        var model = modelBuilder.BuildModel();

        // Assert
        model.States.Should().BeEquivalentTo(expectedStates);
        model.Atoms.Should().BeEquivalentTo(expectedAtoms);
    }

    [Fact]
    public void Should_Throw_When_BuildingWithNoInitialState()
    {
        // Arrange
        var modelBuilder = new KripkeModelBuilder();

        modelBuilder
            .HasAtom("A")
            .HasAtom("B")
            .HasAtom("C");

        modelBuilder
            .HasState("S1")
            .WithAtom("A")
            .WithAtom("B");

        modelBuilder
            .HasState("S2")
            .WithAtom("C");

        modelBuilder
            .State("S1")
            .WithTransition("S2");

        modelBuilder
            .State("S2")
            .WithTransition("S2");

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => modelBuilder.BuildModel());
    }

    [Fact]
    public void Should_Throw_When_AddingDuplicateAtom()
    {
        // Arrange
        var modelBuilder = new KripkeModelBuilder();

        modelBuilder
            .HasAtom("A");

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => modelBuilder.HasAtom("A"), ex => ex.InspectForMessage("already present"));
    }

    [Fact]
    public void Should_Throw_When_AddingDuplicateState()
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

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => modelBuilder.HasState("S1"), ex => ex.InspectForMessage("already present"));
    }

    [Fact]
    public void Should_Throw_When_AddingGettingNonExistingState()
    {
        // Arrange
        var modelBuilder = new KripkeModelBuilder();

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => modelBuilder.State("S1"), ex => ex.InspectForMessage("not found"));
    }
}
