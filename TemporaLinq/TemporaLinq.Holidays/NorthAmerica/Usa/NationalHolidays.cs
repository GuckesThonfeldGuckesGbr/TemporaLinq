using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.NorthAmerica.Usa;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

/// <summary>
/// Provides US national (federal) holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year) // TODO: date dependence of holidays: https://en.wikipedia.org/wiki/Federal_holidays_in_the_United_States
        => new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new (Dates.Invariant().From(new DateOnly(year, 1, 15)).First(DayOfWeek.Monday), BirthdayOfMartinLutherKingJr),
                new (Dates.Invariant().From(new DateOnly(year, 2, 15)).First(DayOfWeek.Monday), BirthdayOfGeorgeWashington),
                new (Dates.Invariant().From(new DateOnly(year, 5, 25)).First(DayOfWeek.Monday), MemorialDay),
                new(new DateOnly(year, 6, 19), Juneteenth),
                new(new DateOnly(year, 7, 4), IndependenceDay),
                new (Dates.Invariant().From(new DateOnly(year, 9, 1)).First(DayOfWeek.Monday), LabourDay),
                new (Dates.Invariant().From(new DateOnly(year, 10, 8)).First(DayOfWeek.Monday), ColumbusDay),
                new(new DateOnly(year, 11, 11), VeteransDay),
                new (Dates.Invariant().From(new DateOnly(year, 11, 22)).First(DayOfWeek.Thursday), ThanksgivingDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
}