using System.Globalization;

namespace TemporaLinq.Holidays;

/// <summary>
/// Converts Taiwanese lunisolar calendar dates to Gregorian dates using
/// <see cref="System.Globalization.TaiwanLunisolarCalendar"/> (accurate for Gregorian
/// years 1912-2051, backed by the framework's precomputed astronomical data rather than a
/// closed-form formula - zero maintenance burden for this codebase either way). The
/// calendar is intercalated to stay aligned with the solar year, so a given (month, day)
/// occurs exactly once per Gregorian year. Callers must be aware that in a leap year, every
/// month after the leap month (per <see cref="TaiwanLunisolarCalendar.GetLeapMonth"/>) is
/// shifted up by one slot relative to an ordinary year - check <c>GetLeapMonth</c> on the
/// target lunisolar year before choosing a month number for a specific year. Note that this
/// calendar's internal year numbering is the ROC (Minguo) era (Gregorian year minus 1911),
/// not the raw Gregorian year - always derive the native year via <c>GetYear</c> rather than
/// assuming it equals the Gregorian year.
/// </summary>
public static class TaiwanLunisolarCalendarCalculation
{
    private static readonly TaiwanLunisolarCalendar Calendar = new();

    /// <summary>
    /// Returns the single Gregorian date on which the given lunisolar month/day falls
    /// within the specified Gregorian year.
    /// </summary>
    public static DateOnly DateInGregorianYear(int gregorianYear, int lunisolarMonth, int lunisolarDay)
    {
        var yearStart = new DateOnly(gregorianYear, 1, 1);
        var yearEnd = new DateOnly(gregorianYear, 12, 31);

        var firstLunisolarYear = Calendar.GetYear(yearStart.ToDateTime(TimeOnly.MinValue));
        var lastLunisolarYear = Calendar.GetYear(yearEnd.ToDateTime(TimeOnly.MinValue));

        for (var lunisolarYear = firstLunisolarYear; lunisolarYear <= lastLunisolarYear; lunisolarYear++)
        {
            var candidate = DateOnly.FromDateTime(
                Calendar.ToDateTime(lunisolarYear, lunisolarMonth, lunisolarDay, 0, 0, 0, 0));

            if (candidate >= yearStart && candidate <= yearEnd)
                return candidate;
        }

        throw new InvalidOperationException(
            $"No Gregorian date found for Taiwanese lunisolar {lunisolarMonth}/{lunisolarDay} within Gregorian year {gregorianYear}.");
    }
}
