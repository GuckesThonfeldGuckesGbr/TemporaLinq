using FluentAssertions;

namespace TemporaLinq.Test;

using TemporaLinq.Times;
using Times = TemporaLinq.Times.Times;

public class TimesTest
{
    [Fact]
    public void FromToEvery_GeneratesTicksAtFixedInterval()
    {
        var times = Times.From(new TimeOnly(8, 0)).To(new TimeOnly(12, 0)).Every(TimeSpan.FromHours(1)).ToList();

        times.Should().Equal(
            new TimeOnly(8, 0),
            new TimeOnly(9, 0),
            new TimeOnly(10, 0),
            new TimeOnly(11, 0));
    }

    [Fact]
    public void To_IsExclusive()
    {
        var times = Times.From(new TimeOnly(8, 0)).To(new TimeOnly(9, 0)).Every(TimeSpan.FromHours(1)).ToList();

        times.Should().Equal(new TimeOnly(8, 0));
    }

    [Fact]
    public void EmptyWhenStartTimeIsAfterEndTime()
    {
        var times = Times.From(new TimeOnly(12, 0)).To(new TimeOnly(8, 0)).Every(TimeSpan.FromHours(1)).ToList();

        times.Should().BeEmpty();
    }

    [Fact]
    public void EmptyWhenStartTimeEqualsEndTime()
    {
        var times = Times.From(new TimeOnly(8, 0)).To(new TimeOnly(8, 0)).Every(TimeSpan.FromHours(1)).ToList();

        times.Should().BeEmpty();
    }

    [Fact]
    public void Every_ThrowsWhenStepIsZero()
    {
        var act = () => Times.From(new TimeOnly(8, 0)).Every(TimeSpan.Zero);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Every_ThrowsWhenStepIsNegative()
    {
        var act = () => Times.From(new TimeOnly(8, 0)).Every(TimeSpan.FromHours(-1));

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
