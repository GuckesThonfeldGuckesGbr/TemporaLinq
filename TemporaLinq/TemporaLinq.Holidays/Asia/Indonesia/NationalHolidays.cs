using System.Collections.Immutable;
using Memoizer;
using TemporaLinq.Astronomy;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Indonesia;

/// <summary>
/// Provides Indonesia's national public holidays (hari libur nasional): fixed civil dates,
/// Christian Good Friday and Ascension Day (<see cref="EasterSundayCalculation"/>), Chinese
/// New Year/Imlek (<see cref="ChineseLunisolarCalendarCalculation"/>), the Islamic-calendar
/// Eid al-Fitr, Eid al-Adha, Islamic New Year, and Mawlid Nabi
/// (<see cref="HijriCalendarCalculation"/> — see that class's documentation for the +/-1,
/// rarely +/-2, day real-world moon-sighting approximation caveat), and Waisak/Vesak
/// (<see cref="SoutheastAsianBuddhistCalendar.VisakhaBuchaDate"/>). Nyepi (the Balinese Saka
/// lunisolar new year) and the Hindu-calendar Deepavali are deliberately out of scope: Nyepi
/// requires the Balinese Saka calendar (a distinct calculation mechanism from this design's
/// scope, deferred to a future phase), and Deepavali needs a Hindu lunisolar calendar with no
/// calculation mechanism in this codebase yet. Government-announced "cuti bersama" (joint
/// collective leave) days bridging holidays and weekends are also out of scope: they are a
/// per-year administrative decision, not calendar arithmetic.
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
            new(easter.AddDays(39), AscensionDay),
            new(SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(year), VesakDay),
            new(new DateOnly(year, 8, 17), IndependenceDay),
            new(new DateOnly(year, 12, 25), ChristmasDay),
        };

        holidays.Add(new(ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 1), LunarNewYearsDay));

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1).Select(d => new Holiday(d, IslamicNewYear)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12).Select(d => new Holiday(d, ProphetsBirthday)));

        return holidays.Order().ToImmutableList();
    }
}
