namespace TemporaLinq.Astronomy;

/// <summary>
/// Shared Julian Ephemeris Day to Gregorian date conversion (Jean Meeus, "Astronomical
/// Algorithms," 2nd ed., chapter 7), used by every calculation in this project that produces a
/// JDE.
/// </summary>
internal static class JulianDayConversion
{
    public static DateOnly DateFromJde(double jde)
    {
        var jd = jde + 0.5;
        var z = Math.Floor(jd);

        double aValue;
        if (z >= 2299161)
        {
            var alpha = Math.Floor((z - 1867216.25) / 36524.25);
            aValue = z + 1 + alpha - Math.Floor(alpha / 4);
        }
        else
        {
            aValue = z;
        }

        var b = aValue + 1524;
        var c = Math.Floor((b - 122.1) / 365.25);
        var d = Math.Floor(365.25 * c);
        var e = Math.Floor((b - d) / 30.6001);

        var day = (int) (b - d - Math.Floor(30.6001 * e));
        var month = (int) (e < 14 ? e - 1 : e - 13);
        var year = (int) (month > 2 ? c - 4716 : c - 4715);

        return new DateOnly(year, month, day);
    }
}
