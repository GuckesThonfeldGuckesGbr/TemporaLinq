using FluentAssertions;
using TemporaLinq.Holidays;

namespace TemporaLinq.Test;

public class KoreanLunisolarCalendarCalculationTest
{
    [Fact]
    public void DateInGregorianYear_ReturnsSeollal()
    {
        // Seollal (Korean Lunar New Year, month 1, day 1) 2024 fell on 2024-02-10 - the
        // same day as Chinese New Year, since both track the same underlying lunisolar
        // system.
        var date = KoreanLunisolarCalendarCalculation.DateInGregorianYear(2024, 1, 1);

        date.Should().Be(new DateOnly(2024, 2, 10));
    }

    [Fact]
    public void DateInGregorianYear_HandlesLeapMonthShift_ForChuseok()
    {
        // The Korean lunisolar year spanning Gregorian 2025 had a leap 7th month
        // (System.Globalization.KoreanLunisolarCalendar.GetLeapMonth(2025) == 7), which
        // shifts every subsequent month up by one slot for that year. Chuseok (15th day of
        // the 8th lunar month) landed on the .NET-numbered month 9, day 15 - 2025-10-06,
        // matching the real-world observed Chuseok 2025 date.
        var calendar = new System.Globalization.KoreanLunisolarCalendar();
        calendar.GetLeapMonth(2025).Should().Be(7);

        var date = KoreanLunisolarCalendarCalculation.DateInGregorianYear(2025, 9, 15);

        date.Should().Be(new DateOnly(2025, 10, 6));
    }

    [Fact]
    public void DateInGregorianYear_OrdinaryYear_UsesUnshiftedMonthNumber()
    {
        // 2024 had no leap month, so Chuseok (month 8, day 15) uses the unshifted month
        // number and fell on 2024-09-17.
        var calendar = new System.Globalization.KoreanLunisolarCalendar();
        calendar.GetLeapMonth(2024).Should().Be(0);

        var date = KoreanLunisolarCalendarCalculation.DateInGregorianYear(2024, 8, 15);

        date.Should().Be(new DateOnly(2024, 9, 17));
    }

    [Fact]
    public void DateInGregorianYear_ReturnsBuddhasBirthday()
    {
        // Buddha's Birthday (month 4, day 8) 2024 fell on 2024-05-15.
        var date = KoreanLunisolarCalendarCalculation.DateInGregorianYear(2024, 4, 8);

        date.Should().Be(new DateOnly(2024, 5, 15));
    }
}
