using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Albania;

/// <summary>
/// Provides Albanian national public holidays. Albania officially observes major
/// holidays of all its main religious communities (Muslim, Orthodox Christian,
/// Catholic) as state holidays, alongside secular/civil ones. Eid al-Fitr and
/// Eid al-Adha are computed from the Hijri calendar via
/// <see cref="HijriCalendarCalculation"/> — a deterministic approximation that can
/// differ by +/-1, rarely +/-2, days from the real-world moon-sighting-confirmed
/// date.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var catholicEaster = EasterSundayCalculation.Christian.ForYear(year);
        var orthodoxEaster = EasterSundayCalculation.ChristianOrthodox.ForYear(year);

        var holidays = new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 1, 2), NewYearsDay),
                new(new DateOnly(year, 3, 14), SummerDay),
                new(new DateOnly(year, 3, 22), NevruzDay),
                new(catholicEaster, EasterSunday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(orthodoxEaster, EasterSunday),
                new(new DateOnly(year, 9, 5), MotherTeresaDay),
                new(new DateOnly(year, 11, 22), AlphabetDay),
                new(new DateOnly(year, 11, 28), IndependenceDay),
                new(new DateOnly(year, 11, 29), LiberationDay),
                new(new DateOnly(year, 12, 8), NationalYouthDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)
            .Select(date => new Holiday(date, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10)
            .Select(date => new Holiday(date, EidAlAdha)));

        return holidays.Order().ToImmutableList();
    }
}
