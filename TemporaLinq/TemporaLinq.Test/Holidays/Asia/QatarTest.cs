using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Qatar;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class QatarTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(8);
    }

    [Fact]
    public void GetHolidays_ContainsFixedAndMovableCivilHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 10) && h.Name == SportsDayOfQatar);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 18) && h.Name == NationalDayOfQatar);
    }

    [Fact]
    public void GetHolidays_ContainsEidAlFitrAndEidAlAdha()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 20) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 26) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 27) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 28) && h.Name == EidAlAdha);
    }
}
