using TemporaLinq.Dates;
using TemporaLinq.Times;

namespace TemporaLinq;

public static class DateTimeCombinators
{
    public static IMonotonicallyAscendingEnumerable<DateTime> On(this IDateEnumerable dates, ITimeEnumerable times)
        => OnImpl(dates, times).AsMonotonicallyAscendingEnumerable();

    private static IEnumerable<DateTime> OnImpl(IDateEnumerable dates, ITimeEnumerable times)
    {
        foreach (var date in dates)
        foreach (var time in times)
            yield return date.ToDateTime(time);
    }
}
