using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Italy;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays;

public class ItalyTest
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
    public void GetHolidays_ContainsEpiphany()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 1, 6) && h.Name == Epiphany);
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
    public void GetHolidays_ContainsLiberationDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 4, 25) && h.Name == LiberationDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsLabourDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 5, 1) && h.Name == LabourDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsRepublicDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 6, 2) && h.Name == RepublicDay);
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
    public void GetHolidays_ContainsImmaculateConception()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 12, 8) && h.Name == ImmaculateConception);
    }
    
    [Fact]
    public void GetHolidays_ContainsChristmasDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 12, 25) && h.Name == ChristmasDay);
    }
    
    [Fact]
    public void GetHolidays_ContainsStStephensDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 12, 26) && h.Name == StStephensDay);
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
    public void GetHolidays_ContainsAscensionDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().Contain(h => h.Name == AscensionDay);
    }
    
    [Fact]
    public void GetHolidays_IsHoliday_ReturnsTrue_WhenDateIsHoliday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.IsHoliday(new DateOnly(2024, 1, 1)).Should().BeTrue();
    }
    
    [Fact]
    public void GetHolidays_IsHoliday_ReturnsFalse_WhenDateIsNotHoliday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.IsHoliday(new DateOnly(2024, 1, 2)).Should().BeFalse();
    }
    
    #region State Holiday Tests
    
    [Fact]
    public void TrentinoAltoAdige_HasWhitsun()
    {
        var holidays = TrentinoAltoAdige.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Name == WhitMonday);
    }
    
    [Fact]
    public void Venice_HasSanMarco()
    {
        var holidays = Venice.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 4, 25) && h.Name == SanMarco);
    }
    
    [Fact]
    public void RomeLazio_HasStPeterAndPaul()
    {
        var holidays = RomeLazio.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 6, 29) && h.Name == StPeterAndPaul);
    }
    
    [Fact]
    public void FlorenceGenoaTurin_HasFeastOfStJohnTheBaptist()
    {
        var holidays = FlorenceGenoaTurin.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 6, 24) && h.Name == FeastOfStJohnTheBaptist);
    }
    
    [Fact]
    public void NaplesCampania_HasFeastOfStJanuarius()
    {
        var holidays = NaplesCampania.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 9, 19) && h.Name == FeastOfStJanuarius);
    }
    
    [Fact]
    public void Bologna_HasFeastOfStPetronius()
    {
        var holidays = Bologna.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 10, 4) && h.Name == FeastOfStPetronius);
    }
    
    [Fact]
    public void Milan_HasStAmbrose()
    {
        var holidays = Milan.Create().From(new DateOnly(2024, 1, 1)).To(new DateOnly(2024, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2024, 12, 7) && h.Name == StAmbrose);
    }
    
    #endregion
}
