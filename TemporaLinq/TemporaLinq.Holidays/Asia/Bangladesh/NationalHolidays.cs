using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Bangladesh;

/// <summary>
/// Provides Bangladeshi national public holidays: fixed civil dates and the
/// Islamic-calendar holidays (Eid al-Fitr, Eid al-Adha, Ashura, Eid
/// Milad-un-Nabi), computed via <see cref="HijriCalendarCalculation"/> — see
/// that class's documentation for the +/-1, rarely +/-2, day real-world
/// moon-sighting approximation caveat. Bangladesh's Hindu- and
/// Buddhist-calendar minority holidays (Durga Puja, Buddha Purnima) are
/// deliberately out of scope: they require a Bengali/Hindu lunisolar or
/// Buddhist calendar calculation with no .NET support today, and are deferred
/// pending a future calendar calculation mechanism for those calendars.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var holidays = new List<Holiday>
        {
            new(new DateOnly(year, 2, 21), LanguageMovementDay),
            new(new DateOnly(year, 3, 26), IndependenceDay),
            new(new DateOnly(year, 4, 14), BengaliNewYear),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(new DateOnly(year, 8, 15), NationalMourningDay),
            new(new DateOnly(year, 12, 16), VictoryDay),
            new(new DateOnly(year, 12, 25), ChristmasDay),
        };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 3).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 11).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 12).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 1, 10).Select(d => new Holiday(d, AshuraDay)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12).Select(d => new Holiday(d, EidMiladUnNabi)));

        return holidays.Order().ToImmutableList();
    }
}
