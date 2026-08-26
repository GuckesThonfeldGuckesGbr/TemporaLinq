using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.NorthAmerica.Haiti;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.NorthAmerica;

public class HaitiTest
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

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == AncestryDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 18) && h.Name == FlagAndUniversitiesDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 17) && h.Name == DessalinesMemorialDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 2) && h.Name == AllSoulsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsEasterRelativeHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-48) && h.Name == CarnivalMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-47) && h.Name == CarnivalTuesday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
    }
}
