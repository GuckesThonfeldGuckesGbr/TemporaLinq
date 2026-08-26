using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Taiwan;

/// <summary>
/// Provides Taiwanese national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var lunarNewYear = TaiwanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 1), 1);
        var dragonBoat = TaiwanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 5), 5);
        var midAutumn = TaiwanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 8), 15);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 2, 28), PeaceMemorialDayOfTaiwan),
                new(lunarNewYear.AddDays(-1), LunarNewYearsEve),
                new(lunarNewYear, LunarNewYearsDay),
                new(lunarNewYear.AddDays(1), SecondDayOfLunarNewYear),
                new(lunarNewYear.AddDays(2), ThirdDayOfLunarNewYear),
                new(new DateOnly(year, 4, 4), ChildrensDay),
                // Fixed by law to April 5 every year rather than tracking the floating
                // Qingming solar term (which ranges April 4-6) - see the doc comment on
                // TaiwanLunisolarCalendarCalculation for the calendar's other caveats.
                new(new DateOnly(year, 4, 5), TombSweepingDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(dragonBoat, DragonBoatFestival),
                new(midAutumn, MidAutumnFestival),
                new(new DateOnly(year, 10, 10), NationalDayOfTaiwan),
            }
            .Order()
            .ToImmutableList();
    }

    /// <summary>
    /// The Taiwanese lunisolar calendar is intercalated with an occasional leap month, which
    /// shifts every subsequent month up by one slot for the rest of that lunisolar year (see
    /// <see cref="TaiwanLunisolarCalendarCalculation"/>'s doc comment). This resolves the
    /// correct .NET-numbered month for a given nominal lunar month within the specified
    /// Gregorian year. Uses <see cref="TaiwanLunisolarCalendar.GetYear"/> to derive the
    /// calendar's native (ROC/Minguo era) year rather than assuming it equals the Gregorian
    /// year.
    /// </summary>
    private static int ShiftedLunarMonth(int year, int lunarMonth)
    {
        var calendar = new TaiwanLunisolarCalendar();
        var nativeYear = calendar.GetYear(new DateTime(year, 7, 1));
        var leapMonth = calendar.GetLeapMonth(nativeYear);
        return leapMonth > 0 && leapMonth <= lunarMonth ? lunarMonth + 1 : lunarMonth;
    }
}
