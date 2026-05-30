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
    
    public static DateOnly First(this IMonotonicallyAscendingEnumerable<DateOnly> seq, DayOfWeek dayOfWeek)
    {
        var ret = seq.FirstOrDefault(dayOfWeek);
        return ret != default
            ? ret
            : throw new InvalidOperationException($"No {dayOfWeek} found in date sequence");
    }

    public static DateOnly FirstOrDefault(this IMonotonicallyAscendingEnumerable<DateOnly> seq, DayOfWeek dayOfWeek)
        => seq.FirstOrDefault(d => d.DayOfWeek == dayOfWeek);

    public static IMonotonicallyAscendingEnumerable<T> AsMonotonicallyAscendingEnumerable<T>(
        this IEnumerable<T> enumerable) where T : IComparable<T>
        => new MonotonicAscendingEnumerableWrapper<T>(enumerable);
}