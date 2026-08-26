using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.SaudiArabia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class SaudiArabiaTest
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

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 23) && h.Name == NationalDayOfSaudiArabia);
    }

    [Fact]
    public void GetHolidays_ContainsEidAlFitr()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 20) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 22) && h.Name == EidAlFitr);
    }

    [Fact]
    public void GetHolidays_ContainsArafahDayAndEidAlAdha()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 25) && h.Name == ArafahDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 26) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 27) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 28) && h.Name == EidAlAdha);
    }
}
