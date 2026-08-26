using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Estonia;

/// <summary>
/// Provides Estonian national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 2, 24), IndependenceDay),
                new(easter.AddDays(-2), GoodFriday),
                new(easter, EasterSunday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(easter.AddDays(49), WhitSunday),
                new(new DateOnly(year, 6, 23), VictoryDay),
                new(new DateOnly(year, 6, 24), MidsummerDay),
                new(new DateOnly(year, 8, 20), RestorationOfIndependenceDay),
                new(new DateOnly(year, 12, 24), ChristmasEve),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), BoxingDay),
            }
            .Order()
            .ToImmutableList();
    }
}
