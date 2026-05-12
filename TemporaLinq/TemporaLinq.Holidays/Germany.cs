using System.Collections;

namespace TemporaLinq.Holidays.Christian;

/// <summary>
/// Provides German national holidays.
/// </summary>
public record GermanHolidays : IEnumerable<Holiday>
{
    public DateOnly StartDate { get; private init; }
    public DateOnly EndDate { get; private init; }

    private GermanHolidays()
    {
        StartDate = DateOnly.MinValue;
        EndDate = DateOnly.MaxValue;
    }

    public static GermanHolidays Create() => new();

    public GermanHolidays From(DateOnly date) => this with { StartDate = date };
    public GermanHolidays To(DateOnly date) => this with { EndDate = date };

    /// <summary>
    /// Looks up a holiday for the specified date.
    /// </summary>
    /// <param name="date">The date to look up.</param>
    /// <returns>The Holiday if found, otherwise null.</returns>
    public Holiday? GetHoliday(DateOnly date)
        => this.FirstOrDefault(h => h.Date == date) is { } holiday && holiday.Date == date
            ? holiday
            : null;

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
    /// <summary>
    /// Checks if the specified date is a holiday.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the date is a holiday, otherwise false.</returns>
    public bool IsHoliday(DateOnly date)
        => this.Any(h => h.Date == date);

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

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static IEnumerable<Holiday> GetHolidaysForYear(int year)
    {
        var easter = CalculateEasterSunday(year);
        var ascensionDay = easter.AddDays(39);
        var corpusChristi = easter.AddDays(60);
        var whitMonday = easter.AddDays(50);
        var mayFirst = new DateOnly(year, 5, 1);

        yield return new Holiday(new DateOnly(year, 1, 1), "Neujahr");
        yield return new Holiday(easter.AddDays(-2), "Karfreitag");
        yield return new Holiday(easter.AddDays(1), "Ostermontag");

        if (ascensionDay < mayFirst)
        {
            yield return new Holiday(ascensionDay, "Christi Himmelfahrt");
            yield return new Holiday(mayFirst, "Tag der Arbeit");
        }
        else
        {
            yield return new Holiday(mayFirst, "Tag der Arbeit");
            yield return new Holiday(ascensionDay, "Christi Himmelfahrt");
        }

        yield return new Holiday(whitMonday, "Pfingstmontag");
        yield return new Holiday(corpusChristi, "Fronleichnam");

        yield return new Holiday(new DateOnly(year, 10, 3), "Tag der Deutschen Einheit");
        yield return new Holiday(new DateOnly(year, 12, 25), "Erster Weihnachtsfeiertag");
        yield return new Holiday(new DateOnly(year, 12, 26), "Zweiter Weihnachtsfeiertag");
    }

    /// <summary>
    /// Calculates Easter Sunday for a given year using the Gauss Easter formula (Ostersonntagsformel).
    /// </summary>
    public static DateOnly CalculateEasterSunday(int year)
    {
        var a = year % 19;
        var b = year / 100;
        var c = year % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;

        var month = (h + l - 7 * m + 114) / 31;
        var day = ((h + l - 7 * m + 114) % 31) + 1;

        return new DateOnly(year, month, day);
    }
}
/// <summary>
/// Represents a holiday with its date and name.
/// </summary>
public readonly record struct Holiday(DateOnly Date, string Name)
{
    public static implicit operator DateOnly(Holiday holiday) => holiday.Date;
    public static implicit operator DateOnly?(Holiday? holiday) => holiday?.Date;
}

/// <summary>
/// Provides access to German national holidays.
/// </summary>
public static class Germany
{
    public static GermanHolidays Holidays => GermanHolidays.Create();
}
