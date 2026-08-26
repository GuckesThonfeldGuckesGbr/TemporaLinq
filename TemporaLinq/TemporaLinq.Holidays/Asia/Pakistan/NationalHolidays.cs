using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Pakistan;

/// <summary>
/// Provides Pakistani national public holidays. The Islamic-calendar holidays
/// (Eid al-Fitr, Eid al-Adha, Ashura, Eid Milad-un-Nabi) are computed via
/// <see cref="HijriCalendarCalculation"/> — see that class's documentation for
/// the +/-1, rarely +/-2, day real-world moon-sighting approximation caveat.
/// December 25 is gazetted as Quaid-e-Azam Day (Muhammad Ali Jinnah's
/// birthday), which coincides with Christmas Day.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var holidays = new List<Holiday>
        {
            new(new DateOnly(year, 2, 5), KashmirSolidarityDay),
            new(new DateOnly(year, 3, 23), PakistanDay),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(new DateOnly(year, 8, 14), IndependenceDay),
            new(new DateOnly(year, 11, 9), IqbalDay),
            new(new DateOnly(year, 12, 25), QuaidEAzamDay),
        };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 3).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 11).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 12).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 1, 9).Select(d => new Holiday(d, AshuraDay)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 1, 10).Select(d => new Holiday(d, AshuraDay)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12).Select(d => new Holiday(d, MiladUnNabi)));

        return holidays.Order().ToImmutableList();
    }
}
