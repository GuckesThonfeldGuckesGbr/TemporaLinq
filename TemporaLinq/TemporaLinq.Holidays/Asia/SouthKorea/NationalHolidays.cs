using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.SouthKorea;

/// <summary>
/// Provides South Korean national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var seollal = KoreanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 1), 1);
        var buddhasBirthday = KoreanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 4), 8);
        var chuseok = KoreanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 8), 15);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(seollal.AddDays(-1), LunarNewYearsEve),
                new(seollal, LunarNewYearsDay),
                new(seollal.AddDays(1), DayAfterLunarNewYear),
                new(new DateOnly(year, 3, 1), IndependenceMovementDayOfKorea),
                new(new DateOnly(year, 5, 5), ChildrensDay),
                new(buddhasBirthday, BuddhasBirthday),
                new(new DateOnly(year, 6, 6), MemorialDay),
                new(new DateOnly(year, 8, 15), LiberationDay),
                new(chuseok.AddDays(-1), ChuseokEve),
                new(chuseok, Chuseok),
                new(chuseok.AddDays(1), DayAfterChuseok),
                new(new DateOnly(year, 10, 3), NationalFoundationDayOfKorea),
                new(new DateOnly(year, 10, 9), HangeulDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }

    /// <summary>
    /// The Korean lunisolar calendar is intercalated with an occasional leap month, which
    /// shifts every subsequent month up by one slot for the rest of that lunisolar year (see
    /// <see cref="KoreanLunisolarCalendarCalculation"/>'s doc comment). This resolves the
    /// correct .NET-numbered month for a given nominal lunar month within the specified
    /// Gregorian year.
    /// </summary>
    private static int ShiftedLunarMonth(int year, int lunarMonth)
    {
        var calendar = new KoreanLunisolarCalendar();
        var nativeYear = calendar.GetYear(new DateTime(year, 7, 1));
        var leapMonth = calendar.GetLeapMonth(nativeYear);
        return leapMonth > 0 && leapMonth <= lunarMonth ? lunarMonth + 1 : lunarMonth;
    }
}
