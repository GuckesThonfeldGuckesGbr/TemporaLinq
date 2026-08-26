using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.HongKong;

/// <summary>
/// Provides Hong Kong general (public) holidays.
/// <para>
/// Mid-Autumn Festival is observed the day *after* the lunisolar 8th-month-15th-day
/// date (most celebrations happen at night, so the following day is the rest day).
/// </para>
/// <para>
/// No Sunday-substitution rule is modeled: Hong Kong law moves a holiday to the next
/// weekday when it falls on a Sunday (occasionally shifting further still to avoid
/// colliding with an already-designated holiday, as with Ching Ming 2026, which
/// truly falls on Sunday April 5 but is officially observed April 7 to avoid Easter
/// Monday on April 6). This library always returns the true underlying calendar/
/// solar-term/lunisolar date, consistent with how movable feasts are treated
/// elsewhere in this library.
/// </para>
/// <para>
/// Qingming/Ching Ming Festival is a solar term, computed the same way as for China
/// (see <see cref="TemporaLinq.Holidays.Asia.China.NationalHolidays"/>).
/// </para>
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        var lunarNewYear = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 1);
        var qingming = QingmingFestivalDate(year);
        var buddhasBirthday = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 4), 8);
        var dragonBoat = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 5), 5);
        var midAutumn = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 8), 15);
        var chungYeung = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 9), 9);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(lunarNewYear, LunarNewYearsDay),
                new(lunarNewYear.AddDays(1), LunarNewYearsDay),
                new(lunarNewYear.AddDays(2), LunarNewYearsDay),
                new(qingming, QingmingFestival),
                new(easter.AddDays(-2), GoodFriday),
                new(easter.AddDays(-1), HolySaturday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(buddhasBirthday, BuddhasBirthday),
                new(dragonBoat, DragonBoatFestival),
                new(new DateOnly(year, 7, 1), HKSAREstablishmentDay),
                new(midAutumn.AddDays(1), MidAutumnFestival),
                new(new DateOnly(year, 10, 1), NationalDayOfChina),
                new(chungYeung, ChungYeungFestival),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), BoxingDay),
            }
            .Order()
            .ToImmutableList();
    }

    private static DateOnly QingmingFestivalDate(int year)
    {
        var y = year % 100;
        var aprilDay = (int)Math.Floor(y * 0.2422 + 4.81) - y / 4;
        return new DateOnly(year, 4, aprilDay);
    }

    private static readonly ChineseLunisolarCalendar Calendar = new();

    private static int EffectiveMonth(int gregorianYear, int civilMonth)
    {
        var lunisolarYear = Calendar.GetYear(new DateTime(gregorianYear, 6, 1));
        var leapMonth = Calendar.GetLeapMonth(lunisolarYear);
        return leapMonth != 0 && leapMonth < civilMonth ? civilMonth + 1 : civilMonth;
    }
}
