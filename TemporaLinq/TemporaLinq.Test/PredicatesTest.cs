using de.baggerbagger.TemporaLinq;
using FluentAssertions;
using static System.DayOfWeek;

namespace TemporaLinq.Test;

public class PredicatesTest
{
    private static readonly Dates Builder = Dates.Invariant();
    private readonly IDateEnumerable _year2026 = Builder
        .From(new DateOnly(2026, 1, 1))
        .Take(365)
        .WithContext(Builder);

    [Fact]
    public void FiltersByWeekday()
    {
        var sundays2026 = _year2026.Only(Sunday).ToList();
        var sundaysAndMondays2026 = _year2026.Only(Sunday, Monday).ToList();

        var allWorkdays2026 = _year2026.Except(Sunday).ToList();
        var allWeekdays2026 = _year2026.Except(Saturday, Sunday).ToList();

        sundays2026.Should().HaveCount(52);
        sundays2026.Should().AllSatisfy(date => date.DayOfWeek.Should().Be(Sunday));

        sundaysAndMondays2026.Should().HaveCount(104);
        sundaysAndMondays2026.Should().AllSatisfy(date => date.DayOfWeek.Should().BeOneOf(Sunday, Monday));

        allWorkdays2026.Should().HaveCount(313);
        allWorkdays2026.Should().AllSatisfy(date => date.DayOfWeek.Should().NotBe(Sunday));

        allWeekdays2026.Should().HaveCount(261);
        allWeekdays2026.Should().AllSatisfy(date => date.DayOfWeek.Should().NotBe(Saturday));
        allWeekdays2026.Should().AllSatisfy(date => date.DayOfWeek.Should().NotBe(Sunday));
    }

    [Fact]
    public void SkippingDays()
    {
        var everyOtherSunday2026 = _year2026.Only(Sunday).EveryNth(2).ToList();

        everyOtherSunday2026.Should().HaveCount(26);
        everyOtherSunday2026.Should().AllSatisfy(date => date.DayOfWeek.Should().Be(Sunday));
    }
}