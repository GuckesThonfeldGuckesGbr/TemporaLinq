using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Slovenia;

/// <summary>
/// Provides Slovenian national public holidays.
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
                new(new DateOnly(year, 1, 2), SecondJanuary),
                new(new DateOnly(year, 2, 8), PresernDay),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 4, 27), DayOfUprisingAgainstOccupation),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 2), LabourDay),
                new(new DateOnly(year, 6, 25), StatehoodDayOfSlovenia),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 10, 31), ReformationDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), IndependenceAndUnityDayOfSlovenia),
            }
            .Order()
            .ToImmutableList();
    }
}
