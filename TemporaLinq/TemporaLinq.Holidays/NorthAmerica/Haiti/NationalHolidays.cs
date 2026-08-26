using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.NorthAmerica.Haiti;

/// <summary>
/// Provides Haitian national public holidays: fixed civil/religious days plus the Christian
/// Easter-relative Carnival Monday/Tuesday and Good Friday.
///
/// Haiti's government periodically shifts specific holidays or adds one-off commemorative days by
/// decree; this covers the stable, consistently-observed annual list only, the same treatment
/// already given to Bosnia and Herzegovina's entity-fragmentation issue.
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
                new(new DateOnly(year, 1, 1), IndependenceDay),
                new(new DateOnly(year, 1, 2), AncestryDay),
                new(easter.AddDays(-48), CarnivalMonday),
                new(easter.AddDays(-47), CarnivalTuesday),
                new(easter.AddDays(-2), GoodFriday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 18), FlagAndUniversitiesDay),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 10, 17), DessalinesMemorialDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 2), AllSoulsDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
