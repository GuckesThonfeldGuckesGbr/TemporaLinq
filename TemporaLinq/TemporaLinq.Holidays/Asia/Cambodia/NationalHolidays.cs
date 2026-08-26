using System.Collections.Immutable;
using Memoizer;
using TemporaLinq.Astronomy;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Cambodia;

/// <summary>
/// Provides Cambodian national public holidays: fixed civil days and Visak Bochea Day, computed
/// from the Theravada Buddhist lunisolar calendar via
/// <see cref="SoutheastAsianBuddhistCalendar.VisakhaBuchaDate"/>. Khmer New Year (a solar-calendar
/// festival, not the lunisolar Buddhist calendar) is modeled as fixed April 14-16 dates, following
/// the same convention already used for Sri Lanka's Sinhala/Tamil New Year, rather than computing
/// solar ingress.
///
/// Cambodia's own government/religious authorities occasionally publish a Visak Bochea date that
/// diverges from the astronomical full-moon calculation by more than the usual +/-1 day seen
/// elsewhere in this project (e.g. Hijri moon-sighting variance) — this is the same kind of
/// local-authority variance already documented as a caveat for other countries (Turkey, Iraq), so
/// the astronomical calculation is used here rather than an unverifiable secondhand published date.
///
/// The following are deliberately out of scope:
/// <list type="bullet">
/// <item>Meak Bochea (Makha Bucha) — was on Cambodia's official statutory holiday list through
/// 2019 but was removed starting 2020 and has not returned; not implemented because it is not
/// currently an official holiday, not because it is uncomputable.</item>
/// <item>Asalha Bucha — never on Cambodia's statutory civil-servant/worker holiday list.</item>
/// <item>Pchum Ben (Ancestors' Day) — a distinct Khmer lunar-calendar festival that does not map
/// onto Makha/Visakha/Asalha Bucha.</item>
/// <item>Water Festival (Bon Om Touk) — tied to a full moon of the traditional Khmer calendar but
/// not one of the three Buddhist holy days this project computes.</item>
/// <item>Royal Ploughing Ceremony — its date is set by royal astrologers each year, not a fixed
/// formula.</item>
/// </list>
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
            new(new DateOnly(year, 1, 7), VictoryOverGenocideDay),
            new(new DateOnly(year, 3, 8), InternationalWomensDay),
            new(new DateOnly(year, 4, 14), KhmerNewYear),
            new(new DateOnly(year, 4, 15), KhmerNewYear),
            new(new DateOnly(year, 4, 16), KhmerNewYear),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(year), VisakBocheaDay),
            new(new DateOnly(year, 5, 14), BirthdayOfKingNorodomSihamoni),
            new(new DateOnly(year, 6, 18), BirthdayOfQueenMotherNorodomMonineath),
            new(new DateOnly(year, 9, 24), ConstitutionDayOfCambodia),
            new(new DateOnly(year, 10, 15), CommemorationDayOfKingFatherNorodomSihanouk),
            new(new DateOnly(year, 10, 29), CoronationDayOfKingNorodomSihamoni),
            new(new DateOnly(year, 11, 9), IndependenceDay),
            new(new DateOnly(year, 12, 29), PeaceDayOfCambodia),
        };

        return holidays.Order().ToImmutableList();
    }
}
