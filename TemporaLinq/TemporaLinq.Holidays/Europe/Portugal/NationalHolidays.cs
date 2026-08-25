using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Portugal;

/// <summary>
/// Provides Portuguese national public holidays.
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
                new(easter.AddDays(-2), GoodFriday),
                new(easter, EasterSunday),
                new(new DateOnly(year, 4, 25), FreedomDayOfPortugal),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 6, 10), PortugalDay),
                new(easter.AddDays(60), CorpusChristi),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 10, 5), RepublicDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 12, 1), RestorationOfIndependenceDay),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
