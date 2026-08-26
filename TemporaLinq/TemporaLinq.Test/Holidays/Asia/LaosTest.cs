using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Laos;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class LaosTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 8) && h.Name == InternationalWomensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 14) && h.Name == LaoNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 15) && h.Name == LaoNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 16) && h.Name == LaoNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 2) && h.Name == LaoNationalDay);
    }
}
