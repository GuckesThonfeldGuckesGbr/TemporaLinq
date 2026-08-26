using FluentAssertions;
using TemporaLinq.Astronomy;

namespace TemporaLinq.Test;

public class DecemberSolsticeCalculationTest
{
    [Theory]
    [InlineData(2023, "2023-12-22")] // 2023-12-22 confirmed a Dec 22 (not the usual Dec 21) year
    [InlineData(2024, "2024-12-21")] // independently verified: Dec 21 09:20 UTC
    [InlineData(2026, "2026-12-21")] // independently verified: Dec 21 20:50 UTC
    public void SolsticeDate_ReturnsKnownReferenceDates(int year, string expectedDate)
    {
        DecemberSolsticeCalculation.SolsticeDate(year).Should().Be(DateOnly.Parse(expectedDate));
    }
}
