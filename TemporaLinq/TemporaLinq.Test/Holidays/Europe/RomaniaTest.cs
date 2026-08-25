using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Romania;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class RomaniaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(16);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == SecondJanuary);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 7) && h.Name == SynaxisOfStJohnTheBaptist);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 1) && h.Name == ChildrensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 30) && h.Name == StAndrewsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 1) && h.Name == NationalDayOfRomania);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsOrthodoxMovableFeasts()
    {
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(49) && h.Name == WhitSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(50) && h.Name == WhitMonday);
    }
}
