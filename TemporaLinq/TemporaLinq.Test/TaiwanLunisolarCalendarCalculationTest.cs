using FluentAssertions;
using TemporaLinq.Holidays;

namespace TemporaLinq.Test;

public class TaiwanLunisolarCalendarCalculationTest
{
    [Fact]
    public void DateInGregorianYear_ReturnsLunarNewYear()
    {
        // Taiwan's Lunar New Year (month 1, day 1) 2024 fell on 2024-02-10 - the same day
        // as Chinese/Korean New Year, since all three track the same underlying lunisolar
        // system (Taiwan just numbers years using the ROC/Minguo era internally).
        var date = TaiwanLunisolarCalendarCalculation.DateInGregorianYear(2024, 1, 1);

        date.Should().Be(new DateOnly(2024, 2, 10));
    }

    [Fact]
    public void DateInGregorianYear_UsesRocEraYearNumberingInternally()
    {
        // Confirms the ROC (Minguo) era offset noted in the doc comment: the calendar's
        // native year for a date in Gregorian 2024 is 113 (2024 - 1911), not 2024.
        var calendar = new System.Globalization.TaiwanLunisolarCalendar();

        calendar.GetYear(new DateTime(2024, 2, 10)).Should().Be(113);
    }

    [Fact]
    public void DateInGregorianYear_HandlesLeapMonthShift_ForMidAutumnFestival()
    {
        // The Taiwanese lunisolar year spanning Gregorian 2025 (native ROC year 114) had a
        // leap 7th month (TaiwanLunisolarCalendar.GetLeapMonth(114) == 7), which shifts
        // every subsequent month up by one slot for that year. The Mid-Autumn Festival
        // (15th day of the 8th lunar month) landed on the .NET-numbered month 9, day 15 -
        // 2025-10-06.
        var calendar = new System.Globalization.TaiwanLunisolarCalendar();
        calendar.GetLeapMonth(114).Should().Be(7);

        var date = TaiwanLunisolarCalendarCalculation.DateInGregorianYear(2025, 9, 15);

        date.Should().Be(new DateOnly(2025, 10, 6));
    }

    [Fact]
    public void DateInGregorianYear_OrdinaryYear_UsesUnshiftedMonthNumber()
    {
        // 2024 (native ROC year 113) had no leap month, so the Mid-Autumn Festival (month 8,
        // day 15) uses the unshifted month number and fell on 2024-09-17.
        var calendar = new System.Globalization.TaiwanLunisolarCalendar();
        calendar.GetLeapMonth(113).Should().Be(0);

        var date = TaiwanLunisolarCalendarCalculation.DateInGregorianYear(2024, 8, 15);

        date.Should().Be(new DateOnly(2024, 9, 17));
    }

    [Fact]
    public void DateInGregorianYear_ReturnsDragonBoatFestival()
    {
        // Dragon Boat Festival (month 5, day 5) 2026 fell on 2026-06-19.
        var date = TaiwanLunisolarCalendarCalculation.DateInGregorianYear(2026, 5, 5);

        date.Should().Be(new DateOnly(2026, 6, 19));
    }
}
