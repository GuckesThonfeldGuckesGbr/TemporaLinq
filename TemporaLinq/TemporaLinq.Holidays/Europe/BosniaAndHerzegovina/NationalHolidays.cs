using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.BosniaAndHerzegovina;

/// <summary>
/// Provides the state-level (BiH-wide) national public holidays of Bosnia and
/// Herzegovina — the small set of dates genuinely observed nationwide by state
/// institutions, as distinct from the much larger set of entity-specific holidays
/// (Federation of Bosnia and Herzegovina vs. Republika Srpska vs. Brčko District)
/// and community-specific religious observances (Catholic/Orthodox Christmas and
/// Easter, Eid al-Fitr, Eid al-Adha), which diverge by entity/community and are
/// out of scope here. Attempts to legislate a unified state-level holiday law
/// covering those additional dates have repeatedly stalled in the Parliamentary
/// Assembly precisely because the entities disagree on which dates qualify as
/// national — this is a genuine, ongoing political fragmentation, not an
/// oversight of this implementation.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 1, 2), NewYearsDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 2), LabourDay),
            }
            .Order()
            .ToImmutableList();
    }
}
