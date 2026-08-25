using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.UnitedKingdom;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

/// <summary>
/// Provides United Kingdom national bank holidays common to England and Wales.
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
                new(easter.AddDays(-2), GoodFriday),
                new(easter.AddDays(1), EasterMonday),
                new(Dates.Invariant().From(new DateOnly(year, 5, 1)).First(DayOfWeek.Monday), EarlyMayBankHoliday),
                new(Dates.Invariant().From(new DateOnly(year, 5, 25)).First(DayOfWeek.Monday), SpringBankHoliday),
                new(Dates.Invariant().From(new DateOnly(year, 8, 25)).First(DayOfWeek.Monday), SummerBankHoliday),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), BoxingDay),
            }
            .Order()
            .ToImmutableList();
    }
}
