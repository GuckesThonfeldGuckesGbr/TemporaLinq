namespace TemporaLinq.Astronomy;

/// <summary>
/// Computes the date of the December solstice using Meeus' low-precision algorithm ("Astronomical
/// Algorithms," 2nd ed., chapter 27). Accurate to within a minute of time for 1951-2050, far more
/// precision than the day-level granularity needed by this codebase's consumers (e.g. the
/// solstice-relative leap-month test in <see cref="SoutheastAsianBuddhistCalendar"/>).
/// </summary>
public static class DecemberSolsticeCalculation
{
    private static readonly (double A, double B, double C)[] PeriodicTerms =
    [
        (485, 324.96, 1934.136), (203, 337.23, 32964.467), (199, 342.08, 20.186),
        (182, 27.85, 445267.112), (156, 73.14, 45036.886), (136, 171.52, 22518.443),
        (77, 222.54, 65928.934), (74, 296.72, 3034.906), (70, 243.58, 9037.513),
        (58, 119.81, 33718.147), (52, 297.17, 150.678), (50, 21.02, 2281.226),
        (45, 247.54, 29929.562), (44, 325.15, 31555.956), (29, 60.93, 4443.417),
        (18, 155.12, 67555.328), (17, 288.79, 4562.452), (16, 198.04, 62894.029),
        (14, 199.76, 31436.921), (12, 95.39, 14577.848), (12, 287.11, 31931.756),
        (12, 320.81, 34777.259), (9, 227.73, 1222.114), (8, 15.45, 16859.074),
    ];

    /// <summary>
    /// Returns the Gregorian date of the December solstice in the given Gregorian year.
    /// </summary>
    public static DateOnly SolsticeDate(int gregorianYear)
    {
        var y = (gregorianYear - 2000) * 0.001;
        var jde0 = 2451900.05952 + 365242.74049 * y - 0.06223 * y * y
            - 0.00823 * y * y * y + 0.00032 * y * y * y * y;

        var t = (jde0 - 2451545.0) / 36525.0;
        var w = (35999.373 * t - 2.47) * Math.PI / 180.0;
        var deltaLambda = 1 + 0.0334 * Math.Cos(w) + 0.0007 * Math.Cos(2 * w);

        var s = 0.0;
        foreach (var (a, b, c) in PeriodicTerms)
            s += a * Math.Cos((b + c * t) * Math.PI / 180.0);

        var jde = jde0 + 0.00001 * s / deltaLambda;
        return JulianDayConversion.DateFromJde(jde);
    }
}
