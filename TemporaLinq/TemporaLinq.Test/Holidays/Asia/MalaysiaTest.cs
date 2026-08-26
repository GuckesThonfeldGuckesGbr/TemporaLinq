using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Malaysia;
using TemporaLinq.Astronomy;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class MalaysiaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(14);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 31) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 16) && h.Name == NationalDayOfMalaysia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsAgongsBirthday()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 1) && h.Name == AgongsBirthday);
    }

    [Fact]
    public void GetHolidays_ContainsChineseNewYear()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 17) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 18) && h.Name == LunarNewYearsDay);
    }

    [Fact]
    public void GetHolidays_ContainsHijriBasedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 2))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 10))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlAdha);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 1, 1))
            holidays.Should().Contain(h => h.Date == date && h.Name == IslamicNewYear);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 3, 12))
            holidays.Should().Contain(h => h.Date == date && h.Name == ProphetsBirthday);
    }

    [Fact]
    public void GetHolidays_ContainsVesakDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var vesak = SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(2026);
        vesak.Should().Be(new DateOnly(2026, 5, 31));
        holidays.Should().Contain(h => h.Date == vesak && h.Name == VesakDay);
    }
}
