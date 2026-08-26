using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Africa.Ethiopia;

/// <summary>
/// Provides Ethiopian national public holidays.
/// </summary>
/// <remarks>
/// Enkutatash (Ethiopian New Year), Meskel, Genna (Ethiopian Christmas), Timkat (Ethiopian
/// Epiphany), Adwa Victory Day, and Patriots' Victory Day are all fixed dates on the
/// Ethiopian calendar, converted via <see cref="EthiopianCalendarCalculation"/>. Good
/// Friday and Easter fall on the Ethiopian Orthodox Tewahedo Church's Easter, computed via
/// <see cref="EasterSundayCalculation.ChristianOrthodox"/> - the same Julian-calendar
/// computus used by Eastern Orthodox and Coptic Orthodox churches.
/// </remarks>
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
                new(EthiopianCalendarCalculation.DateInGregorianYear(year, 1, 1), EthiopianNewYear),
                new(EthiopianCalendarCalculation.DateInGregorianYear(year, 1, 17), FindingOfTheTrueCross),
                new(EthiopianCalendarCalculation.DateInGregorianYear(year, 4, 29), ChristmasDay),
                new(EthiopianCalendarCalculation.DateInGregorianYear(year, 5, 11), Epiphany),
                new(EthiopianCalendarCalculation.DateInGregorianYear(year, 6, 23), AdwaVictoryDay),
                new(EthiopianCalendarCalculation.DateInGregorianYear(year, 8, 27), PatriotsVictoryDay),
                new(orthodoxEaster.AddDays(-2), GoodFriday),
                new(orthodoxEaster, EasterSunday),
            }
            .Order()
            .ToImmutableList();
    }
}
