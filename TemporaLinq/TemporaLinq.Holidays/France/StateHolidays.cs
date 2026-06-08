using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.France;

public record AlsaceMoselle : HolidayEnumerable<AlsaceMoselle>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 12, 26), StStephensDay));
}