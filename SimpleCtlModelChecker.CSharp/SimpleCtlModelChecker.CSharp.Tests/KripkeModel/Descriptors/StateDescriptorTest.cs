using System.Collections.Generic;
using JetBrains.Annotations;
using SimpleCtlModelChecker.CSharp.Exceptions;
using SimpleCtlModelChecker.CSharp.KripkeModel.Descriptors;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.KripkeModel.Descriptors;

[TestSubject(typeof(StateDescriptor))]
public class StateDescriptorTest
{
    [Fact]
    public void Should_Throw_When_NameIsInvalid()
    {
        var atoms = new HashSet<string> {"A", "B", "C"};
        var states = new HashSet<string> {"S1", "S2"};

        var stateDescriptor = new StateDescriptor
        {
            Name = "S3",
            IsInitial = true,
            Atoms = ["A", "B"],
            Transitions = ["S2"],
        };

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => stateDescriptor.Validate(atoms, states), ex => ex.InspectForMessage("not present in the set of states provided"));
    }
    
    [Fact]
    public void Should_Throw_When_NoTransitionSpecified()
    {
        var atoms = new HashSet<string> {"A", "B", "C"};
        var states = new HashSet<string> {"S1", "S2"};

        var stateDescriptor = new StateDescriptor
        {
            Name = "S1",
            IsInitial = true,
            Atoms = ["A", "B"],
            Transitions = [],
        };

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => stateDescriptor.Validate(atoms, states), ex => ex.InspectForMessage("doesn't have any transition"));
    }
    
    [Fact]
    public void Should_Throw_When_HasInvalidAtoms()
    {
        var atoms = new HashSet<string> {"A", "B", "C"};
        var states = new HashSet<string> {"S1", "S2"};

        var stateDescriptor = new StateDescriptor
        {
            Name = "S1",
            IsInitial = true,
            Atoms = ["A", "B", "F"],
            Transitions = ["S2"],
        };

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => stateDescriptor.Validate(atoms, states), ex => ex.InspectForMessage("contains invalid atoms"));
    }
    
        
    [Fact]
    public void Should_Throw_When_HasInvalidTransitions()
    {
        var atoms = new HashSet<string> {"A", "B", "C"};
        var states = new HashSet<string> {"S1", "S2"};

        var stateDescriptor = new StateDescriptor
        {
            Name = "S1",
            IsInitial = true,
            Atoms = ["A", "B"],
            Transitions = ["S2", "S3"],
        };

        // Act - Assert
        Assert.Throws<CtlModelCheckerException>(() => stateDescriptor.Validate(atoms, states), ex => ex.InspectForMessage("contains invalid transitions to the following nonexisting states"));
    }
}
