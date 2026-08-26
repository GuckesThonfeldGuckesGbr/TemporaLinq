using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Turkey;

/// <summary>
/// Provides Turkish national public holidays. Ramazan Bayramı (Eid al-Fitr) and
/// Kurban Bayramı (Eid al-Adha) are computed from the Hijri calendar via
/// <see cref="HijriCalendarCalculation"/> — a deterministic approximation that can
/// differ by +/-1, rarely +/-2, days from the real-world moon-sighting-confirmed
/// date. Turkish law (2429 sayılı Kanun) additionally grants a half-day "arife"
/// (eve) before each bayram, starting at 13:00 the previous day; since
/// <see cref="Holiday"/> has day granularity only, that half-day is out of scope
/// and only the full official days are modeled here (3 full days for Ramazan
/// Bayramı, 4 full days for Kurban Bayramı).
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var holidays = new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 4, 23), NationalSovereigntyAndChildrensDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 19), YouthAndSportsDay),
                new(new DateOnly(year, 7, 15), DemocracyAndNationalUnityDay),
                new(new DateOnly(year, 8, 30), VictoryDay),
                new(new DateOnly(year, 10, 29), RepublicDay),
            };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)
            .SelectMany(start => new[] { start, start.AddDays(1), start.AddDays(2) })
            .Select(date => new Holiday(date, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10)
            .SelectMany(start => new[] { start, start.AddDays(1), start.AddDays(2), start.AddDays(3) })
            .Select(date => new Holiday(date, EidAlAdha)));

        return holidays.Order().ToImmutableList();
    }
}
