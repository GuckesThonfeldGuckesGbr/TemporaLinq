using FluentAssertions;
using static System.DayOfWeek;

namespace TemporaLinq.Test;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

public class WeekendsTest
{
    private static readonly Dates Builder = Dates.Invariant();

    [Fact]
    public void DefaultsToWesternWeekend()
    {
        // 2026 starts on Thursday
        var jan2026 = Builder.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 1, 31)).WithContext(Builder);
        var weekends = jan2026.OnlyWeekends().ToList();

        // Should only contain Saturdays and Sundays
        weekends.Should().AllSatisfy(date => date.DayOfWeek.Should().BeOneOf(Saturday, Sunday));

        // Jan 2026 has 31 days, 4 Saturdays + 5 Sundays = 9 weekend days
        weekends.Should().HaveCount(9);
    }

    [Fact]
    public void WithCustomWeekend()
    {
        // Test Arab weekend (Thursday, Friday)
        var jan2026 = Builder
            .From(new DateOnly(2026, 1, 1))
            .To(new DateOnly(2026, 1, 31))
            .WithWeekend(Thursday, Friday);

        var weekends = jan2026.OnlyWeekends().ToList();

        // Should only contain Thursdays and Fridays
        weekends.Should().AllSatisfy(date => date.DayOfWeek.Should().BeOneOf(Thursday, Friday));

        // Jan 2026: 5 Thursdays + 5 Fridays = 10 weekend days
        weekends.Should().HaveCount(10);
    }
}
