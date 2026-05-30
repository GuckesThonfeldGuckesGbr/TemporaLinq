namespace TemporaLinq;

public static class Operations
{
    /// <summary>
    /// Returns a stream of dates that contains all dates from the given streams in ascending order.
    /// </summary>
    /// <param name="dateStreams"></param>
    /// <returns></returns>
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

    public static IDateEnumerable Merge(this IDateEnumerable dates,
        params IMonotonicallyAscendingEnumerable<DateOnly>[] others)
        => Merge(new[] { dates }.Concat(others)).AsDateEnumerable(dates.Config);

    public static IMonotonicallyAscendingEnumerable<T> Merge<T>(this IMonotonicallyAscendingEnumerable<T> dates,
        params IMonotonicallyAscendingEnumerable<T>[] others) where T : IComparable<T>
        => Merge(new[] { dates }.Concat(others)).AsMonotonicallyAscendingEnumerable();

    public static IDateEnumerable Except(this IDateEnumerable seq, IMonotonicallyAscendingEnumerable<DateOnly> second)
    {
        return new DateEnumerableWrapper(ExceptImpl(), seq.Config);

        IEnumerable<DateOnly> ExceptImpl()
        {
            using var seqEnum = seq.GetEnumerator();
            using var secondEnum = second.GetEnumerator();

            var seqHasNext = seqEnum.MoveNext();
            var secondHasNext = secondEnum.MoveNext();

            while (seqHasNext)
            {
                if (!secondHasNext)
                {
                    do
                    {
                        yield return seqEnum.Current;
                    } while (seqEnum.MoveNext());

                    yield break;
                }

                if (seqEnum.Current < secondEnum.Current)
                {
                    yield return seqEnum.Current;
                    seqHasNext = seqEnum.MoveNext();
                }
                else if (seqEnum.Current > secondEnum.Current)
                {
                    secondHasNext = secondEnum.MoveNext();
                }
                else
                {
                    seqHasNext = seqEnum.MoveNext();
                    secondHasNext = secondEnum.MoveNext();
                }
            }
        }
    }

    /// <summary>
    /// Returns a stream of values that contains all unique values from both sorted streams in ascending order.
    /// Uses an efficient O(n+m) merge algorithm.
    /// </summary>
    public static IDateEnumerable Union(this IDateEnumerable first, IMonotonicallyAscendingEnumerable<DateOnly> second)
    {
        return new DateEnumerableWrapper(UnionImpl(first, second), first.Config);

        IEnumerable<DateOnly> UnionImpl(IDateEnumerable a, IMonotonicallyAscendingEnumerable<DateOnly> b)
        {
            using var firstEnum = a.GetEnumerator();
            using var secondEnum = b.GetEnumerator();

            var firstHasNext = firstEnum.MoveNext();
            var secondHasNext = secondEnum.MoveNext();

            while (firstHasNext || secondHasNext)
            {
                if (!secondHasNext)
                {
                    do
                    {
                        yield return firstEnum.Current;
                    } while (firstEnum.MoveNext());

                    yield break;
                }

                if (!firstHasNext)
                {
                    do
                    {
                        yield return secondEnum.Current;
                    } while (secondEnum.MoveNext());

                    yield break;
                }

                if (firstEnum.Current < secondEnum.Current)
                {
                    yield return firstEnum.Current;
                    firstHasNext = firstEnum.MoveNext();
                }
                else if (firstEnum.Current > secondEnum.Current)
                {
                    yield return secondEnum.Current;
                    secondHasNext = secondEnum.MoveNext();
                }
                else
                {
                    yield return firstEnum.Current;
                    firstHasNext = firstEnum.MoveNext();
                    secondHasNext = secondEnum.MoveNext();
                }
            }
        }
    }

    /// <summary>
    /// Returns a stream of values that contains only values present in both sorted streams in ascending order.
    /// Uses an efficient O(n+m) merge algorithm.
    /// </summary>
    public static IDateEnumerable Intersect(this IDateEnumerable first,
        IMonotonicallyAscendingEnumerable<DateOnly> second)
    {
        return new DateEnumerableWrapper(IntersectImpl(first, second), first.Config);

        IEnumerable<DateOnly> IntersectImpl(IDateEnumerable a, IMonotonicallyAscendingEnumerable<DateOnly> b)
        {
            using var firstEnum = a.GetEnumerator();
            using var secondEnum = b.GetEnumerator();

            var firstHasNext = firstEnum.MoveNext();
            var secondHasNext = secondEnum.MoveNext();

            while (firstHasNext && secondHasNext)
            {
                if (firstEnum.Current < secondEnum.Current)
                {
                    firstHasNext = firstEnum.MoveNext();
                }
                else if (firstEnum.Current > secondEnum.Current)
                {
                    secondHasNext = secondEnum.MoveNext();
                }
                else
                {
                    yield return firstEnum.Current;
                    firstHasNext = firstEnum.MoveNext();
                    secondHasNext = secondEnum.MoveNext();
                }
            }
        }
    }

    public static IDateEnumerable Distinct(this IDateEnumerable seq)
    {
        return new DateEnumerableWrapper(DistinctImpl(seq), seq.Config);

        IEnumerable<DateOnly> DistinctImpl(IDateEnumerable dates)
        {
            using var dateEnumerator = dates.GetEnumerator();
            DateOnly? last = null;
            while (dateEnumerator.MoveNext())
            {
                var current = dateEnumerator.Current;
                if (current != last)
                {
                    yield return current;
                    last = current;
                }
            }
        }
    }
}