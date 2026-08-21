using TemporaLinq.Dates;
using TemporaLinq.Times;

namespace TemporaLinq;

public static class DateTimeCombinators
{
    public static IMonotonicallyAscendingEnumerable<DateTime> On(this IDateEnumerable dates, ITimeEnumerable times)
        => OnImpl(dates, times).AsMonotonicallyAscendingEnumerable();

    private static IEnumerable<DateTime> OnImpl(IDateEnumerable dates, ITimeEnumerable times)
    {
        return dates.SelectMany(_ => times, (date, time) => date.ToDateTime(time));
    }
}
