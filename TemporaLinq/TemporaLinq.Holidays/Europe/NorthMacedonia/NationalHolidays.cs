using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.NorthMacedonia;

/// <summary>
/// Provides the state-mandated national public holidays of North Macedonia.
/// Movable feasts follow the Orthodox Easter calculation. Additional
/// religion-specific non-working days each citizen may choose (including
/// Islamic ones) are out of scope.
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
                new(new DateOnly(year, 1, 2), NewYearsDay),
                new(new DateOnly(year, 1, 7), ChristmasDay),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 2), LabourDay),
                new(new DateOnly(year, 5, 24), SaintsCyrilAndMethodiusDay),
                new(new DateOnly(year, 8, 2), IlindenDay),
                new(new DateOnly(year, 9, 8), IndependenceDay),
                new(new DateOnly(year, 10, 11), DayOfMacedonianUprising),
                new(new DateOnly(year, 10, 23), RevolutionaryStruggleDayOfMacedonia),
                new(new DateOnly(year, 12, 8), StClementOfOhridDay),
            }
            .Order()
            .ToImmutableList();
    }
}
