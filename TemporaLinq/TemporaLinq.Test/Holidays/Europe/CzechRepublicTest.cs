using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.CzechRepublic;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class CzechRepublicTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(13);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 8) && h.Name == VictoryDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 5) && h.Name == SaintsCyrilAndMethodiusDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 6) && h.Name == JanHusDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 28) && h.Name == CzechStatehoodDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 28) && h.Name == IndependentCzechoslovakStateDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 17) && h.Name == StruggleForFreedomAndDemocracyDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 24) && h.Name == ChristmasEve);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }
}
