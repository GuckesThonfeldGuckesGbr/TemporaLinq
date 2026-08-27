using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Mongolia;

/// <summary>
/// Provides Mongolian national public holidays: fixed civil days plus Tsagaan Sar (Lunar New
/// Year, month 1 days 1-3) and Ikh Duichen (Buddha Day, month 4 day 15), both computed from the
/// Mongolian lunisolar calendar via <see cref="MongolianCalendarCalculation"/>.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var tsagaanSar = MongolianCalendarCalculation.DateInGregorianYear(year, 1, 1);
        var ikhDuichen = MongolianCalendarCalculation.DateInGregorianYear(year, 4, 15);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 3, 8), InternationalWomensDay),
                new(tsagaanSar, TsagaanSar),
                new(tsagaanSar.AddDays(1), TsagaanSar),
                new(tsagaanSar.AddDays(2), TsagaanSar),
                new(ikhDuichen, IkhDuichen),
                new(new DateOnly(year, 6, 1), ChildrensDay),
                new(new DateOnly(year, 7, 11), NaadamFestival),
                new(new DateOnly(year, 7, 12), NaadamFestival),
                new(new DateOnly(year, 7, 13), NaadamFestival),
                new(new DateOnly(year, 7, 14), NaadamFestival),
                new(new DateOnly(year, 7, 15), NaadamFestival),
                new(new DateOnly(year, 11, 26), RepublicDay),
                new(new DateOnly(year, 12, 29), IndependenceDay),
            }
            .Order()
            .ToImmutableList();
    }
}
