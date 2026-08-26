using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.China;

/// <summary>
/// Provides Chinese national public holidays: Chinese New Year, Qingming Festival,
/// Dragon Boat Festival, and Mid-Autumn Festival, plus fixed civil holidays.
/// <para>
/// Only the core lunisolar-/solar-term-anchored date for each festival is modeled.
/// China's actual statutory calendar adds a multi-day "Golden Week" around several
/// of these (e.g. Spring Festival's eve + following days) plus government-announced
/// weekend "make-up workday" shifts published separately for each year — those are
/// non-formulaic, per-year administrative decisions, not calendar arithmetic, and
/// are out of scope here (see the worldwide-holidays design doc).
/// </para>
/// <para>
/// Qingming Festival is a solar term (not a lunisolar date), computed via a
/// well-documented arithmetic approximation for the 21st century (2001-2100):
/// <c>floor(Y * 0.2422 + 4.81) - floor(Y / 4)</c> gives the April day number, where
/// Y is the last two digits of the Gregorian year. Verified against known reference
/// dates (e.g. April 4, 2021; April 5, 2019 and 2026).
/// </para>
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var chineseNewYear = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 1);
        var qingming = QingmingFestivalDate(year);
        var dragonBoat = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 5), 5);
        var midAutumn = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 8), 15);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(chineseNewYear, LunarNewYearsDay),
                new(qingming, QingmingFestival),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(dragonBoat, DragonBoatFestival),
                new(midAutumn, MidAutumnFestival),
                new(new DateOnly(year, 10, 1), NationalDayOfChina),
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
