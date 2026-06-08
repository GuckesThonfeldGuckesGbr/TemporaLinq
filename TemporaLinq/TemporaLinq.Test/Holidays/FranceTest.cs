using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.France;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays;

public class FranceTest
{
    [Fact]
    public void GetHolidays_For2024_ReturnsAllNationalHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().HaveCount(13);
    }
    
    [Fact]
    public void GetHolidays_ContainsNewYearsDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 1, 1) && h.Name == NewYearsDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsGoodFriday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Name == GoodFriday);
    }
    
    [Fact]
    public void GetHolidays_ContainsEasterMonday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 4, 1) && h.Name == EasterMonday);
    }
    
    [Theory]
    [InlineData(2024, 4, 1)]
    [InlineData(2023, 4, 10)]
    [InlineData(2022, 4, 18)]
    public void GetHolidays_EasterMonday_VariesByYear(int year, int month, int day)
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(year, 1, 1)).To(new DateOnly(year, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(year, month, day) && h.Name == EasterMonday);
    }
    
    [Fact]
    public void GetHolidays_ContainsAscensionDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Name == AscensionDay);
    }
    
    [Theory]
    [InlineData(2024, 5, 9)]
    [InlineData(2023, 5, 18)]
    [InlineData(2022, 5, 26)]
    public void GetHolidays_AscensionDay_VariesByYear(int year, int month, int day)
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(year, 1, 1)).To(new DateOnly(year, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(year, month, day) && h.Name == AscensionDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsLabourDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 5, 1) && h.Name == LabourDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsVictoryDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 5, 8) && h.Name == VictoryDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsWhitSunday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Name == WhitSunday);
    }
    
    [Fact]
    public void GetHolidays_ContainsWhitMonday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Name == WhitMonday);
    }
    
    [Theory]
    [InlineData(2024, 5, 20)]
    [InlineData(2023, 5, 29)]
    [InlineData(2022, 6, 6)]
    public void GetHolidays_WhitMonday_VariesByYear(int year, int month, int day)
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(year, 1, 1)).To(new DateOnly(year, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(year, month, day) && h.Name == WhitMonday);
    }
    
    [Fact]
    public void GetHolidays_ContainsBastilleDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 7, 14) && h.Name == BastilleDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsAssumptionDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 8, 15) && h.Name == AssumptionDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsAllSaintsDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 11, 1) && h.Name == AllSaintsDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsArmisticeDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 11, 11) && h.Name == ArmisticeDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsChristmasDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 12, 25) && h.Name == ChristmasDay);
    }
    
    [Fact]
    public void GetHolidays_AreOrderedChronologically()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31)).ToList();

        for (var i = 1; i < holidays.Count; i++)
        {
            holidays[i].Date.Should().BeAfter(holidays[i - 1].Date);
        }
    }
    
    [Fact]
    public void GetHolidays_IsHoliday_ReturnsTrue_WhenDateIsHoliday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.IsHoliday(new DateOnly(2024, 7, 14)).Should().BeTrue();
    }
    
    [Fact]
    public void GetHolidays_IsHoliday_ReturnsFalse_WhenDateIsNotHoliday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.IsHoliday(new DateOnly(2024, 7, 15)).Should().BeFalse();
    }
    
    #region State Holiday Tests
    
    [Fact]
    public void AlsaceMoselle_HasStStephensDay()
    {
        var holidays = AlsaceMoselle.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 12, 26) && h.Name == StStephensDay);
    }
    
    #endregion
}
