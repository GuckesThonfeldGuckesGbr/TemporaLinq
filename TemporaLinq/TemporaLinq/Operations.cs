namespace de.baggerbagger.TemporaLinq;

public static class Operations
{
    /// <summary>
    /// Returns a stream of dates that contains all dates from the given streams in ascending order.
    /// </summary>
    /// <param name="dateStreams"></param>
    /// <returns></returns>
    public static IEnumerable<DateOnly> Merge(this IEnumerable<IEnumerable<DateOnly>> dateStreams)
    {
        var heap = new PriorityQueue<IEnumerator<DateOnly>, DateOnly>();

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

    public static IEnumerable<DateOnly> Merge(this IDateEnumerable dates, params IDateEnumerable[] others)
        => Merge(new[] { dates }.Concat(others));
}