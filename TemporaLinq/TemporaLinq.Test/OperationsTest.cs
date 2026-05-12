using de.baggerbagger.TemporaLinq;
using FluentAssertions;

namespace TemporaLinq.Test;

public class OperationsTest
{
    [Fact]
    public void MergingTwoStreams_YieldsAllDatesInTheRightOrder()
    {
        var first = Dates.Invariant().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 1, 3));
        var second = Dates.Invariant().From(new DateOnly(2026, 1, 6)).To(new DateOnly(2026, 1, 8));

        var merged = first.Merge(second);

        merged.Should().ContainInOrder(
            new DateOnly(2026, 1, 1), 
            new DateOnly(2026, 1, 2), 
            new DateOnly(2026, 1, 6),
            new DateOnly(2026, 1, 7));
    }

    [Fact]
    public void MergingThreeStreams_YieldsAllDatesInTheRightOrder()
    {
        var first = Dates.Invariant().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 1, 3));
        var second = Dates.Invariant().From(new DateOnly(2026, 3, 1)).To(new DateOnly(2026, 3, 6));
        var third = Dates.Invariant().From(new DateOnly(2026, 2, 1)).To(new DateOnly(2026, 2, 2));

        var merged = Operations.Merge([first, second, third]);

        merged.Should().ContainInOrder(
            new DateOnly(2026, 1, 1), 
            new DateOnly(2026, 1, 2), 
            new DateOnly(2026, 2, 1),
            new DateOnly(2026, 3, 1),
            new DateOnly(2026, 3, 2),
            new DateOnly(2026, 3, 3),
            new DateOnly(2026, 3, 4),
            new DateOnly(2026, 3, 5),
            new DateOnly(2026, 3, 6));
    }
}