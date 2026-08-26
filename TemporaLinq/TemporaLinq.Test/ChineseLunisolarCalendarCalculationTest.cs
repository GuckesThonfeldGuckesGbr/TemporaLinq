using FluentAssertions;
using TemporaLinq.Holidays;

namespace TemporaLinq.Test;

public class ChineseLunisolarCalendarCalculationTest
{
    [Fact]
    public void DateInGregorianYear_ReturnsChineseNewYear()
    {
        // Chinese New Year (month 1, day 1) 2024 fell on 2024-02-10.
        var date = ChineseLunisolarCalendarCalculation.DateInGregorianYear(2024, 1, 1);

        date.Should().Be(new DateOnly(2024, 2, 10));
    }

    [Fact]
    public void DateInGregorianYear_HandlesLeapMonthShift_ForMidAutumnFestival()
    {
        // The Chinese lunisolar year spanning Gregorian 2023 had a leap 3rd month
        // (System.Globalization.ChineseLunisolarCalendar.GetLeapMonth(2023) == 3), which
        // shifts every subsequent month up by one slot for that year. The Mid-Autumn
        // Festival (15th day of the 8th lunar month) landed on the .NET-numbered month 9,
        // day 15 - 2023-09-29.
        var calendar = new System.Globalization.ChineseLunisolarCalendar();
        calendar.GetLeapMonth(2023).Should().Be(3);

        var date = ChineseLunisolarCalendarCalculation.DateInGregorianYear(2023, 9, 15);

        date.Should().Be(new DateOnly(2023, 9, 29));
    }

    [Fact]
    public void DateInGregorianYear_OrdinaryYear_UsesUnshiftedMonthNumber()
    {
        // 2024 had no leap month, so the Mid-Autumn Festival (month 8, day 15) uses the
        // unshifted month number and fell on 2024-09-17.
        var calendar = new System.Globalization.ChineseLunisolarCalendar();
        calendar.GetLeapMonth(2024).Should().Be(0);

        var date = ChineseLunisolarCalendarCalculation.DateInGregorianYear(2024, 8, 15);

        date.Should().Be(new DateOnly(2024, 9, 17));
    }
}
