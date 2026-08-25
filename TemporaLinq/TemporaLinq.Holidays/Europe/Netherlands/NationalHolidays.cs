using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Netherlands;

/// <summary>
/// Provides Dutch national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        var kingsDayRaw = new DateOnly(year, 4, 27);
        var kingsDay = kingsDayRaw.DayOfWeek == DayOfWeek.Sunday ? kingsDayRaw.AddDays(-1) : kingsDayRaw;

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(kingsDay, KingsDayOfTheNetherlands),
                new(new DateOnly(year, 5, 5), LiberationDay),
                new(easter.AddDays(39), AscensionDay),
                new(easter.AddDays(49), WhitSunday),
                new(easter.AddDays(50), WhitMonday),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
