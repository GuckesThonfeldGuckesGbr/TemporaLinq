namespace TemporaLinq.Astronomy;

/// <summary>
/// Computes the Theravada Buddhist holy days shared by Thailand, Myanmar, Cambodia, and Laos
/// (also known by other local names): Makha Bucha (full moon of the 3rd lunar month), Visakha
/// Bucha/Vesak (full moon of the 6th lunar month), and Asalha Bucha (full moon of the 8th lunar
/// month). Each lunar year's 1st month begins at the most recent new moon on or before the
/// preceding Gregorian year's December solstice - re-anchoring against the solstice every year
/// this way means an earlier year's leap-month insertion (which always occurs after the 8th
/// month, to keep the calendar aligned with the solar year) never has to be tracked forward: it
/// is automatically absorbed by next year's fresh anchor, so no explicit leap-year test is needed
/// to compute months 1-8. This month-numbering approach was derived and verified against
/// independently-published reference dates during design (see
/// docs/superpowers/specs/2026-08-26-southeast-asian-buddhist-calendar-design.md) - it is this
/// project's own synthesis, not a verified third-party port like the underlying moon-phase
/// calculation.
/// </summary>
public static class SoutheastAsianBuddhistCalendar
{
    public static DateOnly MakhaBuchaDate(int gregorianYear) => HolyDayFullMoon(gregorianYear, lunarMonth: 3);

    public static DateOnly VisakhaBuchaDate(int gregorianYear) => HolyDayFullMoon(gregorianYear, lunarMonth: 6);

    public static DateOnly AsalhaBuchaDate(int gregorianYear) => HolyDayFullMoon(gregorianYear, lunarMonth: 8);

    private static DateOnly HolyDayFullMoon(int gregorianYear, int lunarMonth)
    {
        var solstice = DecemberSolsticeCalculation.SolsticeDate(gregorianYear - 1);

        var newMoons = LunarPhaseCalculation.NewMoonsInGregorianYear(gregorianYear - 1)
            .Concat(LunarPhaseCalculation.NewMoonsInGregorianYear(gregorianYear))
            .Concat(LunarPhaseCalculation.NewMoonsInGregorianYear(gregorianYear + 1))
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        var anchor = newMoons.Where(d => d <= solstice).Max();
        var anchorIndex = newMoons.IndexOf(anchor);

        var monthStart = newMoons[anchorIndex + lunarMonth - 1];
        var monthEnd = newMoons[anchorIndex + lunarMonth];

        var fullMoons = LunarPhaseCalculation.FullMoonsInGregorianYear(gregorianYear - 1)
            .Concat(LunarPhaseCalculation.FullMoonsInGregorianYear(gregorianYear))
            .Concat(LunarPhaseCalculation.FullMoonsInGregorianYear(gregorianYear + 1))
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        return fullMoons.First(d => d > monthStart && d < monthEnd);
    }
}
