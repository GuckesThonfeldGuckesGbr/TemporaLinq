using FluentAssertions;
using TemporaLinq.Astronomy;

namespace TemporaLinq.Test;

public class SoutheastAsianBuddhistCalendarTest
{
    [Theory]
    [InlineData(2024, "2024-02-24", "2024-05-22", "2024-07-20")]
    [InlineData(2025, "2025-02-12", "2025-05-12", "2025-07-10")]
    [InlineData(2026, "2026-03-03", "2026-05-31", "2026-07-29")] // confirmed leap-month year
    public void HolyDays_MatchPublishedReferenceDates(
        int year, string makhaBucha, string visakhaBucha, string asalhaBucha)
    {
        var expectedMakha = DateOnly.Parse(makhaBucha);
        var expectedVisakha = DateOnly.Parse(visakhaBucha);
        var expectedAsalha = DateOnly.Parse(asalhaBucha);

        Math.Abs(SoutheastAsianBuddhistCalendar.MakhaBuchaDate(year).DayNumber - expectedMakha.DayNumber)
            .Should().BeLessThanOrEqualTo(1, $"Makha Bucha {year}");
        Math.Abs(SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(year).DayNumber - expectedVisakha.DayNumber)
            .Should().BeLessThanOrEqualTo(1, $"Visakha Bucha {year}");
        Math.Abs(SoutheastAsianBuddhistCalendar.AsalhaBuchaDate(year).DayNumber - expectedAsalha.DayNumber)
            .Should().BeLessThanOrEqualTo(1, $"Asalha Bucha {year}");
    }
}
