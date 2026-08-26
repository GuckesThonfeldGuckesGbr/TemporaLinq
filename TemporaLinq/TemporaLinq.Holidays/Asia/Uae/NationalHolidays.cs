using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Uae;

/// <summary>
/// Provides United Arab Emirates national public holidays. Islamic-calendar
/// holidays are computed from the tabular Hijri calendar via
/// <see cref="HijriCalendarCalculation"/>; real-world moon-sighting announcements
/// can differ from this calculation by +/-1, rarely +/-2, days.
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
            new(new DateOnly(year, 12, 1), CommemorationDayOfUae),
            new(new DateOnly(year, 12, 2), NationalDayOfUae),
            new(new DateOnly(year, 12, 3), NationalDayOfUae),
        };

        foreach (var eidAlFitrStart in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
        {
            for (var day = 0; day < 4; day++)
                holidays.Add(new Holiday(eidAlFitrStart.AddDays(day), EidAlFitr));
        }

        foreach (var arafahDay in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 9))
        {
            holidays.Add(new Holiday(arafahDay, ArafahDay));
            for (var day = 1; day <= 3; day++)
                holidays.Add(new Holiday(arafahDay.AddDays(day), EidAlAdha));
        }

        foreach (var islamicNewYear in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1))
            holidays.Add(new Holiday(islamicNewYear, IslamicNewYear));

        foreach (var mawlid in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12))
            holidays.Add(new Holiday(mawlid, ProphetsBirthday));

        return holidays.Order().ToImmutableList();
    }
}
