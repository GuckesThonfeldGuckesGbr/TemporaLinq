using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.China;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class ChinaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(7);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 1) && h.Name == NationalDayOfChina);
    }

    [Fact]
    public void GetHolidays_ContainsLunisolarAndSolarTermHolidays()
    {
        // Reference dates independently verified 2026-08-26 against System.Globalization.ChineseLunisolarCalendar
        // and cross-checked against China-Briefing's published 2026 holiday schedule.
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 17) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 5) && h.Name == QingmingFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 19) && h.Name == DragonBoatFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 25) && h.Name == MidAutumnFestival);
    }
}
