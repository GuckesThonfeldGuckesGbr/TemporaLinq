using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Qatar;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

/// <summary>
/// Provides Qatari national public holidays, using the statutory minimums from
/// Qatari Labour Law Article 74 for Eid al-Fitr and Eid al-Adha (the government
/// sector often decrees longer ad-hoc extensions in a given year, which are out
/// of scope here as year-specific decrees rather than a stable formula).
/// Islamic-calendar holidays are computed from the tabular Hijri calendar via
/// <see cref="HijriCalendarCalculation"/>; real-world moon-sighting announcements
/// can differ from this calculation by +/-1, rarely +/-2, days.
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
            new(Dates.Invariant().From(new DateOnly(year, 2, 8)).First(DayOfWeek.Tuesday), SportsDayOfQatar),
            new(new DateOnly(year, 12, 18), NationalDayOfQatar),
        };

        foreach (var eidAlFitrStart in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
        {
            for (var day = 0; day < 3; day++)
                holidays.Add(new Holiday(eidAlFitrStart.AddDays(day), EidAlFitr));
        }

        foreach (var eidAlAdhaStart in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10))
        {
            for (var day = 0; day < 3; day++)
                holidays.Add(new Holiday(eidAlAdhaStart.AddDays(day), EidAlAdha));
        }

        return holidays.Order().ToImmutableList();
    }
}
