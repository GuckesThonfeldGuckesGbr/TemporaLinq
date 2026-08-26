using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.SouthKorea;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class SouthKoreaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(15);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 1) && h.Name == IndependenceMovementDayOfKorea);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 5) && h.Name == ChildrensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 6) && h.Name == MemorialDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == LiberationDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 3) && h.Name == NationalFoundationDayOfKorea);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 9) && h.Name == HangeulDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsLunarHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        // Seollal 2026: Feb 16 (eve), Feb 17 (day), Feb 18 (day after).
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 16) && h.Name == LunarNewYearsEve);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 17) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 18) && h.Name == DayAfterLunarNewYear);

        // Buddha's Birthday 2026.
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 24) && h.Name == BuddhasBirthday);

        // Chuseok 2026: Sep 24 (eve), Sep 25 (day), Sep 26 (day after).
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 24) && h.Name == ChuseokEve);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 25) && h.Name == Chuseok);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 26) && h.Name == DayAfterChuseok);
    }
}
