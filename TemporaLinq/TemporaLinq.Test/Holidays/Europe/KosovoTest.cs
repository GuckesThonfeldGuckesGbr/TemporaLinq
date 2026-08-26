using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Kosovo;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class KosovoTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var eidAlFitrCount = HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1).Count();
        var eidAlAdhaCount = HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 10).Count();

        holidays.Should().HaveCount(10 + eidAlFitrCount + eidAlAdhaCount);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 7) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 17) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 9) && h.Name == ConstitutionDayOfKosovo);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 9) && h.Name == EuropeDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var catholicEaster2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == catholicEaster2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
    }

    [Fact]
    public void GetHolidays_ContainsHijriHolidays()
    {
        var eidAlFitrDates = HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1);
        var eidAlAdhaDates = HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 10);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        foreach (var date in eidAlFitrDates)
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);

        foreach (var date in eidAlAdhaDates)
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlAdha);
    }
}
