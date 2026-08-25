namespace TemporaLinq.Dates;

public static class DateLinq
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

    public static IDateEnumerable Only(this IDateEnumerable seq, params DayOfWeek[] weekdays)
        => seq.Where(date => weekdays.Contains(date.DayOfWeek)).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Except(this IDateEnumerable seq, params DayOfWeek[] weekdays)
        => seq.Where(date => !weekdays.Contains(date.DayOfWeek)).AsDateEnumerable(seq.Config);

    public static IDateEnumerable EveryNth(this IDateEnumerable seq, int n)
        => seq.Where((_, idx) => idx % n == 0).AsDateEnumerable(seq.Config);

    public static IDateEnumerable OnlyBusinessDays(this IDateEnumerable seq)
        => seq.Where(d => !seq.Config.WeekendDays.Contains(d.DayOfWeek)).AsDateEnumerable(seq.Config);

    public static IDateEnumerable OnlyWeekends(this IDateEnumerable seq)
        => seq.Where(d => seq.Config.WeekendDays.Contains(d.DayOfWeek)).AsDateEnumerable(seq.Config);

    public static IDateEnumerable WithContext(this IEnumerable<DateOnly> seq, IDateEnumerable context)
        => seq.AsDateEnumerable(context.Config);

    public static IDateEnumerable AsDateEnumerable(this IEnumerable<DateOnly> seq, CalendarConfig config)
        => new DateEnumerableWrapper(seq, config);
}
