using System.Collections;

namespace de.baggerbagger.TemporaLinq;

internal record DateEnumerableWrapper(IEnumerable<DateOnly> Dates, CalendarConfig Config) : IDateEnumerable
{
    public IEnumerator<DateOnly> GetEnumerator() => Dates.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}