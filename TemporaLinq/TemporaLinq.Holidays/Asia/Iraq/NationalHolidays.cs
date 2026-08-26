using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Iraq;

/// <summary>
/// Provides Iraqi national public holidays. Fixed civil holidays are limited to
/// New Year's Day and Republic Day, per this implementation's deliberately
/// narrow scope (Iraq's broader fixed-holiday calendar is contested/frequently
/// revised and out of scope here). Islamic-calendar holidays are computed from
/// the tabular Hijri calendar via <see cref="HijriCalendarCalculation"/>;
/// real-world moon-sighting announcements can differ from this calculation by
/// +/-1, rarely +/-2, days — and in Iraq specifically, Sunni and Shia religious
/// authorities occasionally announce moon-sighting a day apart from each other,
/// independent of the tabular-calendar gap. This is a known, accepted
/// approximation limitation, not a bug.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var holidays = new List<Holiday>
        {
            new(new DateOnly(year, 1, 1), NewYearsDay),
            new(new DateOnly(year, 7, 14), RepublicDay),
        };

        foreach (var eidAlFitrStart in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
        {
            for (var day = 0; day < 3; day++)
                holidays.Add(new Holiday(eidAlFitrStart.AddDays(day), EidAlFitr));
        }

        foreach (var eidAlAdhaStart in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10))
        {
            for (var day = 0; day < 4; day++)
                holidays.Add(new Holiday(eidAlAdhaStart.AddDays(day), EidAlAdha));
        }

        foreach (var islamicNewYear in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1))
            holidays.Add(new Holiday(islamicNewYear, IslamicNewYear));

        foreach (var ashura in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 10))
            holidays.Add(new Holiday(ashura, AshuraDay));

        foreach (var mawlid in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12))
            holidays.Add(new Holiday(mawlid, ProphetsBirthday));

        return holidays.Order().ToImmutableList();
    }
}
