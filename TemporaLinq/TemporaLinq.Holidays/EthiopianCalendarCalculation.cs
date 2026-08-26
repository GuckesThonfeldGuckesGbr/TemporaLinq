namespace TemporaLinq.Holidays;

/// <summary>
/// Converts Ethiopian calendar dates to Gregorian dates. The Ethiopian calendar has 13
/// months (12 of 30 days, plus a short 13th month, Pagume, of 5 days in an ordinary year
/// or 6 days in an Ethiopian leap year) and runs roughly 7-8 years behind the Gregorian
/// calendar. There is no <c>System.Globalization</c> support for it, so this is a small
/// custom day-number-offset calculation: an Ethiopian leap year (one whose number is a
/// multiple of 4) adds one intercalary day, exactly like the Julian calendar's leap rule,
/// just on a different epoch and phase.
///
/// The epoch constant and leap-year alignment used here were independently verified
/// against a maintained third-party reference implementation (Python
/// <c>ethiopian-date-converter</c>, a port of Ealet 2.0 by the Senamirmir Project) across
/// 36 (Ethiopian date, Gregorian date) reference pairs spanning 7 different Gregorian
/// years, both Ethiopian leap and non-leap years, and the Pagume month-13 boundary - all
/// 36 matched exactly. A scripted sweep additionally confirmed zero gaps or duplicate
/// matches for the specific (month, day) pairs this library's Ethiopia holidays use,
/// across Gregorian years 1900-2200. See EthiopianCalendarCalculationTest and
/// docs/superpowers/plans/2026-08-26-africa-holidays.md for the full writeup.
/// </summary>
public static class EthiopianCalendarCalculation
{
    // Day-number offset (in .NET's DateOnly.DayNumber convention, where day 0 is
    // 0001-01-01) for Ethiopian year 1, month 1, day 1.
    private const int EpochDayNumber = 2795;

    /// <summary>
    /// Converts an Ethiopian (year, month, day) to the corresponding Gregorian date.
    /// </summary>
    public static DateOnly ToGregorian(int ethiopianYear, int ethiopianMonth, int ethiopianDay)
    {
        var dayNumber = EpochDayNumber
            + 365 * (ethiopianYear - 1)
            + ethiopianYear / 4
            + 30 * (ethiopianMonth - 1)
            + (ethiopianDay - 1);

        return DateOnly.FromDayNumber(dayNumber);
    }

    /// <summary>
    /// Returns the single Gregorian date on which the given Ethiopian month/day falls
    /// within the specified Gregorian year. Ethiopian and Gregorian years both run ~365.25
    /// days on average and stay in step, so (like <see cref="HebrewCalendarCalculation"/>
    /// and <see cref="PersianCalendarCalculation"/>) a given Ethiopian (month, day) occurs
    /// exactly once per Gregorian year.
    /// </summary>
    public static DateOnly DateInGregorianYear(int gregorianYear, int ethiopianMonth, int ethiopianDay)
    {
        for (var ethiopianYear = gregorianYear - 9; ethiopianYear <= gregorianYear - 6; ethiopianYear++)
        {
            var candidate = ToGregorian(ethiopianYear, ethiopianMonth, ethiopianDay);
            if (candidate.Year == gregorianYear)
                return candidate;
        }

        throw new InvalidOperationException(
            $"No Gregorian date found for Ethiopian {ethiopianMonth}/{ethiopianDay} within Gregorian year {gregorianYear}.");
    }
}
