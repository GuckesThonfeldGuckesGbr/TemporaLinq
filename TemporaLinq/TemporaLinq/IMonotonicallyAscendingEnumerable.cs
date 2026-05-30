namespace TemporaLinq;

public interface IMonotonicallyAscendingEnumerable<out T> : IEnumerable<T> where T : IComparable<T>
{
    public static IMonotonicallyAscendingEnumerable<T> FromIEnumerable<T>(IEnumerable<T> enumerable)
        where T : IComparable<T> => new MonotonicAscendingEnumerableWrapper<T>(enumerable);
}