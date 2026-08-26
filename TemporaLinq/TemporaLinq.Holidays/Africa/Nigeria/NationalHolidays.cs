using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Africa.Nigeria;

/// <summary>
/// Provides Nigerian national public holidays.
/// </summary>
/// <remarks>
/// Eid al-Fitr, Eid al-Adha, and Id el-Maulud (Prophet's Birthday) are computed via the
/// tabular Hijri calendar (<see cref="HijriCalendarCalculation"/>), a deterministic
/// approximation: Nigeria's real-world government announcements, which follow moon
/// sighting, can differ from this calculation by +/-1, rarely +/-2, days.
/// </remarks>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);

        var holidays = new List<Holiday>
        {
            new(new DateOnly(year, 1, 1), NewYearsDay),
            new(easter.AddDays(-2), GoodFriday),
            new(easter.AddDays(1), EasterMonday),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(new DateOnly(year, 6, 12), DemocracyDayOfNigeria),
            new(new DateOnly(year, 10, 1), IndependenceDay),
            new(new DateOnly(year, 12, 25), ChristmasDay),
            new(new DateOnly(year, 12, 26), BoxingDay),
        };

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
            holidays.Add(new Holiday(date, EidAlFitr));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2))
            holidays.Add(new Holiday(date, EidAlFitr));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10))
            holidays.Add(new Holiday(date, EidAlAdha));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 11))
            holidays.Add(new Holiday(date, EidAlAdha));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12))
            holidays.Add(new Holiday(date, ProphetsBirthday));

        return holidays.Order().ToImmutableList();
    }
}
