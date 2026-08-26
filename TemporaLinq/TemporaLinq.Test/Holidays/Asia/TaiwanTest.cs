using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Taiwan;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class TaiwanTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(12);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 28) && h.Name == PeaceMemorialDayOfTaiwan);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 4) && h.Name == ChildrensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 5) && h.Name == TombSweepingDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 10) && h.Name == NationalDayOfTaiwan);
    }

    [Fact]
    public void GetHolidays_ContainsLunarHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        // Lunar New Year 2026 falls on 2026-02-17, so the 4-day statutory span is
        // Feb 16 (eve), Feb 17 (day 1), Feb 18 (day 2), Feb 19 (day 3).
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 16) && h.Name == LunarNewYearsEve);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 17) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 18) && h.Name == SecondDayOfLunarNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 19) && h.Name == ThirdDayOfLunarNewYear);

        // Dragon Boat Festival 2026.
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 19) && h.Name == DragonBoatFestival);

        // Mid-Autumn Festival 2026.
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 25) && h.Name == MidAutumnFestival);
    }
}
