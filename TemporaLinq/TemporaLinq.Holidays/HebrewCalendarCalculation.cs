using System.Globalization;

namespace TemporaLinq.Holidays;

/// <summary>
/// Converts Hebrew (lunisolar) calendar dates to Gregorian dates using
/// <see cref="System.Globalization.HebrewCalendar"/>. The Hebrew calendar is intercalated
/// (a 13th month, Adar II, is added seven times in every 19-year cycle) specifically to
/// stay aligned with the solar year, so a given Hebrew (month, day) occurs exactly once
/// per Gregorian year. Callers must be aware that in a 13-month leap year, Adar splits
/// into Adar I (month 6) and Adar II (month 7), shifting Nisan and all later months up by
/// one slot relative to a 12-month ordinary year - check
/// <see cref="HebrewCalendar.IsLeapYear"/> on the target Hebrew year before choosing a
/// month number for a specific year.
/// </summary>
public static class HebrewCalendarCalculation
{
    private static readonly HebrewCalendar Calendar = new();

    /// <summary>
    /// Returns the single Gregorian date on which the given Hebrew month/day falls within
    /// the specified Gregorian year.
    /// </summary>
    public static DateOnly DateInGregorianYear(int gregorianYear, int hebrewMonth, int hebrewDay)
    {
        var yearStart = new DateOnly(gregorianYear, 1, 1);
        var yearEnd = new DateOnly(gregorianYear, 12, 31);

        var firstHebrewYear = Calendar.GetYear(yearStart.ToDateTime(TimeOnly.MinValue));
        var lastHebrewYear = Calendar.GetYear(yearEnd.ToDateTime(TimeOnly.MinValue));

        for (var hebrewYear = firstHebrewYear; hebrewYear <= lastHebrewYear; hebrewYear++)
        {
            var candidate = DateOnly.FromDateTime(
                Calendar.ToDateTime(hebrewYear, hebrewMonth, hebrewDay, 0, 0, 0, 0));

            if (candidate >= yearStart && candidate <= yearEnd)
                return candidate;
        }

        throw new InvalidOperationException(
            $"No Gregorian date found for Hebrew {hebrewMonth}/{hebrewDay} within Gregorian year {gregorianYear}.");
    }
}
