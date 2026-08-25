using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Ukraine;

/// <summary>
/// Provides Ukrainian national public holidays.
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
                new(new DateOnly(year, 3, 8), InternationalWomensDay),
                new(orthodoxEaster, EasterSunday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 8), VictoryDay),
                new(new DateOnly(year, 6, 28), ConstitutionDayOfUkraine),
                new(new DateOnly(year, 8, 24), IndependenceDay),
                new(new DateOnly(year, 10, 1), DefendersDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
