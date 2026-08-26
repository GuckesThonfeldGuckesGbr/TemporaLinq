using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Turkey;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class TurkeyTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var eidAlFitrDays = HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1).Count() * 3;
        var eidAlAdhaDays = HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 10).Count() * 4;

        holidays.Should().HaveCount(7 + eidAlFitrDays + eidAlAdhaDays);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 23) && h.Name == NationalSovereigntyAndChildrensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 19) && h.Name == YouthAndSportsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 15) && h.Name == DemocracyAndNationalUnityDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 30) && h.Name == VictoryDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 29) && h.Name == RepublicDay);
    }

    [Fact]
    public void GetHolidays_ContainsRamazanBayrami()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        foreach (var start in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1))
        {
            holidays.Should().Contain(h => h.Date == start && h.Name == EidAlFitr);
            holidays.Should().Contain(h => h.Date == start.AddDays(1) && h.Name == EidAlFitr);
            holidays.Should().Contain(h => h.Date == start.AddDays(2) && h.Name == EidAlFitr);
        }
    }

    [Fact]
    public void GetHolidays_ContainsKurbanBayrami()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        foreach (var start in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 10))
        {
            holidays.Should().Contain(h => h.Date == start && h.Name == EidAlAdha);
            holidays.Should().Contain(h => h.Date == start.AddDays(1) && h.Name == EidAlAdha);
            holidays.Should().Contain(h => h.Date == start.AddDays(2) && h.Name == EidAlAdha);
            holidays.Should().Contain(h => h.Date == start.AddDays(3) && h.Name == EidAlAdha);
        }
    }
}
