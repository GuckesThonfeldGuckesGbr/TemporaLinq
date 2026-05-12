using System.Collections;

namespace TemporaLinq;

internal record DateEnumerableWrapper(IEnumerable<DateOnly> Dates, CalendarConfig Config) : MonotonicAscendingEnumerableWrapper<DateOnly>(Dates), IDateEnumerable
{
    public IEnumerator<DateOnly> GetEnumerator() => Dates.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}