using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.SanMarino;

/// <summary>
/// Provides Sammarinese national public holidays.
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
                new(new DateOnly(year, 2, 5), FeastOfSaintAgatha),
                new(new DateOnly(year, 3, 25), AnniversaryOfArengo),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 4, 1), InvestitureOfCaptainsRegent),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 7, 28), FallOfFascismDay),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 9, 3), FoundingOfTheRepublicDay),
                new(new DateOnly(year, 10, 1), InvestitureOfCaptainsRegent),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 2), AllSoulsDay),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
                new(new DateOnly(year, 12, 24), ChristmasEve),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
                new(new DateOnly(year, 12, 31), NewYearsEve),
            }
            .Order()
            .ToImmutableList();
    }
}
