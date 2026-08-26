using FluentAssertions;
using TemporaLinq.Holidays;

namespace TemporaLinq.Test;

public class HijriCalendarCalculationTest
{
    [Fact]
    public void DatesInGregorianYear_ReturnsOneDateInAnOrdinaryYear()
    {
        // 1 Ramadan 1445 AH falls on 2024-03-10 per the tabular Hijri calendar
        // (System.Globalization.HijriCalendar); real-world moon-sighting announcements
        // in various countries landed on 2024-03-11 or 2024-03-12, within the documented
        // +/-1 day approximation.
        var dates = HijriCalendarCalculation.DatesInGregorianYear(2024, 9, 1).ToList();

        dates.Should().ContainSingle().Which.Should().Be(new DateOnly(2024, 3, 10));
    }

    [Fact]
    public void DatesInGregorianYear_ReturnsTwoDatesWhenHijriNewYearDriftsTwiceIntoOneGregorianYear()
    {
        // Confirmed empirically against System.Globalization.HijriCalendar: Gregorian 2008
        // contains two occurrences of 1 Muharram (Hijri New Year), because the ~354-day
        // Hijri year is shorter than the Gregorian year and periodically drifts enough to
        // repeat within one Gregorian year.
        var dates = HijriCalendarCalculation.DatesInGregorianYear(2008, 1, 1).ToList();

        dates.Should().BeEquivalentTo(new[] { new DateOnly(2008, 1, 9), new DateOnly(2008, 12, 28) });
    }

    [Fact]
    public void DatesInGregorianYear_NeverReturnsZeroDates_AcrossATwoCenturySpan()
    {
        // Because the Hijri year is shorter than the Gregorian year, every Gregorian year
        // contains at least one occurrence of any fixed Hijri (month, day) - the drift only
        // ever produces doubles, never a skipped year.
        for (var year = 1925; year <= 2125; year++)
        {
            HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1).Should().NotBeEmpty();
        }
    }
}
