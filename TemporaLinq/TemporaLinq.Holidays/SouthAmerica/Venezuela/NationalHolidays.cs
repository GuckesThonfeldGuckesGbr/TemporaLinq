using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.SouthAmerica.Venezuela;

/// <summary>
/// Provides Venezuelan national public holidays: fixed civil/religious days plus the Christian
/// Easter-relative Carnival Monday/Tuesday, Maundy Thursday, and Good Friday.
///
/// Venezuela's government periodically shifts specific holidays by decree (e.g. to create long
/// "puente" weekends); this covers the stable, consistently-observed annual list only, the same
/// treatment already given to Bosnia and Herzegovina's entity-fragmentation issue.
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
                new(easter.AddDays(-48), CarnivalMonday),
                new(easter.AddDays(-47), CarnivalTuesday),
                new(easter.AddDays(-3), MaundyThursday),
                new(easter.AddDays(-2), GoodFriday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 6, 24), BattleOfCarababoDay),
                new(new DateOnly(year, 7, 5), IndependenceDay),
                new(new DateOnly(year, 7, 24), BolivarsBirthday),
                new(new DateOnly(year, 10, 12), IndigenousResistanceDay),
                new(new DateOnly(year, 12, 24), ChristmasEve),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 31), NewYearsEve),
            }
            .Order()
            .ToImmutableList();
    }
}
