using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Africa.Ethiopia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Africa;

public class EthiopiaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(8);
    }

    [Fact]
    public void GetHolidays_ContainsEthiopianCalendarHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h =>
            h.Date == EthiopianCalendarCalculation.DateInGregorianYear(2026, 1, 1) && h.Name == EthiopianNewYear);
        holidays.Should().Contain(h =>
            h.Date == EthiopianCalendarCalculation.DateInGregorianYear(2026, 1, 17) && h.Name == FindingOfTheTrueCross);
        holidays.Should().Contain(h =>
            h.Date == EthiopianCalendarCalculation.DateInGregorianYear(2026, 4, 29) && h.Name == ChristmasDay);
        holidays.Should().Contain(h =>
            h.Date == EthiopianCalendarCalculation.DateInGregorianYear(2026, 5, 11) && h.Name == Epiphany);
        holidays.Should().Contain(h =>
            h.Date == EthiopianCalendarCalculation.DateInGregorianYear(2026, 6, 23) && h.Name == AdwaVictoryDay);
        holidays.Should().Contain(h =>
            h.Date == EthiopianCalendarCalculation.DateInGregorianYear(2026, 8, 27) && h.Name == PatriotsVictoryDay);
    }

    [Fact]
    public void GetHolidays_ContainsOrthodoxEasterFeasts()
    {
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
    }
}
