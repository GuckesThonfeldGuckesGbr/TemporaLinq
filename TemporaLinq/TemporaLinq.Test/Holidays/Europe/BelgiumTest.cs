using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Belgium;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class BelgiumTest
{
    [Fact]
    public void NationalHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(10);
    }

    [Fact]
    public void NationalHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 21) && h.Name == NationalDayOfBelgium);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 11) && h.Name == ArmisticeDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void NationalHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == AscensionDay);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == WhitMonday);
    }

    [Fact]
    public void FlemishCommunity_HasCorrectHoliday()
    {
        var holidays = FlemishCommunity.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 11) && h.Name == FlemishCommunityDay);
    }

    [Fact]
    public void FrenchCommunity_HasCorrectHoliday()
    {
        var holidays = FrenchCommunity.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 27) && h.Name == FrenchCommunityDay);
    }

    [Fact]
    public void GermanSpeakingCommunity_HasCorrectHoliday()
    {
        var holidays = GermanSpeakingCommunity.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 15) && h.Name == GermanCommunityDay);
    }
}
