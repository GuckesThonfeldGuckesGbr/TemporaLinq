using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Germany;

/// <summary>
/// Provides German national (federal) holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        var ascensionDay = easter.AddDays(39);
        var whitMonday = easter.AddDays(50);
        var mayFirst = new DateOnly(year, 5, 1);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(easter.AddDays(-2), GoodFriday),
                new(easter.AddDays(1), EasterMonday),
                new(ascensionDay, AscensionDay),
                new(mayFirst, LabourDay),
                new(whitMonday, WhitMonday),
                new(new DateOnly(year, 10, 3), DayOfGermanUnity),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay)
            }
            .Order()
            .ToImmutableList();
    }
}