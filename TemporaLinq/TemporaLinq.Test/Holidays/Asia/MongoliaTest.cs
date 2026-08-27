using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Mongolia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class MongoliaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 8) && h.Name == InternationalWomensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 1) && h.Name == ChildrensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 11) && h.Name == NaadamFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 12) && h.Name == NaadamFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 13) && h.Name == NaadamFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 14) && h.Name == NaadamFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 15) && h.Name == NaadamFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 26) && h.Name == RepublicDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 29) && h.Name == IndependenceDay);
    }

    [Fact]
    public void GetHolidays_ContainsTsagaanSar()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var newYear = MongolianCalendarCalculation.DateInGregorianYear(2026, 1, 1);

        holidays.Should().Contain(h => h.Date == newYear && h.Name == TsagaanSar);
        holidays.Should().Contain(h => h.Date == newYear.AddDays(1) && h.Name == TsagaanSar);
        holidays.Should().Contain(h => h.Date == newYear.AddDays(2) && h.Name == TsagaanSar);
    }

    [Fact]
    public void GetHolidays_ContainsIkhDuichen()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var expected = MongolianCalendarCalculation.DateInGregorianYear(2026, 4, 15);

        holidays.Should().Contain(h => h.Date == expected && h.Name == IkhDuichen);
    }
}
