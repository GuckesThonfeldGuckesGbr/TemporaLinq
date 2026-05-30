using System.Collections;

namespace TemporaLinq.Holidays;

public abstract record HolidayEnumerable<T> : IHolidayEnumerable where T : HolidayEnumerable<T>, new()
{
    // keep these private setters for testing 
    // ReSharper disable PropertyCanBeMadeInitOnly.Local
    public DateOnly StartDate { get; private set; } = DateOnly.MinValue;
    public DateOnly EndDate { get; private set; } = DateOnly.MaxValue;
    // ReSharper restore PropertyCanBeMadeInitOnly.Local

    public T From(DateOnly date)
        => (T)this with { StartDate = date };

    public T To(DateOnly date)
        => (T)this with { EndDate = date };
    
    public static T Create() => new();

    public IEnumerator<Holiday> GetEnumerator()
    {
        for (var year = StartDate.Year; year <= EndDate.Year; year++)
        {
            foreach (var holiday in GetHolidaysForYear(year))
            {
                if (holiday.Date < StartDate)
                    continue;
                if (holiday.Date > EndDate)
                    yield break;
                yield return holiday;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Looks up a holiday for the specified date.
    /// </summary>
    /// <param name="date">The date to look up.</param>
    /// <returns>The Holiday if found, otherwise null.</returns>
    public Holiday? GetHoliday(DateOnly date)
        => this.FirstOrDefault(h => h.Date == date) is var holiday && holiday.Date == date
            ? holiday
            : null;

    /// <summary>
    /// Checks if the specified date is a holiday.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the date is a holiday, otherwise false.</returns>
    public bool IsHoliday(DateOnly date)
        => this.Any(h => h.Date == date);

    /// <summary>
    /// Tries to get a holiday for the specified date.
    /// </summary>
    /// <param name="date">The date to look up.</param>
    /// <param name="holiday">The holiday if found, otherwise default.</param>
    /// <returns>True if a holiday was found, otherwise false.</returns>
    public bool TryGetHoliday(DateOnly date, out Holiday? holiday)
    {
        holiday = GetHoliday(date);
        return holiday is not null;
    }

    protected abstract IEnumerable<Holiday> GetHolidaysForYear(int year);
}