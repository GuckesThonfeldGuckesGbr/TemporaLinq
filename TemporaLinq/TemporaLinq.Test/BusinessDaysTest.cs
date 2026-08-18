using FluentAssertions;
using static System.DayOfWeek;

namespace TemporaLinq.Test;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

public class BusinessDaysTest
{
    private static readonly Dates Builder = Dates.Invariant();

    [Fact]
    public void DefaultsToWesternWeekend()
    {
        // 2026 starts on Thursday
        var jan2026 = Builder.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 1, 31)).WithContext(Builder);
        var businessDays = jan2026.BusinessDays().ToList();

        // Should exclude Saturdays and Sundays
        businessDays.Should().AllSatisfy(date => date.DayOfWeek.Should().NotBe(Saturday));
        businessDays.Should().AllSatisfy(date => date.DayOfWeek.Should().NotBe(Sunday));
        
        // Jan 2026 has 31 days, 4,5 weekends = 9 weekend days, 22 business days
        businessDays.Should().HaveCount(22);
    }

    [Fact]
    public void WithCustomWeekend()
    {
        // Test Arab weekend (Thursday, Friday)
        var jan2026 = Builder
            .From(new DateOnly(2026, 1, 1))
            .To(new DateOnly(2026, 1, 31))
            .WithWeekend(Thursday, Friday);
        
        var businessDays = jan2026.BusinessDays().ToList();

        // Should exclude Thursdays and Fridays
        businessDays.Should().AllSatisfy(date => date.DayOfWeek.Should().NotBe(Thursday));
        businessDays.Should().AllSatisfy(date => date.DayOfWeek.Should().NotBe(Friday));

        // Jan 2026: 31 days, 10 Thursdays + Fridays = 10 excluded
        businessDays.Should().HaveCount(21);
    }

    [Fact]
    public void WithContext_PreservesCalendarConfig()
    {
        var dates = Builder
            .From(new DateOnly(2026, 1, 1))
            .To(new DateOnly(2026, 1, 31))
            .WithWeekend(Thursday, Friday);

        // Apply LINQ operations that return IEnumerable<DateOnly>
        var filtered = dates
            .Where(d => d.Day is >= 1 and <= 15)
            .Take(10)
            .WithContext(dates);

        // Should have the config from the original Dates object
        filtered.Config.WeekendDays.Should().BeEquivalentTo([Thursday, Friday]);

        // BusinessDays should use the preserved config
        var businessDays = filtered.BusinessDays().ToList();
        businessDays.Should().AllSatisfy(date => date.DayOfWeek.Should().NotBe(Thursday));
        businessDays.Should().AllSatisfy(date => date.DayOfWeek.Should().NotBe(Friday));
    }
}
