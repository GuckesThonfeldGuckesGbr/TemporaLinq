using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Laos;

/// <summary>
/// Provides Lao national public holidays: the fixed statutory list enumerated by Laos's Labour
/// Law (2013, No. 43/NA, Article 55) that applies to the general population. Lao New Year (Pi
/// Mai, a solar-calendar festival, not the lunisolar Buddhist calendar) is modeled as fixed April
/// 14-16 dates, following the same convention already used for Sri Lanka's Sinhala/Tamil New Year
/// and Cambodia's Khmer New Year, rather than computing solar ingress.
///
/// Unlike Thailand, Myanmar, and Cambodia, Laos has no statutory Buddhist-calendar holiday at
/// all: Visakha Bousa (Vesak), Boun Khao Phansa, and Boun Ok Phansa — the same Buddhist holy days
/// this project can compute via <see cref="TemporaLinq.Astronomy.SoutheastAsianBuddhistCalendar"/>
/// — are widely observed culturally in Laos but are not part of the statutory public holiday law,
/// per multiple independently-corroborated sources during research. This is intentionally not
/// implemented as an approximation; it is out of scope because it is not an official holiday
/// here, not because it is uncomputable.
///
/// National Teachers' Day (7 October) is also out of scope: Article 55 restricts it to teachers
/// and education-management staff rather than the general population, the same treatment already
/// given to other narrow-scope observances elsewhere in this project (e.g. Cambodia's Royal
/// Ploughing Ceremony).
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
            new(new DateOnly(year, 3, 8), InternationalWomensDay),
            new(new DateOnly(year, 4, 14), LaoNewYear),
            new(new DateOnly(year, 4, 15), LaoNewYear),
            new(new DateOnly(year, 4, 16), LaoNewYear),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(new DateOnly(year, 12, 2), LaoNationalDay),
        };

        return holidays.Order().ToImmutableList();
    }
}
