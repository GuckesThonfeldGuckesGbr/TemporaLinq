using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Moldova;

/// <summary>
/// Provides Moldovan national public holidays. Movable feasts follow the Orthodox
/// Easter calculation.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var orthodoxEaster = EasterSundayCalculation.ChristianOrthodox.ForYear(year);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 1, 7), ChristmasDay),
                new(new DateOnly(year, 3, 8), InternationalWomensDay),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(orthodoxEaster.AddDays(9), MemorialDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 9), VictoryDay),
                new(new DateOnly(year, 8, 27), IndependenceDay),
                new(new DateOnly(year, 8, 31), OurLanguageDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
