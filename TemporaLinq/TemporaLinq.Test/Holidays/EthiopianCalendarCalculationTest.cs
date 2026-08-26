using FluentAssertions;
using TemporaLinq.Holidays;

namespace TemporaLinq.Test;

public class EthiopianCalendarCalculationTest
{
    // Reference pairs independently cross-checked against a maintained third-party
    // Ethiopian/Gregorian date converter (Python `ethiopian-date-converter`, ported from
    // Ealet 2.0 by the Senamirmir Project), spanning both Ethiopian leap and non-leap
    // years and the boundary where an Ethiopian leap year's 6-day Pagume shifts the
    // following year's New Year from September 11 to September 12.
    [Theory]
    [InlineData(2013, 1, 1, 2020, 9, 11)]
    [InlineData(2013, 1, 17, 2020, 9, 27)]
    [InlineData(2013, 4, 29, 2021, 1, 7)]
    [InlineData(2013, 5, 11, 2021, 1, 19)]
    [InlineData(2013, 6, 23, 2021, 3, 2)]
    [InlineData(2013, 8, 27, 2021, 5, 5)]
    [InlineData(2016, 1, 1, 2023, 9, 12)] // year after an Ethiopian leap year: New Year shifts to Sept 12
    [InlineData(2016, 1, 17, 2023, 9, 28)]
    [InlineData(2016, 4, 29, 2024, 1, 8)]
    [InlineData(2017, 1, 1, 2024, 9, 11)] // shift resets the following Ethiopian year
    [InlineData(2019, 1, 1, 2026, 9, 11)]
    [InlineData(2019, 4, 29, 2027, 1, 7)]
    [InlineData(2019, 5, 11, 2027, 1, 19)]
    [InlineData(2019, 6, 23, 2027, 3, 2)]
    [InlineData(2019, 8, 27, 2027, 5, 5)]
    public void ToGregorian_MatchesIndependentlyVerifiedReferencePairs(
        int ethiopianYear, int ethiopianMonth, int ethiopianDay,
        int gregorianYear, int gregorianMonth, int gregorianDay)
    {
        var result = EthiopianCalendarCalculation.ToGregorian(ethiopianYear, ethiopianMonth, ethiopianDay);

        result.Should().Be(new DateOnly(gregorianYear, gregorianMonth, gregorianDay));
    }

    [Theory]
    [InlineData(2026, 1, 1, 2026, 9, 11)] // 1 Meskerem (Enkutatash, Ethiopian New Year) 2019 falls in Gregorian 2026
    [InlineData(2026, 1, 17, 2026, 9, 27)] // 17 Meskerem (Meskel) 2019
    [InlineData(2026, 4, 29, 2026, 1, 7)] // 29 Tahsas (Genna) of Ethiopian year 2018 falls in Gregorian 2026
    [InlineData(2026, 5, 11, 2026, 1, 19)] // 11 Tir (Timkat) of Ethiopian year 2018
    [InlineData(2026, 6, 23, 2026, 3, 2)] // 23 Yekatit (Adwa Victory Day) of Ethiopian year 2018
    [InlineData(2026, 8, 27, 2026, 5, 5)] // 27 Miazia (Patriots' Victory Day) of Ethiopian year 2018
    public void DateInGregorianYear_ReturnsTheDateFallingWithinTheRequestedGregorianYear(
        int gregorianYear, int ethiopianMonth, int ethiopianDay,
        int expectedYear, int expectedMonth, int expectedDay)
    {
        var result = EthiopianCalendarCalculation.DateInGregorianYear(gregorianYear, ethiopianMonth, ethiopianDay);

        result.Should().Be(new DateOnly(expectedYear, expectedMonth, expectedDay));
        result.Year.Should().Be(gregorianYear);
    }
}
