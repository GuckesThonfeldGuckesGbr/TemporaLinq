using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Serbia;

/// <summary>
/// Provides Serbian national public holidays. Movable feasts follow the Orthodox
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
                new(new DateOnly(year, 1, 2), NewYearsDay),
                new(new DateOnly(year, 1, 7), ChristmasDay),
                new(new DateOnly(year, 2, 15), StatehoodDayOfSerbia),
                new(new DateOnly(year, 2, 16), StatehoodDayOfSerbia),
                new(orthodoxEaster.AddDays(-2), GoodFriday),
                new(orthodoxEaster.AddDays(-1), HolySaturday),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 2), LabourDay),
                new(new DateOnly(year, 11, 11), ArmisticeDay),
            }
            .Order()
            .ToImmutableList();
    }
}
