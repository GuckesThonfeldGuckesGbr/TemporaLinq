using System.Globalization;

namespace TemporaLinq.Holidays;

/// <summary>
/// Converts Persian solar calendar dates (Iran's civil calendar) to Gregorian dates using
/// <see cref="System.Globalization.PersianCalendar"/>. The Persian calendar is a solar
/// calendar with its own leap-year rule, so a given (month, day) occurs exactly once per
/// Gregorian year.
/// </summary>
public static class PersianCalendarCalculation
{
    private static readonly PersianCalendar Calendar = new();

    /// <summary>
    /// Returns the single Gregorian date on which the given Persian month/day falls within
    /// the specified Gregorian year.
    /// </summary>
    public static DateOnly DateInGregorianYear(int gregorianYear, int persianMonth, int persianDay)
    {
        var yearStart = new DateOnly(gregorianYear, 1, 1);
        var yearEnd = new DateOnly(gregorianYear, 12, 31);

        var firstPersianYear = Calendar.GetYear(yearStart.ToDateTime(TimeOnly.MinValue));
        var lastPersianYear = Calendar.GetYear(yearEnd.ToDateTime(TimeOnly.MinValue));

        for (var persianYear = firstPersianYear; persianYear <= lastPersianYear; persianYear++)
        {
            var candidate = DateOnly.FromDateTime(
                Calendar.ToDateTime(persianYear, persianMonth, persianDay, 0, 0, 0, 0));

            if (candidate >= yearStart && candidate <= yearEnd)
                return candidate;
        }

        throw new InvalidOperationException(
            $"No Gregorian date found for Persian {persianMonth}/{persianDay} within Gregorian year {gregorianYear}.");
    }
}
