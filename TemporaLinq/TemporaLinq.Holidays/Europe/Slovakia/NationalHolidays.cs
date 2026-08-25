using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Slovakia;

/// <summary>
/// Provides Slovak national public holidays.
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
                new(new DateOnly(year, 1, 1), EstablishmentDayOfSlovakRepublic),
                new(new DateOnly(year, 1, 6), Epiphany),
                new(easter.AddDays(-2), GoodFriday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 8), VictoryDay),
                new(new DateOnly(year, 7, 5), SaintsCyrilAndMethodiusDay),
                new(new DateOnly(year, 8, 29), SlovakNationalUprisingDay),
                new(new DateOnly(year, 9, 1), ConstitutionDayOfSlovakia),
                new(new DateOnly(year, 9, 15), OurLadyOfSorrowsDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 17), StruggleForFreedomAndDemocracyDay),
                new(new DateOnly(year, 12, 24), ChristmasEve),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
