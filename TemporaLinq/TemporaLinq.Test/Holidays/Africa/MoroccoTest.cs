using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Africa.Morocco;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Africa;

public class MoroccoTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 11) && h.Name == IndependenceManifestoDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 30) && h.Name == ThroneDayOfMorocco);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 14) && h.Name == OuedEdDahabDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 20) && h.Name == RevolutionOfTheKingAndThePeopleDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 21) && h.Name == YouthDayOfMorocco);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 6) && h.Name == GreenMarchDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 18) && h.Name == IndependenceDay);
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

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 1, 1))
            holidays.Should().Contain(h => h.Date == date && h.Name == IslamicNewYear);

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(2026, 3, 12))
            holidays.Should().Contain(h => h.Date == date && h.Name == ProphetsBirthday);
    }
}
