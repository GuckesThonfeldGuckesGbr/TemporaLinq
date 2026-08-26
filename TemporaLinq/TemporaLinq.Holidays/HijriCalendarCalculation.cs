using System.Globalization;

namespace TemporaLinq.Holidays;

/// <summary>
/// Converts Hijri (Islamic lunar) calendar dates to Gregorian dates using the tabular
/// (arithmetic) Hijri calendar implemented by <see cref="System.Globalization.HijriCalendar"/>.
/// This is a deterministic approximation: real-world government/religious-authority
/// announcements of Islamic holidays (especially Eid al-Fitr and Eid al-Adha, which some
/// countries confirm only by moon-sighting the night before) can differ from this
/// calculation by +/-1, rarely +/-2, days.
/// </summary>
public static class HijriCalendarCalculation
{
    private static readonly HijriCalendar Calendar = new();

    /// <summary>
    /// Returns the Gregorian date(s) on which the given Hijri month/day falls within the
    /// specified Gregorian year. Always at least one date; occasionally two, because a
    /// Hijri year (~354 days) is shorter than the Gregorian year and periodically drifts
    /// enough to repeat within one Gregorian year (never zero, for the same reason).
    /// </summary>
    public static IEnumerable<DateOnly> DatesInGregorianYear(int gregorianYear, int hijriMonth, int hijriDay)
    {
        var yearStart = new DateOnly(gregorianYear, 1, 1);
        var yearEnd = new DateOnly(gregorianYear, 12, 31);

        var firstHijriYear = Calendar.GetYear(yearStart.ToDateTime(TimeOnly.MinValue));
        var lastHijriYear = Calendar.GetYear(yearEnd.ToDateTime(TimeOnly.MinValue));

        for (var hijriYear = firstHijriYear; hijriYear <= lastHijriYear; hijriYear++)
        {
            var candidate = DateOnly.FromDateTime(
                Calendar.ToDateTime(hijriYear, hijriMonth, hijriDay, 0, 0, 0, 0));

            if (candidate >= yearStart && candidate <= yearEnd)
                yield return candidate;
        }
    }
}
