namespace TemporaLinq;

public static class DateLinq
{
    public static IEnumerable<DateOnly> Only(this IEnumerable<DateOnly> seq, params DayOfWeek[] weekdays)
        => seq.Where(date => weekdays.Contains(date.DayOfWeek));
    
    public static IEnumerable<DateOnly> Except(this IEnumerable<DateOnly> seq, params DayOfWeek[] weekdays)
        => seq.Where(date => !weekdays.Contains(date.DayOfWeek));
    
    public static IEnumerable<DateOnly> EveryNth(this IEnumerable<DateOnly> seq, int n)
        => seq.Where((_, idx) => idx % n == 0);
}