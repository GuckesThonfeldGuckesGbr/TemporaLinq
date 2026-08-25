using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Ireland;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class IrelandTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 17) && h.Name == StPatricksDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 4) && h.Name == EarlyMayBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 1) && h.Name == JuneBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 3) && h.Name == AugustBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 26) && h.Name == OctoberBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }

    [Theory]
    [InlineData(2026, 2, 2)]
    [InlineData(2029, 2, 5)]
    [InlineData(2030, 2, 1)]
    public void StBrigidsDay_FollowsSpecialFridayRule(int year, int expectedMonth, int expectedDay)
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(year, 1, 1)).To(new DateOnly(year, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(year, expectedMonth, expectedDay) && h.Name == StBrigidsDay);
    }
}
