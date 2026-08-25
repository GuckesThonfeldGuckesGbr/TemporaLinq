using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.UnitedKingdom;

/// <summary>
/// Additive Scottish bank holidays layered on top of the England-and-Wales <see cref="NationalHolidays"/>.
/// This record alone does not represent a complete or accurate Scottish calendar: Scotland does not
/// observe Easter Monday (which remains present via <see cref="NationalHolidays"/> when merged), and
/// Scotland's summer bank holiday falls on the first Monday of August rather than the last Monday
/// observed by <see cref="NationalHolidays"/>'s SummerBankHoliday. Neither discrepancy is corrected
/// by this additive model.
/// </summary>
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
