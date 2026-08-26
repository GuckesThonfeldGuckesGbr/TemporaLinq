using System.Collections.Immutable;
using Memoizer;
using TemporaLinq.Astronomy;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Myanmar;

/// <summary>
/// Provides Myanmar national public holidays. The Southeast Asian Buddhist
/// lunisolar calendar underlying <see cref="SoutheastAsianBuddhistCalendar"/> is
/// the same calendar Myanmar uses, just with different traditional month names:
/// the full moon of Tabaung is the same event as Makha Bucha, Kason's full moon
/// the same as Visakha Bucha (Buddha's Birthday / Vesak), and Waso's full moon the
/// same as Asalha Bucha (Dhamma Day, start of Buddhist Lent) — so those methods
/// are reused directly here under Myanmar's local names. Later Burmese lunar
/// months (Thadingyut, Tazaungmone — used for National Day) are out of scope,
/// since <see cref="SoutheastAsianBuddhistCalendar"/> only computes months 3/6/8.
/// Thingyan (the Myanmar New Year water festival) is modeled as its traditional
/// fixed 4-day span (Apr 13 Eve through Apr 16 New Year's Day); occasional
/// government decrees adding extra bonus days around it are not modeled, the same
/// as Thailand's occasional one-off bonus holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var tabaungFullMoon = SoutheastAsianBuddhistCalendar.MakhaBuchaDate(year);
        var kasonFullMoon = SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(year);
        var wasoFullMoon = SoutheastAsianBuddhistCalendar.AsalhaBuchaDate(year);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 1, 4), IndependenceDay),
                new(new DateOnly(year, 2, 12), UnionDayOfMyanmar),
                new(new DateOnly(year, 3, 2), PeasantsDay),
                new(tabaungFullMoon, TabaungFullMoonDay),
                new(new DateOnly(year, 3, 27), ArmedForcesDayOfMyanmar),
                new(new DateOnly(year, 4, 13), ThingyanDay),
                new(new DateOnly(year, 4, 14), ThingyanDay),
                new(new DateOnly(year, 4, 15), ThingyanDay),
                new(new DateOnly(year, 4, 16), ThingyanDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(kasonFullMoon, KasonFullMoonDay),
                new(new DateOnly(year, 7, 19), MartyrsDayOfMyanmar),
                new(wasoFullMoon, WasoFullMoonDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
