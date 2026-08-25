using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Romania;

/// <summary>
/// Provides Romanian national public holidays. Movable feasts follow the Orthodox
/// Easter calculation, per Romanian law.
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
                new(new DateOnly(year, 1, 2), SecondJanuary),
                new(new DateOnly(year, 1, 6), Epiphany),
                new(new DateOnly(year, 1, 7), SynaxisOfStJohnTheBaptist),
                new(new DateOnly(year, 1, 24), UnionDayOfRomania),
                new(orthodoxEaster.AddDays(-2), GoodFriday),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 6, 1), ChildrensDay),
                new(orthodoxEaster.AddDays(49), WhitSunday),
                new(orthodoxEaster.AddDays(50), WhitMonday),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 11, 30), StAndrewsDay),
                new(new DateOnly(year, 12, 1), NationalDayOfRomania),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
