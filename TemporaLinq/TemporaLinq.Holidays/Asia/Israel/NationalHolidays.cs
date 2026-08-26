using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Israel;

/// <summary>
/// Provides Israeli national public holidays, computed from the Hebrew lunisolar calendar
/// via <see cref="HebrewCalendarCalculation"/>. Tishrei-based holidays (Rosh Hashanah, Yom
/// Kippur, Sukkot, Simchat Torah) always use Hebrew month 1 regardless of leap status. Nisan,
/// Iyar, and Sivan-based holidays (Passover, Yom HaShoah, Yom Ha'atzmaut, Shavuot) shift up by
/// one month slot in a 13-month Hebrew leap year, because Adar splits into Adar I (month 6)
/// and Adar II (month 7) earlier in that same Hebrew year - so this type re-derives the
/// correct month numbers per year rather than hardcoding them.
///
/// Known simplification: Yom Ha'atzmaut's real-world shifting rule (5 Iyar moves to the
/// preceding Thursday if it falls on Friday or Saturday, or to the following Tuesday if it
/// falls on a Monday - in force since 1951/2004 to avoid Sabbath conflicts around Yom
/// HaZikaron/Yom Ha'atzmaut) IS implemented below. Yom HaShoah has an analogous but less
/// consistently documented shift rule that is NOT implemented - its date here is always the
/// unshifted 27 Nisan. Yom HaZikaron itself (a solemn memorial day, not a non-working public
/// holiday) is intentionally out of scope.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    private static readonly HebrewCalendar HebrewCal = new();

    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        // The Hebrew year covering Nisan/Iyar/Sivan in this Gregorian year - use a date well
        // inside that stretch (April 1) to look up the relevant Hebrew year and its leap status.
        var isLeapYear = HebrewCal.IsLeapYear(HebrewCal.GetYear(new DateTime(year, 4, 1)));
        var nisan = isLeapYear ? 8 : 7;
        var iyar = isLeapYear ? 9 : 8;
        var sivan = isLeapYear ? 10 : 9;

        var yomHaAtzmaut = ShiftYomHaAtzmaut(HebrewCalendarCalculation.DateInGregorianYear(year, iyar, 5));

        return new List<Holiday>
            {
                new(HebrewCalendarCalculation.DateInGregorianYear(year, 1, 1), RoshHashanah),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, 1, 2), RoshHashanah),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, 1, 10), YomKippur),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, 1, 15), Sukkot),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, 1, 22), SimchatTorah),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, nisan, 15), Passover),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, nisan, 21), Passover),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, nisan, 27), YomHaShoah),
                new(yomHaAtzmaut, YomHaAtzmaut),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, sivan, 6), Shavuot),
            }
            .Order()
            .ToImmutableList();
    }

    /// <summary>
    /// Applies Israel's Independence Day shifting rule: if 5 Iyar falls on Friday or
    /// Saturday, the holiday moves to the preceding Thursday; if it falls on a Monday, it
    /// moves to the following Tuesday. All other weekdays are unshifted.
    /// </summary>
    private static DateOnly ShiftYomHaAtzmaut(DateOnly fifthOfIyar)
        => fifthOfIyar.DayOfWeek switch
        {
            DayOfWeek.Friday => fifthOfIyar.AddDays(-1),
            DayOfWeek.Saturday => fifthOfIyar.AddDays(-2),
            DayOfWeek.Monday => fifthOfIyar.AddDays(1),
            _ => fifthOfIyar,
        };
}
