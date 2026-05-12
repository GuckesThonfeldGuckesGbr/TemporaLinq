namespace TemporaLinq;

public static class LinqExtensions
{
    public static IDateEnumerable Where(this IDateEnumerable seq, Func<DateOnly, bool> predicate)
        => Enumerable.Where(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Where(this IDateEnumerable seq, Func<DateOnly, int, bool> predicate)
        => Enumerable.Where(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Take(this IDateEnumerable seq, int count)
        => Enumerable.Take(seq, count).AsDateEnumerable(seq.Config);

    public static IDateEnumerable TakeWhile(this IDateEnumerable seq, Func<DateOnly, bool> predicate)
        => Enumerable.TakeWhile(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable TakeWhile(this IDateEnumerable seq, Func<DateOnly, int, bool> predicate)
        => Enumerable.TakeWhile(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable TakeLast(this IDateEnumerable seq, int count)
        => Enumerable.TakeLast(seq, count).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Skip(this IDateEnumerable seq, int count)
        => Enumerable.Skip(seq, count).AsDateEnumerable(seq.Config);

    public static IDateEnumerable SkipWhile(this IDateEnumerable seq, Func<DateOnly, bool> predicate)
        => Enumerable.SkipWhile(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable SkipWhile(this IDateEnumerable seq, Func<DateOnly, int, bool> predicate)
        => Enumerable.SkipWhile(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable SkipLast(this IDateEnumerable seq, int count)
        => Enumerable.SkipLast(seq, count).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Select(this IDateEnumerable seq, Func<DateOnly, DateOnly> selector)
        => Enumerable.Select(seq, selector).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Select(this IDateEnumerable seq, Func<DateOnly, int, DateOnly> selector)
        => Enumerable.Select(seq, selector).AsDateEnumerable(seq.Config);

    public static IDateEnumerable SelectMany(this IDateEnumerable seq, Func<DateOnly, IEnumerable<DateOnly>> selector)
        => Enumerable.SelectMany(seq, selector).AsDateEnumerable(seq.Config);

    public static IDateEnumerable SelectMany(this IDateEnumerable seq,
        Func<DateOnly, int, IEnumerable<DateOnly>> selector)
        => Enumerable.SelectMany(seq, selector).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Distinct(this IDateEnumerable seq)
        => Enumerable.Distinct(seq).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Distinct(this IDateEnumerable seq, IEqualityComparer<DateOnly> comparer)
        => Enumerable.Distinct(seq, comparer).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Union(this IDateEnumerable seq, IEnumerable<DateOnly> second)
        => Enumerable.Union(seq, second).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Intersect(this IDateEnumerable seq, IEnumerable<DateOnly> second)
        => Enumerable.Intersect(seq, second).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Intersect(this IDateEnumerable seq, IEnumerable<DateOnly> second,
        IEqualityComparer<DateOnly> comparer)
        => Enumerable.Intersect(seq, second, comparer).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Concat(this IDateEnumerable seq, IEnumerable<DateOnly> second)
        => Enumerable.Concat(seq, second).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Reverse(this IDateEnumerable seq)
        => Enumerable.Reverse(seq).AsDateEnumerable(seq.Config);

    public static IMonotonicallyAscendingEnumerable<T> AsMonotonicallyAscendingEnumerable<T>(this IEnumerable<T> enumerable) where T : IComparable, IComparable<T>
        => new MonotonicAscendingEnumerableWrapper<T>(enumerable);
}