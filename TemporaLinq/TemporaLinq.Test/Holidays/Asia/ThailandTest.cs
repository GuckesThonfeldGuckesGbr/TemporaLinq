using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Thailand;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class ThailandTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(19);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 6) && h.Name == ChakriMemorialDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 13) && h.Name == SongkranDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 14) && h.Name == SongkranDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 15) && h.Name == SongkranDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 4) && h.Name == CoronationDayOfThailand);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 3) && h.Name == QueensBirthdayOfThailand);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 28) && h.Name == KingsBirthdayOfThailand);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 12) && h.Name == MothersDayOfThailand);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 13) && h.Name == KingBhumibolMemorialDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 23) && h.Name == ChulalongkornDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 5) && h.Name == NationalDayOfThailand);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 10) && h.Name == ConstitutionDayOfThailand);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 31) && h.Name == NewYearsEve);
    }

    [Fact]
    public void GetHolidays_ContainsBuddhistHolyDays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 3) && h.Name == MakhaBuchaDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 31) && h.Name == VisakhaBuchaDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 29) && h.Name == AsalhaBuchaDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 30) && h.Name == KhaoPhansaDay);
    }
}
