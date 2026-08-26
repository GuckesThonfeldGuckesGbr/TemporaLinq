namespace TemporaLinq.Astronomy;

/// <summary>
/// Computes lunar phase events using Meeus' truncated astronomical algorithm (a periodic-term
/// series derived from lunar theory, not a linear approximation - see Jean Meeus, "Astronomical
/// Algorithms," 2nd ed., chapter 49). Accurate to well under a minute for the foreseeable past
/// and future (roughly +/-200-300 years of the present); accuracy slowly degrades many centuries
/// further out because of growing uncertainty in Delta-T (the gap between Terrestrial Time and
/// Earth's actual, slightly irregular rotation). This implementation does not apply a Delta-T
/// correction, since it is at most a few minutes across this codebase's practical year range -
/// far smaller than the day-level granularity being computed here.
/// </summary>
public static class LunarPhaseCalculation
{
    private static readonly double[] FullMoonPeriodicTerms =
    [
        -0.40614, 0.17302, 0.01614, 0.01043, 0.00734, -0.00515, 0.00209, -0.00111,
        -0.00057, 0.00056, -0.00042, 0.00042, 0.00038, -0.00024, -0.00017, -0.00007,
        0.00004, 0.00004, 0.00003, 0.00003, -0.00003, 0.00003, -0.00002, -0.00002, 0.00002,
    ];

    private static readonly double[] AdditionalCorrectionCoefficients =
    [
        0.000325, 0.000165, 0.000164, 0.000126, 0.00011, 0.000062, 0.00006,
        0.000056, 0.000047, 0.000042, 0.00004, 0.000037, 0.000035, 0.000023,
    ];

    /// <summary>
    /// Returns the Gregorian date of every full moon that falls within the given Gregorian year
    /// (typically 12, occasionally 13).
    /// </summary>
    public static IEnumerable<DateOnly> FullMoonsInGregorianYear(int gregorianYear)
    {
        // Scan lunation numbers k covering the target year with a one-lunation margin on each
        // side, since a full moon computed from a "nearest to decimal year" k can land just
        // outside the target year.
        var approximateK = (gregorianYear - 2000) * 12.3685;
        var startK = Math.Floor(approximateK) - 2;
        var endK = Math.Ceiling(approximateK) + 14;

        for (var k = startK + 0.5; k <= endK; k += 1.0)
        {
            var date = DateFromJde(FullMoonJde(k));
            if (date.Year == gregorianYear)
                yield return date;
        }
    }

    private static double FullMoonJde(double k)
    {
        var t = k / 1236.85;
        var jdeMean = 2451550.09766 + 29.530588861 * k
            + 0.00015437 * t * t
            - 0.00000015 * t * t * t
            + 0.00000000073 * t * t * t * t;

        var e = 1 - 0.002516 * t - 0.0000074 * t * t;

        double Deg(double degrees) => degrees * Math.PI / 180.0;

        var m = Deg(2.5534 + 29.1053567 * k - 0.0000014 * t * t - 0.00000011 * t * t * t);
        var mPrime = Deg(201.5643 + 385.81693528 * k + 0.0107582 * t * t + 0.00001238 * t * t * t
            - 0.000000058 * t * t * t * t);
        var f = Deg(160.7108 + 390.67050284 * k - 0.0016118 * t * t - 0.00000227 * t * t * t
            + 0.000000011 * t * t * t * t);
        var omega = Deg(124.7746 - 1.56375588 * k + 0.0020672 * t * t + 0.00000215 * t * t * t);

        var fc = FullMoonPeriodicTerms;
        var correction =
            fc[0] * Math.Sin(mPrime) + fc[1] * Math.Sin(m) * e + fc[2] * Math.Sin(2 * mPrime)
            + fc[3] * Math.Sin(2 * f) + fc[4] * Math.Sin(mPrime - m) * e
            + fc[5] * Math.Sin(mPrime + m) * e + fc[6] * Math.Sin(2 * m) * e * e
            + fc[7] * Math.Sin(mPrime - 2 * f) + fc[8] * Math.Sin(mPrime + 2 * f)
            + fc[9] * Math.Sin(2 * mPrime + m) * e + fc[10] * Math.Sin(3 * mPrime)
            + fc[11] * Math.Sin(m + 2 * f) * e + fc[12] * Math.Sin(m - 2 * f) * e
            + fc[13] * Math.Sin(2 * mPrime - m) * e + fc[14] * Math.Sin(omega)
            + fc[15] * Math.Sin(mPrime + 2 * m) + fc[16] * Math.Sin(2 * (mPrime - f))
            + fc[17] * Math.Sin(3 * m) + fc[18] * Math.Sin(mPrime + m - 2 * f)
            + fc[19] * Math.Sin(2 * (mPrime + f)) + fc[20] * Math.Sin(mPrime + m + 2 * f)
            + fc[21] * Math.Sin(mPrime - m + 2 * f) + fc[22] * Math.Sin(mPrime - m - 2 * f)
            + fc[23] * Math.Sin(3 * mPrime + m) + fc[24] * Math.Sin(4 * mPrime);

        double[] a =
        [
            Deg(299.7 + 0.107408 * k - 0.009173 * t * t),
            Deg(251.88 + 0.016321 * k),
            Deg(251.83 + 26.651886 * k),
            Deg(349.42 + 36.412478 * k),
            Deg(84.66 + 18.206239 * k),
            Deg(141.74 + 53.303771 * k),
            Deg(207.17 + 2.453732 * k),
            Deg(154.84 + 7.30686 * k),
            Deg(34.52 + 27.261239 * k),
            Deg(207.19 + 0.121824 * k),
            Deg(291.34 + 1.844379 * k),
            Deg(161.72 + 24.198154 * k),
            Deg(239.56 + 25.513099 * k),
            Deg(331.55 + 3.592518 * k),
        ];

        var additional = 0.0;
        for (var i = 0; i < a.Length; i++)
            additional += AdditionalCorrectionCoefficients[i] * Math.Sin(a[i]);

        return jdeMean + correction + additional;
    }

    private static DateOnly DateFromJde(double jde)
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
