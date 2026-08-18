namespace TemporaLinq;

public static class Operations
{
    /// <summary>
    /// Returns a stream of values that contains all values from the given streams in ascending order.
    /// </summary>
    public static IEnumerable<T> Merge<T>(IEnumerable<IEnumerable<T>> dateStreams)
    {
        var heap = new PriorityQueue<IEnumerator<T>, T>();

        var nonEmptyEnumerators = dateStreams
            .Select(str => str.GetEnumerator())
            .Where(e => e.MoveNext());

        foreach (var enumerator in nonEmptyEnumerators)
            heap.Enqueue(enumerator, enumerator.Current);

        while (heap.Count > 0)
        {
            var smallestEnum = heap.Dequeue();

            yield return smallestEnum.Current;

            if (smallestEnum.MoveNext())
                heap.Enqueue(smallestEnum, smallestEnum.Current);
            else
                smallestEnum.Dispose();
        }
    }

    public static IMonotonicallyAscendingEnumerable<T> Merge<T>(this IMonotonicallyAscendingEnumerable<T> dates,
        params IMonotonicallyAscendingEnumerable<T>[] others) where T : IComparable<T>
        => Merge(new[] { dates }.Concat(others)).AsMonotonicallyAscendingEnumerable();
}
