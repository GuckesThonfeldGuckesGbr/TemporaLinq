namespace de.baggerbagger.TemporaLinq;

public static class Operations
{
    /// <summary>
    /// Returns a stream of dates that contains all dates from the given streams in ascending order.
    /// </summary>
    /// <param name="dateStreams"></param>
    /// <returns></returns>
    public static IEnumerable<DateOnly> Merge(IEnumerable<IEnumerable<DateOnly>> dateStreams)
    {
        // iterate over all dateStreams and yield the lowest date, increment that enumerator then

        var enumerators = dateStreams
            .Select(stream => stream.GetEnumerator())
            .Where(enumerator => enumerator.MoveNext())
            .ToList();

        var nextValues = new DateOnly[enumerators.Count];

        for (var i = 0; i < enumerators.Count; i++)
        {
            nextValues[i] = enumerators[i].Current;
        }

        while (nextValues.Any(v => v != default))
        {
            var minValue = nextValues.Min();
            if (minValue != DateOnly.MaxValue)
                yield return minValue;
            else
                yield break;

            for (var i = 0; i < nextValues.Length; i++)
            {
                if (nextValues[i] == minValue)
                {
                    var hasNext = enumerators[i].MoveNext();
                    if (hasNext)
                    {
                        nextValues[i] = enumerators[i].Current;
                    }
                    else
                    {
                        nextValues[i] = DateOnly.MaxValue;
                    }

                    break;
                }
            }
        }
    }

    public static IEnumerable<DateOnly> Merge(this IDateEnumerable dates, params IDateEnumerable[] others)
        => Merge(new[] { dates }.Concat(others));
}