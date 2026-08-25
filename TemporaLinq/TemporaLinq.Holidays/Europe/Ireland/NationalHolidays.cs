using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Ireland;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

/// <summary>
/// Provides Irish national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        var february1 = new DateOnly(year, 2, 1);
        var stBrigidsDay = february1.DayOfWeek == DayOfWeek.Friday
            ? february1
            : Dates.Invariant().From(february1).First(DayOfWeek.Monday);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(stBrigidsDay, StBrigidsDay),
                new(new DateOnly(year, 3, 17), StPatricksDay),
                new(easter.AddDays(1), EasterMonday),
                new(Dates.Invariant().From(new DateOnly(year, 5, 1)).First(DayOfWeek.Monday), EarlyMayBankHoliday),
                new(Dates.Invariant().From(new DateOnly(year, 6, 1)).First(DayOfWeek.Monday), JuneBankHoliday),
                new(Dates.Invariant().From(new DateOnly(year, 8, 1)).First(DayOfWeek.Monday), AugustBankHoliday),
                new(Dates.Invariant().From(new DateOnly(year, 10, 25)).First(DayOfWeek.Monday), OctoberBankHoliday),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
