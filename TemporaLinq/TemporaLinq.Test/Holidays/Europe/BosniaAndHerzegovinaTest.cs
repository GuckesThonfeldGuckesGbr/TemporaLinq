using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.BosniaAndHerzegovina;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class BosniaAndHerzegovinaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(4);
    }

    [Fact]
    public void GetHolidays_ContainsAllFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 2) && h.Name == LabourDay);
    }
}
