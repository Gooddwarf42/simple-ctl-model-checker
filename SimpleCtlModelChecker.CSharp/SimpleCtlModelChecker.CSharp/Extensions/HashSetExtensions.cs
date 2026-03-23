using System.Diagnostics.Contracts;

namespace SimpleCtlModelChecker.CSharp.Extensions;

internal static class HashSetExtensions
{
    [Pure]
    public static ISet<T> SetDifference<T>(this ISet<T> source, IEnumerable<T> other)
    {
        var result = new HashSet<T>(source);
        result.ExceptWith(other);

        return result;
    }
}