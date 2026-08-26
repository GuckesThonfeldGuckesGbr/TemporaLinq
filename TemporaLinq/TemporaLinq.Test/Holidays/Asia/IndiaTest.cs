using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.India;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class IndiaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(9);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 26) && h.Name == RepublicDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 2) && h.Name == GandhiJayanti);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsGoodFriday()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
    }

    [Fact]
    public void GetHolidays_ContainsHijriBasedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var eidAlFitr = HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1).Single();
        var eidAlAdha = HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 10).Single();
        var muharram = HijriCalendarCalculation.DatesInGregorianYear(2026, 1, 1).Single();
        var miladUnNabi = HijriCalendarCalculation.DatesInGregorianYear(2026, 3, 12).Single();

        holidays.Should().Contain(h => h.Date == eidAlFitr && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == eidAlAdha && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == muharram && h.Name == Muharram);
        holidays.Should().Contain(h => h.Date == miladUnNabi && h.Name == MiladUnNabi);
    }
}
