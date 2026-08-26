using FluentAssertions;
using TemporaLinq.Holidays;

namespace TemporaLinq.Test;

public class HebrewCalendarCalculationTest
{
    [Fact]
    public void DateInGregorianYear_ReturnsRoshHashanah()
    {
        // Rosh Hashanah (1 Tishrei 5785) fell on 2024-10-03.
        var date = HebrewCalendarCalculation.DateInGregorianYear(2024, 1, 1);

        date.Should().Be(new DateOnly(2024, 10, 3));
    }

    [Fact]
    public void DateInGregorianYear_HandlesLeapYearMonthShift_ForPassover()
    {
        // Hebrew year 5784 (which spans Gregorian 2023-2024) was a 13-month leap year:
        // Adar splits into Adar I (month 6) and Adar II (month 7), so Nisan - normally
        // month 7 - becomes month 8. Passover (15 Nisan) 5784 fell on 2024-04-23.
        var calendar = new System.Globalization.HebrewCalendar();
        calendar.IsLeapYear(5784).Should().BeTrue();

        var date = HebrewCalendarCalculation.DateInGregorianYear(2024, 8, 15);

        date.Should().Be(new DateOnly(2024, 4, 23));
    }

    [Fact]
    public void DateInGregorianYear_NonLeapYear_UsesUnshiftedMonthNumber()
    {
        // Hebrew year 5785 (spanning Gregorian 2024-2025) is a 12-month ordinary year, so
        // Nisan is month 7. Passover (15 Nisan) 5785 fell on 2025-04-13.
        var date = HebrewCalendarCalculation.DateInGregorianYear(2025, 7, 15);

        date.Should().Be(new DateOnly(2025, 4, 13));
    }
}
