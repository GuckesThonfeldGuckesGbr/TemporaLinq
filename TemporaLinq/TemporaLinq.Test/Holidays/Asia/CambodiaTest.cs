using FluentAssertions;
using TemporaLinq.Astronomy;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Cambodia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class CambodiaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 7) && h.Name == VictoryOverGenocideDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 8) && h.Name == InternationalWomensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 14) && h.Name == KhmerNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 15) && h.Name == KhmerNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 16) && h.Name == KhmerNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 14) && h.Name == BirthdayOfKingNorodomSihamoni);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 18) && h.Name == BirthdayOfQueenMotherNorodomMonineath);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 24) && h.Name == ConstitutionDayOfCambodia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 15) && h.Name == CommemorationDayOfKingFatherNorodomSihanouk);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 29) && h.Name == CoronationDayOfKingNorodomSihamoni);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 9) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 29) && h.Name == PeaceDayOfCambodia);
    }

    [Fact]
    public void GetHolidays_ContainsVisakBocheaDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        var expected = SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(2026);

        holidays.Should().Contain(h => h.Date == expected && h.Name == VisakBocheaDay);
    }
}
