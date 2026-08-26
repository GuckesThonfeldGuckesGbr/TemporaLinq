using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.SouthAmerica.Venezuela;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.SouthAmerica;

public class VenezuelaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 24) && h.Name == BattleOfCarababoDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 5) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 24) && h.Name == BolivarsBirthday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 12) && h.Name == IndigenousResistanceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 24) && h.Name == ChristmasEve);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 31) && h.Name == NewYearsEve);
    }

    [Fact]
    public void GetHolidays_ContainsEasterRelativeHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-48) && h.Name == CarnivalMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-47) && h.Name == CarnivalTuesday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-3) && h.Name == MaundyThursday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
    }
}
