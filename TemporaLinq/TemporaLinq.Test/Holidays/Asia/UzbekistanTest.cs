using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Uzbekistan;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class UzbekistanTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 8) && h.Name == InternationalWomensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == NowruzDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 9) && h.Name == MemoryAndHonorDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 1) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 1) && h.Name == TeachersAndInstructorsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == ConstitutionDayOfUzbekistan);
    }

    [Fact]
    public void GetHolidays_ContainsHijriBasedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 2))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 3))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 10))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlAdha);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 11))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlAdha);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 12))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlAdha);
    }
}
