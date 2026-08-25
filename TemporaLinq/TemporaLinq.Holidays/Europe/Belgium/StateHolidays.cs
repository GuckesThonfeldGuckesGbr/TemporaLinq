using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Belgium;

public record FlemishCommunity : HolidayEnumerable<FlemishCommunity>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 7, 11), FlemishCommunityDay));
}

public record FrenchCommunity : HolidayEnumerable<FrenchCommunity>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 9, 27), FrenchCommunityDay));
}

public record GermanSpeakingCommunity : HolidayEnumerable<GermanSpeakingCommunity>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 11, 15), GermanCommunityDay));
}
