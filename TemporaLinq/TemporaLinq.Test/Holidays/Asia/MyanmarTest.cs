using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Myanmar;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class MyanmarTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(15);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 4) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 12) && h.Name == UnionDayOfMyanmar);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 2) && h.Name == PeasantsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 27) && h.Name == ArmedForcesDayOfMyanmar);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 13) && h.Name == ThingyanDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 14) && h.Name == ThingyanDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 15) && h.Name == ThingyanDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 16) && h.Name == ThingyanDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 19) && h.Name == MartyrsDayOfMyanmar);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsBuddhistHolyDays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 3) && h.Name == TabaungFullMoonDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 31) && h.Name == KasonFullMoonDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 29) && h.Name == WasoFullMoonDay);
    }
}
