namespace de.baggerbagger.TemporaLinq;

public static class DateLinq
{
    public static IDateEnumerable Only(this IDateEnumerable seq, params DayOfWeek[] weekdays)
        => seq.Where(date => weekdays.Contains(date.DayOfWeek)).AsDateEnumerable(seq.Config);
    
    public static IDateEnumerable Except(this IDateEnumerable seq, params DayOfWeek[] weekdays)
        => seq.Where(date => !weekdays.Contains(date.DayOfWeek)).AsDateEnumerable(seq.Config);
    
    public static IDateEnumerable EveryNth(this IDateEnumerable seq, int n)
        => seq.Where((_, idx) => idx % n == 0).AsDateEnumerable(seq.Config);

    public static IDateEnumerable BusinessDays(this IDateEnumerable seq)
        => seq.Where(d => !seq.Config.WeekendDays.Contains(d.DayOfWeek)).AsDateEnumerable(seq.Config);

    public static IDateEnumerable WithContext(this IEnumerable<DateOnly> seq, IDateEnumerable context)
        => seq.AsDateEnumerable(context.Config);

    public static IDateEnumerable AsDateEnumerable(this IEnumerable<DateOnly> seq, CalendarConfig config)
        => new DateEnumerableWrapper(seq, config);
}