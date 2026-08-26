using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Africa.Nigeria;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Africa;

public class NigeriaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 12) && h.Name == DemocracyDayOfNigeria);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 1) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == BoxingDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableChristianFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }

    [Fact]
    public void GetHolidays_ContainsHijriFeasts()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 2))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 10))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlAdha);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 11))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlAdha);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 3, 12))
            holidays.Should().Contain(h => h.Date == date && h.Name == ProphetsBirthday);
    }
}
