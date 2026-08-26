using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.HongKong;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class HongKongTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(17);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 1) && h.Name == HKSAREstablishmentDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 1) && h.Name == NationalDayOfChina);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == BoxingDay);
    }

    [Fact]
    public void GetHolidays_ContainsLunarNewYearAndOtherLunisolarHolidays()
    {
        // Reference dates independently verified 2026-08-25/26 against
        // System.Globalization.ChineseLunisolarCalendar and cross-checked against the
        // Hong Kong Labour Department's published 2026 statutory holidays circular.
        // The true unshifted dates are used (no Sunday-substitution rule modeled) —
        // e.g. Buddha's Birthday and Chung Yeung both fall on a Sunday in 2026 and are
        // officially observed one day later, which this library does not model.
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 17) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 18) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 19) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 5) && h.Name == QingmingFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 24) && h.Name == BuddhasBirthday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 19) && h.Name == DragonBoatFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 26) && h.Name == MidAutumnFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 18) && h.Name == ChungYeungFestival);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-1) && h.Name == HolySaturday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }
}
