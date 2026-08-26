using FluentAssertions;
using TemporaLinq.Astronomy;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.SriLanka;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class SriLankaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        // 8 fixed/Easter-based days (New Year, Pongal, Independence, Sinhala/Tamil New Year x2,
        // Good Friday, Labour Day, Christmas) + Eid al-Fitr + Eid al-Adha + 13 Poya days (2026
        // has 13 full moons) = 23. Maha Sivarathri (Hindu lunar) is deliberately out of scope.
        holidays.Should().HaveCount(23);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 14) && h.Name == TamilThaiPongalDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 4) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 13) && h.Name == SinhalaAndTamilNewYearDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 14) && h.Name == SinhalaAndTamilNewYearDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
    }

    [Fact]
    public void GetHolidays_ContainsHijriBasedHolidays()
    {
        var eidAlFitr2026 = HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1).Single();
        var eidAlAdha2026 = HijriCalendarCalculation.DatesInGregorianYear(2026, 12, 10).Single();
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == eidAlFitr2026 && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == eidAlAdha2026 && h.Name == EidAlAdha);
    }

    [Fact]
    public void GetHolidays_ContainsAPoyaDayForEveryFullMoonInTheYear()
    {
        var fullMoons = LunarPhaseCalculation.FullMoonsInGregorianYear(2026).ToList();
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        fullMoons.Should().HaveCount(13);
        foreach (var fullMoon in fullMoons)
            holidays.Should().Contain(h => h.Date == fullMoon && h.Name == PoyaDay);
    }
}
