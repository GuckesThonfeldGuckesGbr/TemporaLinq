using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Germany;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays;

public class GermanyTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(9);
    }

    [Fact]
    public void GetHolidays_AreOrderedChronologically()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31)).ToList();

        for (var i = 1; i < holidays.Count; i++)
        {
            holidays[i].Date.Should().BeAfter(holidays[i - 1].Date);
        }
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date.Month == 1 && h.Date.Day == 1 && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date.Month == 5 && h.Date.Day == 1 && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date.Month == 10 && h.Date.Day == 3 && h.Name == DayOfGermanUnity);
        holidays.Should().Contain(h => h.Date.Month == 12 && h.Date.Day == 25 && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date.Month == 12 && h.Date.Day == 26 && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == AscensionDay);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == WhitMonday);
        // holidays.Should().Contain(h => h.Date == easter2026.AddDays(60) && h.Name == "Fronleichnam");
    }

    [Theory]
    [InlineData(2024, 3, 31)]
    [InlineData(2025, 4, 20)]
    [InlineData(2026, 4, 5)]
    [InlineData(2027, 3, 28)]
    [InlineData(2028, 4, 16)]
    public void CalculateEasterSunday_ReturnsCorrectDate(int year, int expectedMonth, int expectedDay)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);

        easter.Year.Should().Be(year);
        easter.Month.Should().Be(expectedMonth);
        easter.Day.Should().Be(expectedDay);
    }

    [Fact]
    public void GetHoliday_ReturnsHoliday_WhenDateIsHoliday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var newYears = holidays.GetHoliday(new DateOnly(2026, 1, 1));

        newYears.Should().NotBeNull();
        newYears.Value.Name.Should().Be(NewYearsDay);
    }

    [Fact]
    public void GetHoliday_ReturnsNull_WhenDateIsNotHoliday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var result = holidays.GetHoliday(new DateOnly(2026, 1, 2));

        result.Should().BeNull();
    }

    [Fact]
    public void TryGetHoliday_ReturnsTrue_WhenDateIsHoliday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var success = holidays.TryGetHoliday(new DateOnly(2026, 1, 1), out var holiday);

        success.Should().BeTrue();
        holiday!.Value.Name.Should().Be(NewYearsDay);
    }

    [Fact]
    public void TryGetHoliday_ReturnsFalse_WhenDateIsNotHoliday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var success = holidays.TryGetHoliday(new DateOnly(2026, 1, 2), out var holiday);

        success.Should().BeFalse();
        holiday.Should().BeNull();
    }

    [Fact]
    public void IsHoliday_ReturnsTrue_WhenDateIsHoliday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.IsHoliday(new DateOnly(2026, 1, 1)).Should().BeTrue();
    }

    [Fact]
    public void IsHoliday_ReturnsFalse_WhenDateIsNotHoliday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.IsHoliday(new DateOnly(2026, 1, 2)).Should().BeFalse();
    }

    [Fact]
    public void DateRange_FiltersHolidaysCorrectly()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 5, 1)).To(new DateOnly(2026, 5, 31));

        holidays.Should().HaveCount(3); // Labor day, ascension day, whit monday 
    }

    [Fact]
    public void MultipleYears_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2025, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(18); // 9 holidays per year
    }

    [Fact]
    public void Holiday_ImplicitlyConvertsToDateOnly()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        var holiday = holidays.GetHoliday(new DateOnly(2026, 1, 1))!;

        DateOnly? date = holiday;
        date.Should().Be(new DateOnly(2026, 1, 1));
    }

    [Fact]
    public void AscensionDay_BeforeMayFirst_WhenEasterIsLate()
    {
        // When Easter is late (e.g., mid-April), Ascension Day (39 days after Easter)
        // can fall after May 1
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31)).ToList();

        var ascensionDay = holidays.First(h => h.Name == AscensionDay);
        var mayFirst = holidays.First(h => h.Name == LabourDay);

        // In 2026, Easter is April 5, Ascension is May 14, so May 1 comes first
        mayFirst.Date.Should().BeBefore(ascensionDay.Date);
    }
}