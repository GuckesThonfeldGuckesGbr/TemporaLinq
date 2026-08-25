using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.UnitedKingdom;

public record Scotland : HolidayEnumerable<Scotland>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => new List<Holiday>
            {
                new(new DateOnly(year, 1, 2), SecondJanuary),
                new(new DateOnly(year, 11, 30), StAndrewsDay),
            }
            .Order()
            .ToImmutableList();
}

public record NorthernIreland : HolidayEnumerable<NorthernIreland>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => new List<Holiday>
            {
                new(new DateOnly(year, 3, 17), StPatricksDay),
                new(new DateOnly(year, 7, 12), BattleOfTheBoyneDay),
            }
            .Order()
            .ToImmutableList();
}
