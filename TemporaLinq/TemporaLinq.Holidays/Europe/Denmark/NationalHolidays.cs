using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Denmark;

/// <summary>
/// Provides Danish national public holidays. Note: "Store Bededag" (General Prayer
/// Day) was abolished as an official public holiday in Denmark starting in 2024
/// and is deliberately not modeled here.
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
                new(easter.AddDays(-3), MaundyThursday),
                new(easter.AddDays(-2), GoodFriday),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(easter.AddDays(39), AscensionDay),
                new(easter.AddDays(49), WhitSunday),
                new(easter.AddDays(50), WhitMonday),
                new(new DateOnly(year, 6, 5), ConstitutionDayOfDenmark),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
