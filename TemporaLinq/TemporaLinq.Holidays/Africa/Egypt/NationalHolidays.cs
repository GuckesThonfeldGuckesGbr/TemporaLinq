using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Africa.Egypt;

/// <summary>
/// Provides Egyptian national public holidays.
/// </summary>
/// <remarks>
/// Eid al-Fitr, the Day of Arafat, Eid al-Adha, the Islamic New Year, and the Prophet's
/// Birthday are computed via the tabular Hijri calendar (<see cref="HijriCalendarCalculation"/>),
/// a deterministic approximation: Egypt's real-world government announcements, which
/// follow moon sighting, can differ from this calculation by +/-1, rarely +/-2, days.
/// Sham el-Nessim (a secular spring festival observed by Egyptians of all religions) falls
/// the day after Coptic Easter Sunday, computed via
/// <see cref="EasterSundayCalculation.ChristianOrthodox"/> - Coptic Easter uses the same
/// Julian-calendar computus as Eastern Orthodox Easter.
/// </remarks>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var copticEaster = EasterSundayCalculation.ChristianOrthodox.ForYear(year);

        var holidays = new List<Holiday>
        {
            new(new DateOnly(year, 1, 7), ChristmasDay),
            new(new DateOnly(year, 1, 25), RevolutionDayOfEgypt),
            new(new DateOnly(year, 4, 25), SinaiLiberationDay),
            new(copticEaster.AddDays(1), ShamElNessim),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(new DateOnly(year, 7, 23), RevolutionDayOfEgypt),
            new(new DateOnly(year, 10, 6), ArmedForcesDay),
        };

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
            holidays.Add(new Holiday(date, EidAlFitr));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2))
            holidays.Add(new Holiday(date, EidAlFitr));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 3))
            holidays.Add(new Holiday(date, EidAlFitr));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 9))
            holidays.Add(new Holiday(date, ArafatDay));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10))
            holidays.Add(new Holiday(date, EidAlAdha));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 11))
            holidays.Add(new Holiday(date, EidAlAdha));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 12))
            holidays.Add(new Holiday(date, EidAlAdha));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1))
            holidays.Add(new Holiday(date, IslamicNewYear));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12))
            holidays.Add(new Holiday(date, ProphetsBirthday));

        return holidays.Order().ToImmutableList();
    }
}
