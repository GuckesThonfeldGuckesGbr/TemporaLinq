namespace TemporaLinq.Holidays;

/// <summary>
/// Converts Mongolian lunisolar calendar dates to Gregorian dates via a direct port of the
/// Phugpa calendar mathematics (Svante Janson, "Tibetan calendar mathematics," 2007, revised
/// 2014: https://www2.math.uu.se/~svantejs/papers/calendars/tibet.pdf), using the
/// Mongolian-specific epoch and reference-time constants of the Tegus Buyantu system devised by
/// the monk Ishbaljir in 1747 for Ulaanbaatar. Ported from the MIT-licensed
/// <c>@hnw/date-tibetan</c> library's <c>CalendarMongolian</c>/<c>CalendarTibetan</c> classes
/// (https://github.com/hnw/date-tibetan), which implement the same paper.
///
/// This is a genuine closed-form calculation - no per-year lookup table - verified against nine
/// independently-sourced real Tsagaan Sar (Mongolian Lunar New Year) dates spanning 2020-2027 and
/// three real Ikh Duichen (Buddha Day) dates spanning 2024-2026: all three Ikh Duichen dates and
/// six of the nine Tsagaan Sar dates matched exactly, the remaining three Tsagaan Sar dates were
/// off by one day. As with this project's Hijri-based calculations, treat the result as accurate
/// to within +/-1 day of the locally observed date.
/// </summary>
public static class MongolianCalendarCalculation
{
    // Epoch constants for the Mongolian (Tegus Buyantu, 1747) variant of the Phugpa system.
    private const double M0 = 2359237.0 + 2603.0 / 2828.0;
    private const double M1 = 167025.0 / 5656.0;
    private const double M2 = 11135.0 / 11312.0;
    private const double S0 = 397.0 / 402.0;
    private const double S1 = 65.0 / 804.0;
    private const double S2 = 13.0 / 4824.0;
    private const double A0 = 1523.0 / 1764.0;
    private const double A1 = 253.0 / 3528.0;
    private const double A2 = 1.0 / 28.0;
    private const double P0 = 209.0 / 270.0;
    private const int EpochYear = 1747;

    private static readonly double[] MoonTable = { 0, 5, 10, 15, 19, 22, 24, 25 };
    private static readonly double[] SunTable = { 0, 6, 10, 11 };

    /// <summary>
    /// Returns the Gregorian date on which the given Mongolian lunisolar month/day falls within
    /// the specified Gregorian year.
    /// </summary>
    public static DateOnly DateInGregorianYear(int gregorianYear, int lunarMonth, int lunarDay)
    {
        foreach (var hintYear in new[] { gregorianYear, gregorianYear - 1, gregorianYear + 1 })
        {
            var date = DateForHintYear(hintYear, lunarMonth, lunarDay);
            if (date.Year == gregorianYear)
                return date;
        }

        throw new InvalidOperationException(
            $"No Gregorian date found for Mongolian lunisolar {lunarMonth}/{lunarDay} within Gregorian year {gregorianYear}.");
    }

    private static DateOnly DateForHintYear(int hintYear, int lunarMonth, int lunarDay)
    {
        var n = TrueMonthCount(hintYear, lunarMonth);
        var jdn = Jdn(n, lunarDay);
        return DateFromJd(jdn - 0.5);
    }

    private static double TrueMonthCount(int gregorianYear, int month)
    {
        var alpha = 12 * (S0 - P0);
        var mPrime = 12 * (gregorianYear - EpochYear) + month;
        return Math.Floor(67 * (mPrime - alpha) / 65);
    }

    private static double TrueDate(double n, double lunarDay)
    {
        var meanDate = n * M1 + lunarDay * M2 + M0;

        var meanSun = n * S1 + lunarDay * S2 + S0;
        meanSun -= Math.Floor(meanSun);

        var anomalyMoon = n * A1 + lunarDay * A2 + A0;
        anomalyMoon -= Math.Floor(anomalyMoon);

        var moonEquation = Interpolate(28 * anomalyMoon, MoonTable, 7, 28);

        var anomalySun = meanSun - 0.25;
        anomalySun -= Math.Floor(anomalySun);

        var sunEquation = Interpolate(12 * anomalySun, SunTable, 3, 12);

        return meanDate + moonEquation / 60 - sunEquation / 60;
    }

    private static double Jdn(double n, int lunarDay)
    {
        var jdn = Math.Floor(TrueDate(n, lunarDay));
        var prevJdn = Math.Floor(TrueDate(n, lunarDay - 1));

        // "A calendar day is labelled by the lunar day that is current at the beginning of the
        // calendar day" (Janson, section 8) - a skipped lunar day pushes the calendar date
        // forward by one.
        if (jdn == prevJdn)
            jdn += 1;

        return jdn;
    }

    private static double Interpolate(double xIn, double[] table, int halfSymmetryLength, int periodLength)
    {
        var x = xIn % periodLength;
        if (x < 0) x += periodLength;

        double sign = 1;
        var symmetryPoint = halfSymmetryLength * 2;
        if (x >= symmetryPoint) { sign = -1; x -= symmetryPoint; }
        if (x > halfSymmetryLength) x = symmetryPoint - x;

        var i = (int) Math.Floor(x);
        var frac = x - i;

        var value = i >= table.Length - 1
            ? table[^1]
            : table[i] * (1 - frac) + table[i + 1] * frac;

        return sign * value;
    }

    // Julian Day -> Gregorian date (Jean Meeus, "Astronomical Algorithms," 2nd ed., ch. 7).
    private static DateOnly DateFromJd(double jd)
    {
        var jdAdjusted = jd + 0.5;
        var z = Math.Floor(jdAdjusted);

        double a;
        if (z >= 2299161)
        {
            var alpha = Math.Floor((z - 1867216.25) / 36524.25);
            a = z + 1 + alpha - Math.Floor(alpha / 4);
        }
        else
        {
            a = z;
        }

        var b = a + 1524;
        var c = Math.Floor((b - 122.1) / 365.25);
        var d = Math.Floor(365.25 * c);
        var e = Math.Floor((b - d) / 30.6001);

        var day = (int) (b - d - Math.Floor(30.6001 * e));
        var month = (int) (e < 14 ? e - 1 : e - 13);
        var year = (int) (month > 2 ? c - 4716 : c - 4715);

        return new DateOnly(year, month, day);
    }
}
