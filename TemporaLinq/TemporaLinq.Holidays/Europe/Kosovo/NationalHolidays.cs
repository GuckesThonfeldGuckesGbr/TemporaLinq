using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Kosovo;

/// <summary>
/// Provides Kosovar national public holidays under Law No. 03/L-064 for Official
/// Holidays in the Republic of Kosovo. Kosovo recognizes both Catholic and
/// Orthodox Christmas/Easter (the latter for the Serb minority) alongside the
/// Muslim-majority population's Eid holidays and civil/European-integration days.
/// Eid al-Fitr and Eid al-Adha are computed from the Hijri calendar via
/// <see cref="HijriCalendarCalculation"/> — a deterministic approximation that can
/// differ by +/-1, rarely +/-2, days from the real-world moon-sighting-confirmed
/// date.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var catholicEaster = EasterSundayCalculation.Christian.ForYear(year);
        var orthodoxEaster = EasterSundayCalculation.ChristianOrthodox.ForYear(year);

        var holidays = new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 1, 2), NewYearsDay),
                new(new DateOnly(year, 1, 7), ChristmasDay),
                new(new DateOnly(year, 2, 17), IndependenceDay),
                new(new DateOnly(year, 4, 9), ConstitutionDayOfKosovo),
                new(catholicEaster, EasterSunday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 9), EuropeDay),
                new(orthodoxEaster, EasterSunday),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)
            .Select(date => new Holiday(date, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10)
            .Select(date => new Holiday(date, EidAlAdha)));

        return holidays.Order().ToImmutableList();
    }
}
