using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Iran;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class IranTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(17);
    }

    [Fact]
    public void GetHolidays_ContainsPersianCalendarHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == Nowruz);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 22) && h.Name == Nowruz);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 23) && h.Name == Nowruz);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 24) && h.Name == Nowruz);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 1) && h.Name == IslamicRepublicDayOfIran);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 2) && h.Name == NaturesDayOfIran);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 4) && h.Name == DeathOfKhomeini);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 5) && h.Name == KhordadNationalUprisingDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 11) && h.Name == IslamicRevolutionDayOfIran);
    }

    [Fact]
    public void GetHolidays_ContainsHijriShiaHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 24) && h.Name == Tasua);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 25) && h.Name == Ashura);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 4) && h.Name == Arbaeen);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 30) && h.Name == MawlidAlNabi);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 20) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 26) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 3) && h.Name == EidAlGhadir);
    }
}
