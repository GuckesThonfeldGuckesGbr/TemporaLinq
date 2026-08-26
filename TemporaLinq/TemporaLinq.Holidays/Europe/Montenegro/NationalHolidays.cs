using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Montenegro;

/// <summary>
/// Provides Montenegrin national public holidays. All dates are fixed —
/// Montenegro's state calendar has no movable feast.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 1, 2), NewYearsDay),
                new(new DateOnly(year, 1, 7), ChristmasDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 2), LabourDay),
                new(new DateOnly(year, 5, 21), IndependenceDay),
                new(new DateOnly(year, 7, 13), StatehoodDayOfMontenegro),
            }
            .Order()
            .ToImmutableList();
    }
}
