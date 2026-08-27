using FluentAssertions;
using TemporaLinq.Holidays;

namespace TemporaLinq.Test;

public class MongolianCalendarCalculationTest
{
    [Theory]
    [InlineData(2020, 2, 24)]
    [InlineData(2021, 2, 12)]
    [InlineData(2023, 2, 21)]
    [InlineData(2024, 2, 10)]
    [InlineData(2025, 3, 1)]
    [InlineData(2026, 2, 18)]
    public void DateInGregorianYear_ReturnsTsagaanSar(int year, int expectedMonth, int expectedDay)
    {
        // Mongolian Lunar New Year (Tsagaan Sar, month 1, day 1), verified via WebSearch against
        // independently-sourced real observed dates.
        var date = MongolianCalendarCalculation.DateInGregorianYear(year, 1, 1);

        date.Should().Be(new DateOnly(year, expectedMonth, expectedDay));
    }

    [Theory]
    [InlineData(2024, 5, 23)]
    [InlineData(2025, 6, 11)]
    [InlineData(2026, 5, 31)]
    public void DateInGregorianYear_ReturnsIkhDuichen(int year, int expectedMonth, int expectedDay)
    {
        // Ikh Duichen (Buddha Day, month 4, day 15), verified via WebSearch against
        // independently-sourced real observed dates.
        var date = MongolianCalendarCalculation.DateInGregorianYear(year, 4, 15);

        date.Should().Be(new DateOnly(year, expectedMonth, expectedDay));
    }
}
