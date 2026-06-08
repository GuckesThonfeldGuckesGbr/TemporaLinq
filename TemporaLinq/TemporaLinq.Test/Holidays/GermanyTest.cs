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

        holidays.Should().HaveCount(3);
    }

    [Fact]
    public void MultipleYears_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2025, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(18);
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
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31)).ToList();

        var ascensionDay = holidays.First(h => h.Name == AscensionDay);
        var mayFirst = holidays.First(h => h.Name == LabourDay);

        mayFirst.Date.Should().BeBefore(ascensionDay.Date);
    }

    #region State Holiday Tests

    [Fact]
    public void BadenWuerttemberg_HasCorrectHolidays()
    {
        var holidays = BadenWuerttemberg.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(3);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 4) && h.Name == CorpusChristi);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
    }

    [Fact]
    public void BavariaCatholic_HasCorrectHolidays()
    {
        var holidays = BavariaCatholic.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(4);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 4) && h.Name == CorpusChristi);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
    }

    [Fact]
    public void BavariaProtestant_HasCorrectHolidays()
    {
        var holidays = BavariaProtestant.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(3);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 4) && h.Name == CorpusChristi);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
    }

    [Fact]
    public void Augsburg_HasPeaceFestivalHoliday()
    {
        var holidays = Augsburg.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 8) && h.Name == AugsburgPeaceFestival);
    }

    [Fact]
    public void Berlin_HasCorrectHolidays()
    {
        var holidays = Berlin.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(2);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 8) && h.Name == InternationalWomensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 20) && h.Name == WorldChildrensDay);
    }

    [Fact]
    public void Brandenburg_HasCorrectHolidays()
    {
        var holidays = Brandenburg.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 31) && h.Name == ReformationDay);
    }

    [Fact]
    public void Bremen_HasCorrectHolidays()
    {
        var holidays = Bremen.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 31) && h.Name == ReformationDay);
    }

    [Fact]
    public void Hamburg_HasCorrectHolidays()
    {
        var holidays = Hamburg.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 31) && h.Name == ReformationDay);
    }

    [Fact]
    public void Hesse_HasCorrectHolidays()
    {
        var holidays = Hesse.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 4) && h.Name == CorpusChristi);
    }

    [Fact]
    public void LowerSaxony_HasCorrectHolidays()
    {
        var holidays = LowerSaxony.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 31) && h.Name == ReformationDay);
    }

    [Fact]
    public void MecklenburgVorpommern_HasCorrectHolidays()
    {
        var holidays = MecklenburgVorpommern.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(2);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 8) && h.Name == InternationalWomensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 31) && h.Name == ReformationDay);
    }

    [Fact]
    public void NorthRhineWestphalia_HasCorrectHolidays()
    {
        var holidays = NorthRhineWestphalia.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(2);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 4) && h.Name == CorpusChristi);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
    }

    [Fact]
    public void RhinelandPalatinate_HasCorrectHolidays()
    {
        var holidays = RhinelandPalatinate.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(2);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 4) && h.Name == CorpusChristi);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
    }

    [Fact]
    public void Saarland_HasCorrectHolidays()
    {
        var holidays = Saarland.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(3);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 4) && h.Name == CorpusChristi);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
    }

    [Fact]
    public void Saxony_HasCorrectHolidays()
    {
        var holidays = Saxony.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(2);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 31) && h.Name == ReformationDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 18) && h.Name == RepentanceAndPrayerDay);
    }

    [Fact]
    public void SaxonyAnhalt_HasCorrectHolidays()
    {
        var holidays = SaxonyAnhalt.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(2);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 31) && h.Name == ReformationDay);
    }

    [Fact]
    public void SchleswigHolstein_HasCorrectHolidays()
    {
        var holidays = SchleswigHolstein.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 31) && h.Name == ReformationDay);
    }

    [Fact]
    public void Thuringia_HasCorrectHolidays()
    {
        var holidays = Thuringia.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 20) && h.Name == WorldChildrensDay);
    }

    [Theory]
    [InlineData(2024, 11, 20)]
    [InlineData(2025, 11, 19)]
    [InlineData(2026, 11, 18)]
    [InlineData(2027, 11, 17)]
    public void Saxony_RepentanceAndPrayerDay_FallsOnFirstWednesday(int year, int expectedMonth, int expectedDay)
    {
        var holidays = Saxony.Create().From(new DateOnly(year, 1, 1)).To(new DateOnly(year, 12, 31));
        var repentanceDay = holidays.First(h => h.Name == RepentanceAndPrayerDay);

        repentanceDay.Date.Should().Be(new DateOnly(year, expectedMonth, expectedDay));
        repentanceDay.Date.DayOfWeek.Should().Be(DayOfWeek.Wednesday);
    }

    [Fact]
    public void StateHolidays_CanBeQueriedWithLinqExtensions()
    {
        var holidays = BavariaCatholic.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
        
        var juneHolidays = holidays.Where(h => h.Date.Month == 6).ToList();
        
        juneHolidays.Should().HaveCount(1);
        juneHolidays.Should().Contain(h => h.Name == CorpusChristi);
    }

    #endregion
}