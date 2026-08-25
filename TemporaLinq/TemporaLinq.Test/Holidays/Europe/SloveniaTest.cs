using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Slovenia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class SloveniaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(14);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == SecondJanuary);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 8) && h.Name == PresernDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 27) && h.Name == DayOfUprisingAgainstOccupation);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 2) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 25) && h.Name == StatehoodDayOfSlovenia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 31) && h.Name == ReformationDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == IndependenceAndUnityDayOfSlovenia);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }
}
