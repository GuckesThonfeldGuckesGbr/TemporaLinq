using FluentAssertions;

namespace TemporaLinq.Test;

using TemporaLinq.Dates;
using TemporaLinq.Times;
using Dates = TemporaLinq.Dates.Dates;
using Times = TemporaLinq.Times.Times;

public class DateTimeCombinatorsTest
{
    [Fact]
    public void On_ProducesCrossJoinOfDatesAndTimes()
    {
        var dates = Dates.Invariant().From(new DateOnly(2026, 1, 5)).To(new DateOnly(2026, 1, 6)); // Mon, Tue (To is inclusive)
        var times = Times.From(new TimeOnly(8, 0)).To(new TimeOnly(10, 0)).Every(TimeSpan.FromHours(1)); // 8, 9

        var result = dates.On(times).ToList();

        result.Should().Equal(
            new DateTime(2026, 1, 5, 8, 0, 0),
            new DateTime(2026, 1, 5, 9, 0, 0),
            new DateTime(2026, 1, 6, 8, 0, 0),
            new DateTime(2026, 1, 6, 9, 0, 0));
    }

    [Fact]
    public void On_ResultIsInAscendingOrder()
    {
        var dates = Dates.Invariant().From(new DateOnly(2026, 1, 5)).To(new DateOnly(2026, 1, 9)); // To is inclusive
        var times = Times.From(new TimeOnly(8, 0)).To(new TimeOnly(16, 0)).Every(TimeSpan.FromHours(1));

        var result = dates.On(times).ToList();

        result.Should().BeInAscendingOrder();
        result.Should().HaveCount(5 * 8);
    }

    [Fact]
    public void MergingTwoOnResults_CombinesDifferentSchedulesPerWeekdayGroup()
    {
        var start = new DateOnly(2026, 1, 5); // Monday
        var end = new DateOnly(2026, 1, 10);  // inclusive; Only(...) below narrows each sequence to its weekdays

        var monToThu = Dates.Invariant().From(start).To(end)
            .Only(DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday)
            .On(Times.From(new TimeOnly(8, 0)).To(new TimeOnly(10, 0)).Every(TimeSpan.FromHours(1)));

        var friday = Dates.Invariant().From(start).To(end)
            .Only(DayOfWeek.Friday)
            .On(Times.From(new TimeOnly(8, 0)).To(new TimeOnly(9, 0)).Every(TimeSpan.FromHours(1)));

        var schedule = monToThu.Merge(friday).ToList();

        schedule.Should().Equal(
            new DateTime(2026, 1, 5, 8, 0, 0),
            new DateTime(2026, 1, 5, 9, 0, 0),
            new DateTime(2026, 1, 6, 8, 0, 0),
            new DateTime(2026, 1, 6, 9, 0, 0),
            new DateTime(2026, 1, 7, 8, 0, 0),
            new DateTime(2026, 1, 7, 9, 0, 0),
            new DateTime(2026, 1, 8, 8, 0, 0),
            new DateTime(2026, 1, 8, 9, 0, 0),
            new DateTime(2026, 1, 9, 8, 0, 0));
    }
}
