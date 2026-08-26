using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.India;

/// <summary>
/// Provides India's central-government Gazetted public holidays — the subset of the
/// national holiday calendar that is deterministically formula-computable: three fixed
/// civil days (Republic Day, Independence Day, Gandhi Jayanti), Good Friday (Christian
/// Easter calculation) and Christmas Day, and the four Islamic-calendar holidays that
/// appear on the central Gazetted list (Eid al-Fitr, Eid al-Adha/Bakrid, Muharram,
/// Milad-un-Nabi), computed via <see cref="HijriCalendarCalculation"/>.
///
/// This is a deliberately partial implementation. India's Gazetted holiday list also
/// includes several Hindu-calendar holidays (Diwali, Holi, Dussehra, Raksha Bandhan,
/// Janmashtami, etc.), Buddha Purnima, and Jain/Sikh lunar-date holidays — none of
/// which have an accepted simple arithmetic formula or .NET calendar support, and are
/// therefore out of scope pending a future Hindu/Buddhist calendar calculation
/// mechanism (see docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md).
/// State-specific additions (e.g. Pongal, Onam, Bihu) are likewise out of scope.
///
/// As with <see cref="HijriCalendarCalculation"/> generally, the Islamic-calendar dates
/// here are a tabular approximation; real-world moon-sighting-confirmed dates announced
/// by Indian authorities can differ by +/-1, rarely +/-2, days from the calculated date.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);

        var holidays = new List<Holiday>
        {
            new(new DateOnly(year, 1, 26), RepublicDay),
            new(easter.AddDays(-2), GoodFriday),
            new(new DateOnly(year, 8, 15), IndependenceDay),
            new(new DateOnly(year, 10, 2), GandhiJayanti),
            new(new DateOnly(year, 12, 25), ChristmasDay),
        };

        // 1 Shawwal
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)
            .Select(d => new Holiday(d, EidAlFitr)));
        // 10 Dhu al-Hijjah
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10)
            .Select(d => new Holiday(d, EidAlAdha)));
        // 1 Muharram
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1)
            .Select(d => new Holiday(d, Muharram)));
        // 12 Rabi al-Awwal
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12)
            .Select(d => new Holiday(d, MiladUnNabi)));

        return holidays.Order().ToImmutableList();
    }
}
