using FluentAssertions;
using TemporaLinq.Astronomy;

namespace TemporaLinq.Test;

public class LunarPhaseCalculationTest
{
    [Fact]
    public void FullMoonsInGregorianYear_2024_MatchesPublishedReferenceDates()
    {
        var expected = new[]
        {
            new DateOnly(2024, 1, 25), new DateOnly(2024, 2, 24), new DateOnly(2024, 3, 25),
            new DateOnly(2024, 4, 23), new DateOnly(2024, 5, 23), new DateOnly(2024, 6, 21),
            new DateOnly(2024, 7, 21), new DateOnly(2024, 8, 19), new DateOnly(2024, 9, 17),
            new DateOnly(2024, 10, 17), new DateOnly(2024, 11, 15), new DateOnly(2024, 12, 15),
        };

        var actual = LunarPhaseCalculation.FullMoonsInGregorianYear(2024).ToList();

        actual.Should().HaveCount(12);
        for (var i = 0; i < expected.Length; i++)
        {
            Math.Abs(actual[i].DayNumber - expected[i].DayNumber).Should().BeLessThanOrEqualTo(1,
                $"full moon #{i} in 2024 should be within 1 day of the published reference date");
        }
    }

    [Fact]
    public void FullMoonsInGregorianYear_2026_HasThirteenFullMoonsAndMatchesReferenceDates()
    {
        // 2026 has 13 full moons (two in May: May 1 and May 31), since the ~354-day span of
        // 12 lunations is shorter than a calendar year and periodically an extra one fits.
        var expected = new[]
        {
            new DateOnly(2026, 1, 3), new DateOnly(2026, 2, 1), new DateOnly(2026, 3, 3),
            new DateOnly(2026, 4, 1), new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31),
            new DateOnly(2026, 6, 29), new DateOnly(2026, 7, 29), new DateOnly(2026, 8, 28),
            new DateOnly(2026, 9, 26), new DateOnly(2026, 10, 26), new DateOnly(2026, 11, 24),
            new DateOnly(2026, 12, 23),
        };

        var actual = LunarPhaseCalculation.FullMoonsInGregorianYear(2026).ToList();

        actual.Should().HaveCount(13);
        for (var i = 0; i < expected.Length; i++)
        {
            Math.Abs(actual[i].DayNumber - expected[i].DayNumber).Should().BeLessThanOrEqualTo(1,
                $"full moon #{i} in 2026 should be within 1 day of the published reference date");
        }
    }

    [Fact]
    public void FullMoonsInGregorianYear_ReturnsDatesInAscendingOrderWithinTheYear()
    {
        var actual = LunarPhaseCalculation.FullMoonsInGregorianYear(2025).ToList();

        actual.Should().BeInAscendingOrder();
        actual.Should().OnlyContain(d => d.Year == 2025);
    }

    [Fact]
    public void NewMoonsInGregorianYear_2024_MatchesPublishedReferenceDates()
    {
        // 2024 has 13 new moons (two in December), independently confirmed: the first new moon
        // of 2024 was Jan 11, and the year's last (a "black moon", the second new moon within
        // December) was Dec 30.
        var expected = new[]
        {
            new DateOnly(2024, 1, 11), new DateOnly(2024, 2, 9), new DateOnly(2024, 3, 10),
            new DateOnly(2024, 4, 8), new DateOnly(2024, 5, 8), new DateOnly(2024, 6, 6),
            new DateOnly(2024, 7, 5), new DateOnly(2024, 8, 4), new DateOnly(2024, 9, 3),
            new DateOnly(2024, 10, 2), new DateOnly(2024, 11, 1), new DateOnly(2024, 12, 1),
            new DateOnly(2024, 12, 30),
        };

        var actual = LunarPhaseCalculation.NewMoonsInGregorianYear(2024).ToList();

        actual.Should().HaveCount(13);
        for (var i = 0; i < expected.Length; i++)
        {
            Math.Abs(actual[i].DayNumber - expected[i].DayNumber).Should().BeLessThanOrEqualTo(1,
                $"new moon #{i} in 2024 should be within 1 day of the published reference date");
        }
    }

    [Fact]
    public void NewMoonsInGregorianYear_ReturnsDatesInAscendingOrderWithinTheYear()
    {
        var actual = LunarPhaseCalculation.NewMoonsInGregorianYear(2025).ToList();

        actual.Should().BeInAscendingOrder();
        actual.Should().OnlyContain(d => d.Year == 2025);
        actual.Should().HaveCountGreaterThanOrEqualTo(12);
    }
}
