using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Israel;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class IsraelTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(10);
    }

    [Fact]
    public void GetHolidays_ContainsHebrewCalendarHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 12) && h.Name == RoshHashanah);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 13) && h.Name == RoshHashanah);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 21) && h.Name == YomKippur);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 26) && h.Name == Sukkot);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 3) && h.Name == SimchatTorah);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 2) && h.Name == Passover);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 8) && h.Name == Passover);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 14) && h.Name == YomHaShoah);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 22) && h.Name == YomHaAtzmaut);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 22) && h.Name == Shavuot);
    }
}
