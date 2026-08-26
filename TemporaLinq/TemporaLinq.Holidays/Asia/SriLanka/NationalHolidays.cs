using System.Collections.Immutable;
using Memoizer;
using TemporaLinq.Astronomy;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.SriLanka;

/// <summary>
/// Provides Sri Lankan national public holidays: fixed civil days, Christian Good Friday, Hijri-
/// based Eid al-Fitr and Eid al-Adha, and a Poya (full moon) holiday for every full moon of the
/// year. Maha Sivarathri (a Hindu lunar-calendar holiday) is out of scope pending a future Hindu
/// calendar calculation mechanism.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);

        var holidays = new List<Holiday>
        {
            new(new DateOnly(year, 1, 1), NewYearsDay),
            new(new DateOnly(year, 1, 14), TamilThaiPongalDay),
            new(new DateOnly(year, 2, 4), IndependenceDay),
            new(new DateOnly(year, 4, 13), SinhalaAndTamilNewYearDay),
            new(new DateOnly(year, 4, 14), SinhalaAndTamilNewYearDay),
            new(easter.AddDays(-2), GoodFriday),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(new DateOnly(year, 12, 25), ChristmasDay),
        };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)
            .Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10)
            .Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(LunarPhaseCalculation.FullMoonsInGregorianYear(year)
            .Select(d => new Holiday(d, PoyaDay)));

        return holidays.Order().ToImmutableList();
    }
}
