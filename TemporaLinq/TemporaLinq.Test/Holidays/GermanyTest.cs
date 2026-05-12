using TemporaLinq.Holidays.Christian;
using FluentAssertions;

namespace TemporaLinq.Test.Holidays;

public class GermanyTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        holidays.Should().HaveCount(10);
    }

    [Fact]
    public void GetHolidays_AreOrderedChronologically()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31)).ToList();
        
        for (var i = 1; i < holidays.Count; i++)
        {
            holidays[i].Date.Should().BeAfter(holidays[i - 1].Date);
        }
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        holidays.Should().Contain(h => h.Date.Month == 1 && h.Date.Day == 1 && h.Name == "Neujahr");
        holidays.Should().Contain(h => h.Date.Month == 5 && h.Date.Day == 1 && h.Name == "Tag der Arbeit");
        holidays.Should().Contain(h => h.Date.Month == 10 && h.Date.Day == 3 && h.Name == "Tag der Deutschen Einheit");
        holidays.Should().Contain(h => h.Date.Month == 12 && h.Date.Day == 25 && h.Name == "Erster Weihnachtsfeiertag");
        holidays.Should().Contain(h => h.Date.Month == 12 && h.Date.Day == 26 && h.Name == "Zweiter Weihnachtsfeiertag");
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = GermanHolidays.CalculateEasterSunday(2026);
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == "Karfreitag");
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == "Ostermontag");
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == "Christi Himmelfahrt");
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == "Pfingstmontag");
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(60) && h.Name == "Fronleichnam");
    }

    [Theory]
    [InlineData(2024, 3, 31)]
    [InlineData(2025, 4, 20)]
    [InlineData(2026, 4, 5)]
    [InlineData(2027, 3, 28)]
    [InlineData(2028, 4, 16)]
    public void CalculateEasterSunday_ReturnsCorrectDate(int year, int expectedMonth, int expectedDay)
    {
        var easter = GermanHolidays.CalculateEasterSunday(year);
        
        easter.Year.Should().Be(year);
        easter.Month.Should().Be(expectedMonth);
        easter.Day.Should().Be(expectedDay);
    }

    [Fact]
    public void GetHoliday_ReturnsHoliday_WhenDateIsHoliday()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        var newYears = holidays.GetHoliday(new DateOnly(2026, 1, 1));
        
        newYears.Should().NotBeNull();
        newYears!.Value.Name.Should().Be("Neujahr");
    }

    [Fact]
    public void GetHoliday_ReturnsNull_WhenDateIsNotHoliday()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        var result = holidays.GetHoliday(new DateOnly(2026, 1, 2));
        
        result.Should().BeNull();
    }

    [Fact]
    public void TryGetHoliday_ReturnsTrue_WhenDateIsHoliday()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        var success = holidays.TryGetHoliday(new DateOnly(2026, 1, 1), out var holiday);
        
        success.Should().BeTrue();
        holiday!.Value.Name.Should().Be("Neujahr");
    }

    [Fact]
    public void TryGetHoliday_ReturnsFalse_WhenDateIsNotHoliday()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        var success = holidays.TryGetHoliday(new DateOnly(2026, 1, 2), out var holiday);
        
        success.Should().BeFalse();
        holiday.Should().BeNull();
    }

    [Fact]
    public void IsHoliday_ReturnsTrue_WhenDateIsHoliday()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        holidays.IsHoliday(new DateOnly(2026, 1, 1)).Should().BeTrue();
    }

    [Fact]
    public void IsHoliday_ReturnsFalse_WhenDateIsNotHoliday()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        holidays.IsHoliday(new DateOnly(2026, 1, 2)).Should().BeFalse();
    }

    [Fact]
    public void DateRange_FiltersHolidaysCorrectly()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 6, 1)).To(new DateOnly(2026, 6, 30));
        
        holidays.Should().HaveCount(1); // Fronleichnam in June 2026
    }

    [Fact]
    public void MultipleYears_ReturnsAllHolidays()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2025, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        holidays.Should().HaveCount(20); // 10 holidays per year
    }

    [Fact]
    public void Holiday_ImplicitlyConvertsToDateOnly()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        var holiday = holidays.GetHoliday(new DateOnly(2026, 1, 1))!;
        
        DateOnly? date = holiday;
        date.Should().Be(new DateOnly(2026, 1, 1));
    }

    [Fact]
    public void AscensionDay_BeforeMayFirst_WhenEasterIsLate()
    {
        // When Easter is late (e.g., mid-April), Ascension Day (39 days after Easter)
        // can fall after May 1
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31)).ToList();
        
        var ascensionDay = holidays.First(h => h.Name == "Christi Himmelfahrt");
        var mayFirst = holidays.First(h => h.Name == "Tag der Arbeit");
        
        // In 2026, Easter is April 5, Ascension is May 14, so May 1 comes first
        mayFirst.Date.Should().BeBefore(ascensionDay.Date);
    }

    [Fact]
    public void WhitMonday_AlwaysBeforeCorpusChristi()
    {
        var holidays = Germany.Holidays.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31)).ToList();
        
        var whitMonday = holidays.First(h => h.Name == "Pfingstmontag");
        var corpusChristi = holidays.First(h => h.Name == "Fronleichnam");
        
        whitMonday.Date.Should().BeBefore(corpusChristi.Date);
    }
}
