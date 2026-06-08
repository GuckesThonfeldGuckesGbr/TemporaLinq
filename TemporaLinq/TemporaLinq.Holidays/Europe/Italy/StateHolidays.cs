using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Italy;

public record TrentinoAltoAdige : HolidayEnumerable<TrentinoAltoAdige>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        return ImmutableList.Create(new Holiday(easter.AddDays(50), WhitMonday));
    }
}

public record Venice : HolidayEnumerable<Venice>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year) 
        => ImmutableList.Create(new Holiday(new DateOnly(year, 4,25), SanMarco));
}

public record RomeLazio : HolidayEnumerable<RomeLazio>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year) 
        => ImmutableList.Create(new Holiday(new DateOnly(year, 6,29), StPeterAndPaul));
}

public record FlorenceGenoaTurin : HolidayEnumerable<FlorenceGenoaTurin>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year) 
        => ImmutableList.Create(new Holiday(new DateOnly(year, 6,24), FeastOfStJohnTheBaptist));
}

public record NaplesCampania : HolidayEnumerable<NaplesCampania>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year) 
        => ImmutableList.Create(new Holiday(new DateOnly(year, 9,19), FeastOfStJanuarius));
}

public record Bologna : HolidayEnumerable<Bologna>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year) 
        => ImmutableList.Create(new Holiday(new DateOnly(year, 10,4), FeastOfStPetronius));
}

public record Milan : HolidayEnumerable<Milan>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year) 
        => ImmutableList.Create(new Holiday(new DateOnly(year, 12,7), StAmbrose));
}
