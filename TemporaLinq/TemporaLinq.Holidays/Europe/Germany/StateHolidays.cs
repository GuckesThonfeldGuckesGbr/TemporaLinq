using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Germany;

using static Operations;

public record Augsburg : HolidayEnumerable<Augsburg>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var bavaria = BavariaCatholic.Create().From(new DateOnly(year, 1, 1)).To(new DateOnly(year, 12, 31));
        var assumption =
            new[] { new Holiday(new DateOnly(year, 8, 8), AugsburgPeaceFestival) }
                .AsMonotonicallyAscendingEnumerable();
        return Merge([bavaria, assumption]).ToImmutableList();
    }
};

public record BadenWuerttemberg : HolidayEnumerable<BadenWuerttemberg>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        return ImmutableList.Create(
            new Holiday(new DateOnly(year, 1, 6), Epiphany),
            new Holiday(easter.AddDays(60), CorpusChristi),
            new Holiday(new DateOnly(year, 11, 1), AllSaintsDay));
    }
};

public record BavariaCatholic : HolidayEnumerable<BavariaCatholic>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var protestant = BavariaProtestant.Create()
            .From(new DateOnly(year, 1, 1))
            .To(new DateOnly(year, 12, 31));
        var assumption =
            new[] { new Holiday(new DateOnly(year, 8, 15), AssumptionDay) }
                .AsMonotonicallyAscendingEnumerable();
        return Merge([protestant, assumption]).ToImmutableList();
    }
}

public record BavariaProtestant : HolidayEnumerable<BavariaProtestant>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        return ImmutableList.Create(
            new Holiday(new DateOnly(year, 1, 6), Epiphany),
            new Holiday(easter.AddDays(60), CorpusChristi),
            new Holiday(new DateOnly(year, 11, 1), AllSaintsDay));
    }
}

public record Berlin : HolidayEnumerable<Berlin>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(
            new Holiday(new DateOnly(year, 3, 8), InternationalWomensDay),
            new Holiday(new DateOnly(year, 9, 20), WorldChildrensDay));
}

public record Brandenburg : HolidayEnumerable<Brandenburg>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 10, 31), ReformationDay));
}

public record Bremen : HolidayEnumerable<Bremen>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 10, 31), ReformationDay));
}

public record Hamburg : HolidayEnumerable<Hamburg>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 10, 31), ReformationDay));
}

public record Hesse : HolidayEnumerable<Hesse>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        return ImmutableList.Create(new Holiday(easter.AddDays(60), CorpusChristi));
    }
}

public record LowerSaxony : HolidayEnumerable<LowerSaxony>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 10, 31), ReformationDay));
}

public record MecklenburgVorpommern : HolidayEnumerable<MecklenburgVorpommern>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(
            new Holiday(new DateOnly(year, 3, 8), InternationalWomensDay),
            new Holiday(new DateOnly(year, 10, 31), ReformationDay));
}

public record NorthRhineWestphalia : HolidayEnumerable<NorthRhineWestphalia>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        return ImmutableList.Create(
            new Holiday(easter.AddDays(60), CorpusChristi),
            new Holiday(new DateOnly(year, 11, 1), AllSaintsDay));
    }
}

public record RhinelandPalatinate : HolidayEnumerable<RhinelandPalatinate>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        return ImmutableList.Create(
            new Holiday(easter.AddDays(60), CorpusChristi),
            new Holiday(new DateOnly(year, 11, 1), AllSaintsDay));
    }
}

public record Saarland : HolidayEnumerable<Saarland>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        return ImmutableList.Create(
            new Holiday(easter.AddDays(60), CorpusChristi),
            new Holiday(new DateOnly(year, 8, 15), AssumptionDay),
            new Holiday(new DateOnly(year, 11, 1), AllSaintsDay));
    }
};

public record Saxony : HolidayEnumerable<Saxony>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(
            new Holiday(new DateOnly(year, 10, 31), ReformationDay),
            new Holiday(GetRepentanceAndPrayerDayFor(year), RepentanceAndPrayerDay));

    private static DateOnly GetRepentanceAndPrayerDayFor(int year)
        => Dates
            .OfCalendar(new GregorianCalendar())
            .From(new DateOnly(year, 11, 16))
            .To(new DateOnly(year, 11, 23))
            .First(DayOfWeek.Wednesday);
}

public record SaxonyAnhalt : HolidayEnumerable<SaxonyAnhalt>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(
            new Holiday(new DateOnly(year, 1, 6), Epiphany),
            new Holiday(new DateOnly(year, 10, 31), ReformationDay));
}

public record SchleswigHolstein : HolidayEnumerable<SchleswigHolstein>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 10, 31), ReformationDay));
}

public record Thuringia : HolidayEnumerable<Thuringia>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 9, 20), WorldChildrensDay));
}