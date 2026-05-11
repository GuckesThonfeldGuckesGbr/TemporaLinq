using de.baggerbagger.TemporaLinq;
using FluentAssertions;
using static System.DayOfWeek;

namespace TemporaLinq.Test;

public class LinqExtensionsTest
{
    private static readonly Dates Builder = Dates.Invariant();

    private static IDateEnumerable CreateDateRange()
        => Builder.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 1, 31)).WithContext(Builder);

    [Fact]
    public void Where_PreservesConfig()
    {
        var dates = CreateDateRange();
        var filtered = dates.Where(d => d.Day >= 10);

        filtered.Config.Should().BeSameAs(dates.Config);
        filtered.Should().AllSatisfy(d => d.Day.Should().BeGreaterThanOrEqualTo(10));
    }

    [Fact]
    public void Take_PreservesConfig()
    {
        var dates = CreateDateRange();
        var taken = dates.Take(10);

        taken.Config.Should().BeSameAs(dates.Config);
        taken.Should().HaveCount(10);
    }

    [Fact]
    public void Skip_PreservesConfig()
    {
        var dates = CreateDateRange();
        var skipped = dates.Skip(10);

        skipped.Config.Should().BeSameAs(dates.Config);
        skipped.Should().HaveCount(21);
        skipped.First().Should().Be(new DateOnly(2026, 1, 11));
    }

    [Fact]
    public void Select_PreservesConfig()
    {
        var dates = CreateDateRange();
        var selected = dates.Select(d => d.AddDays(1));

        selected.Config.Should().BeSameAs(dates.Config);
        selected.First().Should().Be(new DateOnly(2026, 1, 2));
    }

    [Fact]
    public void ChainedOperations_PreserveConfig()
    {
        var dates = CreateDateRange()
            .Where(d => d.Day >= 5)
            .Skip(5)
            .Take(10);

        dates.Config.Should().NotBeNull();
        dates.Should().HaveCount(10);
        dates.First().Should().Be(new DateOnly(2026, 1, 10));
    }

    [Fact]
    public void BusinessDays_AfterLinqOperations()
    {
        var dates = CreateDateRange()
            .Where(d => d.Day >= 1 && d.Day <= 15)
            .Take(10)
            .BusinessDays();

        dates.Should().AllSatisfy(d => d.DayOfWeek.Should().NotBe(Saturday));
        dates.Should().AllSatisfy(d => d.DayOfWeek.Should().NotBe(Sunday));
    }

    [Fact]
    public void Distinct_PreservesConfig()
    {
        var dates = CreateDateRange();
        var distinct = dates.Distinct();

        distinct.Config.Should().BeSameAs(dates.Config);
    }

    [Fact]
    public void Reverse_PreservesConfig()
    {
        var dates = CreateDateRange();
        var reversed = dates.Reverse();

        reversed.Config.Should().BeSameAs(dates.Config);
        reversed.First().Should().Be(new DateOnly(2026, 1, 31));
    }
    
    
}
