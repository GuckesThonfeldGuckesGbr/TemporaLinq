using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Spain;

/// <summary>
/// Provides Spain national (federal) holidays.
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
                new(new DateOnly(year, 1, 6), Epiphany),
                new(easter.AddDays(-2), GoodFriday),
                // new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 10, 12), NationalDayOfSpain),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 12, 6), SpanishConstitutionDay),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}