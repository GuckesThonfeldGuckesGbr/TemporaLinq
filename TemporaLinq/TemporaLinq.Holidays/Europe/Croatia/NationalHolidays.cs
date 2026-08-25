using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Croatia;

/// <summary>
/// Provides Croatian national public holidays.
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
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(easter.AddDays(60), CorpusChristi),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 30), StatehoodDayOfCroatia),
                new(new DateOnly(year, 6, 22), AntiFascistStruggleDay),
                new(new DateOnly(year, 8, 5), VictoryAndHomelandThanksgivingDay),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 18), RemembranceDayOfCroatia),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
