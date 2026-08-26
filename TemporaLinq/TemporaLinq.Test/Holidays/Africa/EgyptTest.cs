using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Africa.Egypt;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Africa;

public class EgyptTest
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

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 7) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 25) && h.Name == RevolutionDayOfEgypt);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 25) && h.Name == SinaiLiberationDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 23) && h.Name == RevolutionDayOfEgypt);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 6) && h.Name == ArmedForcesDay);
    }

    [Fact]
    public void GetHolidays_ContainsShamElNessimTheDayAfterCopticEaster()
    {
        var copticEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == copticEaster2026.AddDays(1) && h.Name == ShamElNessim);
    }

    [Fact]
    public void GetHolidays_ContainsHijriFeasts()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 2))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 3))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlFitr);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 9))
            holidays.Should().Contain(h => h.Date == date && h.Name == ArafatDay);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 10))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlAdha);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 11))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlAdha);
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 12))
            holidays.Should().Contain(h => h.Date == date && h.Name == EidAlAdha);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 1, 1))
            holidays.Should().Contain(h => h.Date == date && h.Name == IslamicNewYear);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 3, 12))
            holidays.Should().Contain(h => h.Date == date && h.Name == ProphetsBirthday);
    }
}
