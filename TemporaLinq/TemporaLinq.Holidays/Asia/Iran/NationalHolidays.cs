using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Iran;

/// <summary>
/// Provides Iranian national public holidays: Persian solar civil-calendar holidays (via
/// <see cref="PersianCalendarCalculation"/>) plus Hijri lunar-calendar Shia religious
/// observances (via <see cref="HijriCalendarCalculation"/>). Hijri dates use a tabular
/// (arithmetic) approximation that can differ from real-world moon-sighting-confirmed dates
/// by up to a day or two - see that type's XML doc. This is most relevant for Eid al-Fitr and
/// Eid al-Adha. Scope is limited to the holidays explicitly in scope for this implementation
/// pass; Iran's full official calendar includes several additional Shia observance days
/// (e.g. martyrdom of Imam Ali, birth of Imam Mahdi) that are not included here.
///
/// Note: Mawlid al-Nabi is observed here on 17 Rabi' al-awwal, the Shia date (differing
/// deliberately from the Sunni practice of 12 Rabi' al-awwal).
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
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 1), NowruzDay),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 2), NowruzDay),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 3), NowruzDay),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 4), NowruzDay),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 12), IslamicRepublicDayOfIran),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 13), NaturesDayOfIran),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 3, 14), DeathOfKhomeini),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 3, 15), KhordadNationalUprisingDay),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 11, 22), IslamicRevolutionDayOfIran),
        };

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 9))
            holidays.Add(new(date, Tasua));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 10))
            holidays.Add(new(date, AshuraDay));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 2, 20))
            holidays.Add(new(date, Arbaeen));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 17))
            holidays.Add(new(date, MawlidAlNabi));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
            holidays.Add(new(date, EidAlFitr));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2))
            holidays.Add(new(date, EidAlFitr));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10))
            holidays.Add(new(date, EidAlAdha));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 18))
            holidays.Add(new(date, EidAlGhadir));

        return holidays.Order().ToImmutableList();
    }
}
