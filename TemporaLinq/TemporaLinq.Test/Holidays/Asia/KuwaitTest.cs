using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Kuwait;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class KuwaitTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(12);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 25) && h.Name == NationalDayOfKuwait);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 26) && h.Name == LiberationDay);
    }

    [Fact]
    public void GetHolidays_ContainsEidAlFitrAndEidAlAdha()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 25) && h.Name == ArafahDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 26) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 28) && h.Name == EidAlAdha);
    }

    [Fact]
    public void GetHolidays_ContainsIslamicNewYearAshuraAndMawlid()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 16) && h.Name == IslamicNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 25) && h.Name == AshuraDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 25) && h.Name == ProphetsBirthday);
    }
}
