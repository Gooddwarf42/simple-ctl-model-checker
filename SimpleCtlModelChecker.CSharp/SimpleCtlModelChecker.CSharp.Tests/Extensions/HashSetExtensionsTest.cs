using System.Collections.Generic;
using JetBrains.Annotations;
using SimpleCtlModelChecker.CSharp.Extensions;
using Xunit;

namespace SimpleCtlModelChecker.CSharp.Tests.Extensions;

[TestSubject(typeof(HashSetExtensions))]
public class HashSetExtensionsTest
{
    [Fact]
    public void SetDifferenceWorks()
    {
        // Arrange
        var originalA = new HashSet<int> {1, 2, 3, 4}; 
        var a = new HashSet<int> {1, 2, 3, 4};
        var b = new[] {2, 4, 6};

        var expected = new HashSet<int> {3, 1};

        // Act
        var result = a.SetDifference(b);

        // Assert
        Assert.True(expected.SetEquals(result)); // Result is what I expect
        Assert.True(originalA.SetEquals(a)); // original set has not been modified
        Assert.False(ReferenceEquals(expected, a)); // expected is not a reference to the original
    }
}
