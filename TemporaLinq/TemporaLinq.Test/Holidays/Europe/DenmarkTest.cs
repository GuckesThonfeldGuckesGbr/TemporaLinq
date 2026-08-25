using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Denmark;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class DenmarkTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(11);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 5) && h.Name == ConstitutionDayOfDenmark);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-3) && h.Name == MaundyThursday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == AscensionDay);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(49) && h.Name == WhitSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == WhitMonday);
    }

    [Fact]
    public void GetHolidays_DoesNotContainAbolishedGeneralPrayerDay()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().NotContain(h => h.Date == easter2026.AddDays(26));
    }
}
