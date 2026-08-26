using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Malta;

/// <summary>
/// Provides Maltese national public holidays.
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
                new(new DateOnly(year, 2, 10), FeastOfStPaulsShipwreck),
                new(new DateOnly(year, 3, 19), FeastOfStJoseph),
                new(new DateOnly(year, 3, 31), FreedomDayOfMalta),
                new(easter.AddDays(-2), GoodFriday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 6, 7), SetteGiugno),
                new(new DateOnly(year, 6, 29), StPeterAndPaul),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 9, 8), OurLadyOfVictoriesDay),
                new(new DateOnly(year, 9, 21), IndependenceDay),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
                new(new DateOnly(year, 12, 13), RepublicDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
