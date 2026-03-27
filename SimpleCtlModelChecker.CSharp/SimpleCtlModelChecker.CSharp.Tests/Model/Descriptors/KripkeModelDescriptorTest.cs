using JetBrains.Annotations;
using SimpleCtlModelChecker.CSharp.Exceptions;
using SimpleCtlModelChecker.CSharp.Model.Descriptors;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.Model.Descriptors;

[TestSubject(typeof(KripkeModelDescriptor))]
public class KripkeModelDescriptorTest
{
    [Fact]
    public void Should_Throw_When_DuplicateStatusIsPresent()
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
                    Name = "S1",
                    Atoms = ["C"],
                    Transitions = ["S2"],
                },
            ],
        };

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => modelDescriptor.Validate(), ex => ex.InspectForMessage("States with duplicate name"));
    }
    
    [Fact]
    public void Should_Throw_When_NoInitialStatusIsSpecified()
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

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => modelDescriptor.Validate(), ex => ex.InspectForMessage("at least one initial state should be specified"));
    }
    
    [Fact]
    public void Should_Throw_When_HasInvalidState()
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
                    Atoms = ["F"],
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

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => modelDescriptor.Validate());
    }
}
