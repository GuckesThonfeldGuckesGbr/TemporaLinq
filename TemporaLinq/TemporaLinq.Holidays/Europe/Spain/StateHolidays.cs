using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Spain;

public record BalearicIslands : HolidayEnumerable<BalearicIslands>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        return ImmutableList.Create(new Holiday(easter.AddDays(1), EasterMonday));
    }
}

public record Andalusia : HolidayEnumerable<Andalusia>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year) 
        => ImmutableList.Create(new Holiday(new DateOnly(year, 4,25), SanMarco));
}
public record BasqueCountry : HolidayEnumerable<BasqueCountry>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year) 
        => ImmutableList.Create(new Holiday(new DateOnly(year, 4,25), SanMarco));
}
