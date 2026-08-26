using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Kuwait;

/// <summary>
/// Provides Kuwaiti national public holidays. Islamic New Year, Eid al-Fitr,
/// Waqfat Arafat, Eid al-Adha, and Prophet's Birthday follow Kuwait Labour Law
/// Article 68; Ashura is included as a government-observed holiday even though
/// it is not itemized in that private-sector article. Islamic-calendar holidays
/// are computed from the tabular Hijri calendar via
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
            new(new DateOnly(year, 2, 25), NationalDayOfKuwait),
            new(new DateOnly(year, 2, 26), LiberationDay),
        };

        foreach (var eidAlFitrStart in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
        {
            for (var day = 0; day < 3; day++)
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

        foreach (var ashura in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 10))
            holidays.Add(new Holiday(ashura, AshuraDay));

        foreach (var mawlid in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12))
            holidays.Add(new Holiday(mawlid, ProphetsBirthday));

        return holidays.Order().ToImmutableList();
    }
}
