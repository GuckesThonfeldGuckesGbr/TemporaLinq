using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Vietnam;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class VietnamTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(10);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 30) && h.Name == ReunificationDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 2) && h.Name == NationalDayOfVietnam);
    }

    [Fact]
    public void GetHolidays_ContainsLunisolarHolidays()
    {
        // Reference dates independently verified 2026-08-26 against
        // System.Globalization.ChineseLunisolarCalendar (used here as a documented
        // approximation for Vietnam's own lunisolar calendar) and cross-checked
        // against Vietnam-Briefing's published 2026 Tet and Hung Kings schedule.
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 16) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 17) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 18) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 19) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 20) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 26) && h.Name == HungKingsCommemorationDay);
    }
}
