using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.NorthMacedonia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class NorthMacedoniaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 7) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 2) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 24) && h.Name == SaintsCyrilAndMethodiusDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 2) && h.Name == IlindenDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 8) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 11) && h.Name == DayOfMacedonianUprising);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 23) && h.Name == RevolutionaryStruggleDayOfMacedonia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == StClementOfOhridDay);
    }

    [Fact]
    public void GetHolidays_ContainsOrthodoxMovableFeasts()
    {
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(1) && h.Name == EasterMonday);
    }
}
