using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.UnitedKingdom;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class UnitedKingdomTest
{
    [Fact]
    public void NationalHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(8);
    }

    [Fact]
    public void NationalHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 4) && h.Name == EarlyMayBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 25) && h.Name == SpringBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 31) && h.Name == SummerBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == BoxingDay);
    }

    [Fact]
    public void NationalHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }

    [Fact]
    public void Scotland_HasCorrectHolidays()
    {
        var holidays = Scotland.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(2);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == SecondJanuary);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 30) && h.Name == StAndrewsDay);
    }

    [Fact]
    public void NorthernIreland_HasCorrectHolidays()
    {
        var holidays = NorthernIreland.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(2);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 17) && h.Name == StPatricksDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 12) && h.Name == BattleOfTheBoyneDay);
    }
}
