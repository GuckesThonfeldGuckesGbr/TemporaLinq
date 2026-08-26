using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Uzbekistan;

/// <summary>
/// Provides Uzbek national public holidays. Nowruz (Mar 21) is Uzbekistan's
/// fixed Gregorian civil-calendar spring holiday, not an astronomically
/// calculated date like Iran's. Eid al-Fitr (Ramadan Hayit) and Eid al-Adha
/// (Kurban Hayit) are computed via <see cref="HijriCalendarCalculation"/> —
/// see that class's documentation for the +/-1, rarely +/-2, day real-world
/// moon-sighting approximation caveat.
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
            new(new DateOnly(year, 1, 1), NewYearsDay),
            new(new DateOnly(year, 3, 8), InternationalWomensDay),
            new(new DateOnly(year, 3, 21), NowruzDay),
            new(new DateOnly(year, 5, 9), MemoryAndHonorDay),
            new(new DateOnly(year, 9, 1), IndependenceDay),
            new(new DateOnly(year, 10, 1), TeachersAndInstructorsDay),
            new(new DateOnly(year, 12, 8), ConstitutionDayOfUzbekistan),
        };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 3).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 11).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 12).Select(d => new Holiday(d, EidAlAdha)));

        return holidays.Order().ToImmutableList();
    }
}
