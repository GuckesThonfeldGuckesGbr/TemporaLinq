using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.SaudiArabia;

/// <summary>
/// Provides Saudi Arabian national public holidays. Eid al-Fitr and Eid al-Adha
/// (including the Day of Arafah) are computed from the tabular Hijri calendar via
/// <see cref="HijriCalendarCalculation"/>; real-world moon-sighting announcements
/// can differ from this calculation by +/-1, rarely +/-2, days. Saudi Arabia has
/// no statutory Gregorian New Year's Day holiday.
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
            new(new DateOnly(year, 9, 23), NationalDayOfSaudiArabia),
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

        return holidays.Order().ToImmutableList();
    }
}
