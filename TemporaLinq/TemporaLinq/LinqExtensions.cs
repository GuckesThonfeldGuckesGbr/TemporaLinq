namespace TemporaLinq;

public static class LinqExtensions
{
    public static IMonotonicallyAscendingEnumerable<T> AsMonotonicallyAscendingEnumerable<T>(
        this IEnumerable<T> enumerable) where T : IComparable<T>
        => new MonotonicAscendingEnumerableWrapper<T>(enumerable);
}
