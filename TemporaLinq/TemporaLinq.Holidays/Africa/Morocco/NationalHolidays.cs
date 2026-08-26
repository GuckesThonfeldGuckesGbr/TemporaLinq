using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Africa.Morocco;

/// <summary>
/// Provides Moroccan national public holidays.
/// </summary>
/// <remarks>
/// Eid al-Fitr, Eid al-Adha, the Islamic New Year, and the Prophet's Birthday are computed
/// via the tabular Hijri calendar (<see cref="HijriCalendarCalculation"/>), a deterministic
/// approximation: Morocco's real-world government announcements, which follow moon
/// sighting, can differ from this calculation by +/-1, rarely +/-2, days.
/// </remarks>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var holidays = new List<Holiday>
        {
            new(new DateOnly(year, 1, 1), NewYearsDay),
            new(new DateOnly(year, 1, 11), IndependenceManifestoDay),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(new DateOnly(year, 7, 30), ThroneDayOfMorocco),
            new(new DateOnly(year, 8, 14), OuedEdDahabDay),
            new(new DateOnly(year, 8, 20), RevolutionOfTheKingAndThePeopleDay),
            new(new DateOnly(year, 8, 21), YouthDayOfMorocco),
            new(new DateOnly(year, 11, 6), GreenMarchDay),
            new(new DateOnly(year, 11, 18), IndependenceDay),
        };

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
            holidays.Add(new Holiday(date, EidAlFitr));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2))
            holidays.Add(new Holiday(date, EidAlFitr));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10))
            holidays.Add(new Holiday(date, EidAlAdha));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 11))
            holidays.Add(new Holiday(date, EidAlAdha));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1))
            holidays.Add(new Holiday(date, IslamicNewYear));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12))
            holidays.Add(new Holiday(date, ProphetsBirthday));

        return holidays.Order().ToImmutableList();
    }
}
