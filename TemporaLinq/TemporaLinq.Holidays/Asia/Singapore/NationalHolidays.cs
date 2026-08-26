using System.Collections.Immutable;
using Memoizer;
using TemporaLinq.Astronomy;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Singapore;

/// <summary>
/// Provides Singaporean public holidays: fixed civil dates, Christian Good Friday
/// (<see cref="EasterSundayCalculation"/>), Chinese New Year
/// (<see cref="ChineseLunisolarCalendarCalculation"/>), the Islamic-calendar Hari Raya Puasa
/// (Eid al-Fitr) and Hari Raya Haji (Eid al-Adha) (<see cref="HijriCalendarCalculation"/> —
/// see that class's documentation for the +/-1, rarely +/-2, day real-world moon-sighting
/// approximation caveat), and Vesak Day
/// (<see cref="SoutheastAsianBuddhistCalendar.VisakhaBuchaDate"/>). Deepavali is deliberately
/// out of scope: it is a Hindu lunisolar-calendar holiday with no calculation mechanism in
/// this codebase yet, deferred pending a future Hindu calendar calculation mechanism (same
/// status as India's and Sri Lanka's Hindu-calendar holidays). Sunday-observed in-lieu
/// Monday shifts (e.g. when Vesak Day, National Day, or Deepavali fall on a Sunday) are also
/// out of scope: they are a per-year administrative gazette decision, not calendar
/// arithmetic.
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
            new(new DateOnly(year, 1, 1), NewYearsDay),
            new(easter.AddDays(-2), GoodFriday),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(year), VesakDay),
            new(new DateOnly(year, 8, 9), IndependenceDay),
            new(new DateOnly(year, 12, 25), ChristmasDay),
        };

        holidays.Add(new(ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 1), LunarNewYearsDay));
        holidays.Add(new(ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 2), LunarNewYearsDay));

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10).Select(d => new Holiday(d, EidAlAdha)));

        return holidays.Order().ToImmutableList();
    }
}
