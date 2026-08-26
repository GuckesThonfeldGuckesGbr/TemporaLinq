using System.Collections.Immutable;
using Memoizer;
using TemporaLinq.Astronomy;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Thailand;

/// <summary>
/// Provides Thai national public holidays. Makha Bucha, Visakha Bucha, and Asalha
/// Bucha are computed via <see cref="SoutheastAsianBuddhistCalendar"/>. Khao Phansa
/// (the start of Buddhist Lent, the day after Asalha Bucha) is included as it is
/// an actual government-sector public holiday date in Thailand's official calendar
/// each year — though, unlike the other holidays here, it is observed by government
/// offices only; banks and most of the private sector remain open. One-off
/// Cabinet-approved bonus holidays (e.g. an extra day added around New Year in some
/// years) are not modeled, since they are not a stable per-year formula.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var makhaBucha = SoutheastAsianBuddhistCalendar.MakhaBuchaDate(year);
        var visakhaBucha = SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(year);
        var asalhaBucha = SoutheastAsianBuddhistCalendar.AsalhaBuchaDate(year);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(makhaBucha, MakhaBuchaDay),
                new(new DateOnly(year, 4, 6), ChakriMemorialDay),
                new(new DateOnly(year, 4, 13), SongkranDay),
                new(new DateOnly(year, 4, 14), SongkranDay),
                new(new DateOnly(year, 4, 15), SongkranDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 4), CoronationDayOfThailand),
                new(visakhaBucha, VisakhaBuchaDay),
                new(new DateOnly(year, 6, 3), QueensBirthdayOfThailand),
                new(new DateOnly(year, 7, 28), KingsBirthdayOfThailand),
                new(asalhaBucha, AsalhaBuchaDay),
                new(asalhaBucha.AddDays(1), KhaoPhansaDay),
                new(new DateOnly(year, 8, 12), MothersDayOfThailand),
                new(new DateOnly(year, 10, 13), KingBhumibolMemorialDay),
                new(new DateOnly(year, 10, 23), ChulalongkornDay),
                new(new DateOnly(year, 12, 5), NationalDayOfThailand),
                new(new DateOnly(year, 12, 10), ConstitutionDayOfThailand),
                new(new DateOnly(year, 12, 31), NewYearsEve),
            }
            .Order()
            .ToImmutableList();
    }
}
