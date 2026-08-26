using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Vietnam;

/// <summary>
/// Provides Vietnamese national public holidays.
/// <para>
/// Tết (Lunar New Year) is modeled as the 5-day statutory span fixed by Vietnamese
/// labour law: New Year's Eve (the day before lunisolar month 1 day 1) plus the
/// first four days of the lunar new year. Government-announced weekend "bridge day"
/// extensions around this core span are a per-year administrative decision, not
/// calendar arithmetic, and are out of scope (see the worldwide-holidays design doc).
/// </para>
/// <para>
/// <b>Approximation caveat:</b> Tết and Hùng Kings' Commemoration Day are computed
/// via <see cref="ChineseLunisolarCalendarCalculation"/>, which models China's
/// lunisolar calendar (computed for UTC+8). Vietnam's own lunisolar calendar is
/// nominally computed for UTC+7; in rare years a new moon falling close to the day
/// boundary between the two time zones can cause Vietnam's calendar to land a full
/// lunar month off from China's. This is a documented, accepted approximation per
/// the calendar-calculation-mechanisms design.
/// </para>
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var tetDay1 = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 1);
        var hungKings = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 3), 10);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(tetDay1.AddDays(-1), LunarNewYearsDay),
                new(tetDay1, LunarNewYearsDay),
                new(tetDay1.AddDays(1), LunarNewYearsDay),
                new(tetDay1.AddDays(2), LunarNewYearsDay),
                new(tetDay1.AddDays(3), LunarNewYearsDay),
                new(hungKings, HungKingsCommemorationDay),
                new(new DateOnly(year, 4, 30), ReunificationDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 9, 2), NationalDayOfVietnam),
            }
            .Order()
            .ToImmutableList();
    }

    private static readonly ChineseLunisolarCalendar Calendar = new();

    private static int EffectiveMonth(int gregorianYear, int civilMonth)
    {
        var lunisolarYear = Calendar.GetYear(new DateTime(gregorianYear, 6, 1));
        var leapMonth = Calendar.GetLeapMonth(lunisolarYear);
        return leapMonth != 0 && leapMonth < civilMonth ? civilMonth + 1 : civilMonth;
    }
}
