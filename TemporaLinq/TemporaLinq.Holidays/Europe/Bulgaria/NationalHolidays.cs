using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Bulgaria;

/// <summary>
/// Provides Bulgarian national public holidays. Movable feasts follow the Orthodox
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
                new(new DateOnly(year, 3, 3), LiberationDayOfBulgaria),
                new(orthodoxEaster.AddDays(-2), GoodFriday),
                new(orthodoxEaster.AddDays(-1), HolySaturday),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 6), StGeorgesDay),
                new(new DateOnly(year, 5, 24), SaintsCyrilAndMethodiusDay),
                new(new DateOnly(year, 9, 6), UnificationDayOfBulgaria),
                new(new DateOnly(year, 9, 22), IndependenceDay),
                new(new DateOnly(year, 12, 24), ChristmasEve),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
