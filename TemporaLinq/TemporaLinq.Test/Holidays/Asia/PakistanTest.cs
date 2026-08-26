using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Pakistan;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class PakistanTest
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

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 5) && h.Name == KashmirSolidarityDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 23) && h.Name == PakistanDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 14) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 9) && h.Name == IqbalDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == QuaidEAzamDay);
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

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 1, 9))
            holidays.Should().Contain(h => h.Date == date && h.Name == AshuraDay);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 1, 10))
            holidays.Should().Contain(h => h.Date == date && h.Name == AshuraDay);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 3, 12))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidMiladUnNabi);
    }
}
