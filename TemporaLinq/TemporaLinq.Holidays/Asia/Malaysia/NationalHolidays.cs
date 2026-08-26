using System.Collections.Immutable;
using Memoizer;
using TemporaLinq.Astronomy;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Malaysia;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

/// <summary>
/// Provides Malaysia's federal/national-level public holidays only — the compulsory
/// national holidays plus the gazetted list observed across all states and federal
/// territories (New Year's Day, Chinese New Year, Hari Raya Puasa, Wesak Day, the Yang
/// di-Pertuan Agong's Birthday, Hari Raya Haji, Awal Muharram, Merdeka Day, Malaysia Day,
/// Maulidur Rasul, and Christmas Day). Deliberately out of scope, same spirit as this
/// codebase's Germany federal/state split: each state's own Sultan's/Governor's Birthday
/// or (for the Federal Territories) Federal Territory Day, plus the numerous other
/// state-specific Islamic and cultural holidays (e.g. Kelantan, Terengganu observe several
/// dates unique to their states) — these are jurisdiction-varying and not a single national
/// calendar. Deepavali (Hindu lunisolar calendar) is also deferred, pending a future Hindu
/// calendar calculation mechanism.
/// <para>
/// Computed via <see cref="ChineseLunisolarCalendarCalculation"/> (Chinese New Year),
/// <see cref="HijriCalendarCalculation"/> (Hari Raya Puasa/Eid al-Fitr, Hari Raya
/// Haji/Eid al-Adha, Awal Muharram/Islamic New Year, Maulidur Rasul/Prophet's Birthday — see
/// that class's documentation for the +/-1, rarely +/-2, day real-world moon-sighting
/// approximation caveat), and <see cref="SoutheastAsianBuddhistCalendar.VisakhaBuchaDate"/>
/// (Wesak Day). The Agong's Birthday has been fixed by law to the first Monday of June since
/// 2018 (previously a variable date announced separately each year), so it is formulaic for
/// all years this class is expected to be used for.
/// </para>
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
            new(new DateOnly(year, 5, 1), LabourDay),
            new(SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(year), VesakDay),
            new(Dates.Invariant().From(new DateOnly(year, 6, 1)).First(DayOfWeek.Monday), AgongsBirthday),
            new(new DateOnly(year, 8, 31), IndependenceDay),
            new(new DateOnly(year, 9, 16), NationalDayOfMalaysia),
            new(new DateOnly(year, 12, 25), ChristmasDay),
        };

        holidays.Add(new(ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 1), LunarNewYearsDay));
        holidays.Add(new(ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 2), LunarNewYearsDay));

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1).Select(d => new Holiday(d, IslamicNewYear)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12).Select(d => new Holiday(d, ProphetsBirthday)));

        return holidays.Order().ToImmutableList();
    }
}
