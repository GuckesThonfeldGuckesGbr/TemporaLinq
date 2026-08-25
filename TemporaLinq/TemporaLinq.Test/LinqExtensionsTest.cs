using FluentAssertions;
using static System.DayOfWeek;

namespace TemporaLinq.Test;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

public class LinqExtensionsTest
{
    private static readonly Dates Builder = Dates.Invariant();

    private static IDateEnumerable CreateDateRange()
        => Builder.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 1, 31));

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
            .Where(d => d.Day is >= 1 and <= 15)
            .Take(10)
            .OnlyBusinessDays();

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
    public void TakeWhile_PreservesConfig()
    {
        var dates = CreateDateRange();
        var taken = dates.TakeWhile(d => d.Day <= 10);

        taken.Config.Should().BeSameAs(dates.Config);
        taken.Should().HaveCount(10);
    }

    [Fact]
    public void TakeWhile_WithIndex_PreservesConfig()
    {
        var dates = CreateDateRange();
        var taken = dates.TakeWhile((_, i) => i < 5);

        taken.Config.Should().BeSameAs(dates.Config);
        taken.Should().HaveCount(5);
    }

    [Fact]
    public void TakeWhile_TakesAllWhenPredicateAlwaysTrue()
    {
        var dates = CreateDateRange();
        var taken = dates.TakeWhile(_ => true);

        taken.Should().HaveCount(31);
    }

    [Fact]
    public void TakeWhile_TakesNoneWhenPredicateAlwaysFalse()
    {
        var dates = CreateDateRange();
        var taken = dates.TakeWhile(_ => false);

        taken.Should().BeEmpty();
    }

    [Fact]
    public void TakeLast_PreservesConfig()
    {
        var dates = CreateDateRange();
        var taken = dates.TakeLast(5);

        taken.Config.Should().BeSameAs(dates.Config);
        taken.Should().HaveCount(5);
        taken.First().Should().Be(new DateOnly(2026, 1, 27));
    }

    [Fact]
    public void TakeLast_TakesAllWhenCountExceedsLength()
    {
        var dates = CreateDateRange();
        var taken = dates.TakeLast(100);

        taken.Should().HaveCount(31);
    }

    [Fact]
    public void TakeLast_ReturnsEmptyWhenCountIsZero()
    {
        var dates = CreateDateRange();
        var taken = dates.TakeLast(0);

        taken.Should().BeEmpty();
    }

    [Fact]
    public void SkipWhile_PreservesConfig()
    {
        var dates = CreateDateRange();
        var skipped = dates.SkipWhile(d => d.Day < 10);

        skipped.Config.Should().BeSameAs(dates.Config);
        skipped.Should().HaveCount(22);
        skipped.First().Should().Be(new DateOnly(2026, 1, 10));
    }

    [Fact]
    public void SkipWhile_WithIndex_PreservesConfig()
    {
        var dates = CreateDateRange();
        var skipped = dates.SkipWhile((_, i) => i < 5);

        skipped.Config.Should().BeSameAs(dates.Config);
        skipped.Should().HaveCount(26);
        skipped.First().Should().Be(new DateOnly(2026, 1, 6));
    }

    [Fact]
    public void SkipWhile_SkipsAllWhenPredicateAlwaysTrue()
    {
        var dates = CreateDateRange();
        var skipped = dates.SkipWhile(_ => true);

        skipped.Should().BeEmpty();
    }

    [Fact]
    public void SkipWhile_SkipsNoneWhenPredicateAlwaysFalse()
    {
        var dates = CreateDateRange();
        var skipped = dates.SkipWhile(_ => false);

        skipped.Should().HaveCount(31);
    }

    [Fact]
    public void SkipLast_PreservesConfig()
    {
        var dates = CreateDateRange();
        var skipped = dates.SkipLast(5);

        skipped.Config.Should().BeSameAs(dates.Config);
        skipped.Should().HaveCount(26);
        skipped.Last().Should().Be(new DateOnly(2026, 1, 26));
    }

    [Fact]
    public void SkipLast_SkipsAllWhenCountExceedsLength()
    {
        var dates = CreateDateRange();
        var skipped = dates.SkipLast(100);

        skipped.Should().BeEmpty();
    }

    [Fact]
    public void SkipLast_SkipsNoneWhenCountIsZero()
    {
        var dates = CreateDateRange();
        var skipped = dates.SkipLast(0);

        skipped.Should().HaveCount(31);
    }

    [Fact]
    public void SelectMany_PreservesConfig()
    {
        var dates = CreateDateRange();
        var selected = dates.SelectMany(d => [d, d.AddDays(1)]);

        selected.Config.Should().BeSameAs(dates.Config);
        selected.Should().HaveCount(62);
    }

    [Fact]
    public void SelectMany_WithIndex_PreservesConfig()
    {
        var dates = CreateDateRange();
        var selected = dates.SelectMany((d, _) => [d]);

        selected.Config.Should().BeSameAs(dates.Config);
        selected.Should().HaveCount(31);
    }

    [Fact]
    public void SelectMany_FlattenMultipleDates()
    {
        var dates = Builder.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 1, 2));
        var selected = dates.SelectMany(d => [d, d.AddDays(1), d.AddDays(2)]);

        selected.Should().HaveCount(6);
    }

    [Fact]
    public void Union_PreservesConfig()
    {
        var dates = CreateDateRange();
        var second = Builder.From(new DateOnly(2026, 2, 1)).To(new DateOnly(2026, 2, 5));
        var union = dates.Union(second);

        union.Config.Should().BeSameAs(dates.Config);
        union.Should().HaveCount(36);
    }

    [Fact]
    public void Union_RemovesDuplicates()
    {
        var dates = Builder.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 1, 5));
        var second = new[] { new DateOnly(2026, 1, 3), new DateOnly(2026, 1, 4), new DateOnly(2026, 1, 5) };
        var union = dates.Union(second);

        union.Should().HaveCount(5);
    }

    [Fact]
    public void Intersect_PreservesConfig()
    {
        var dates = CreateDateRange();
        var second = new[]
        {
            new DateOnly(2026, 1, 1), 
            new DateOnly(2026, 1, 15), 
            new DateOnly(2026, 1, 31)
        }.AsMonotonicallyAscendingEnumerable();
        var intersect = dates.Intersect(second);

        intersect.Config.Should().BeSameAs(dates.Config);
        intersect.Should().HaveCount(3);
    }

    [Fact]
    public void Intersect_ReturnsEmptyWhenNoOverlap()
    {
        var dates = Builder.From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 1, 5));
        var second = new[] { new DateOnly(2026, 2, 1), new DateOnly(2026, 2, 5) };
        var intersect = dates.Intersect(second);

        intersect.Should().BeEmpty();
    }

    [Fact]
    public void Except_PreservesConfig()
    {
        var dates = CreateDateRange();
        var second = new[] { 
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 15),
            new DateOnly(2026, 1, 31) }
            .AsMonotonicallyAscendingEnumerable();
        var except = dates.Except(second);

        except.Config.Should().BeSameAs(dates.Config);
        except.Should().HaveCount(28);
    }

    [Fact]
    public void Except_WithComparer_PreservesConfig()
    {
        var dates = CreateDateRange();
        var second = new[] { new DateOnly(2026, 1, 1) }.AsMonotonicallyAscendingEnumerable();
        var except = dates.Except(second);

        except.Config.Should().BeSameAs(dates.Config);
    }

    [Fact]
    public void Except_ReturnsAllWhenSecondIsEmpty()
    {
        var dates = CreateDateRange();
        var except = dates.Except(Array.Empty<DateOnly>());

        except.Should().HaveCount(31);
    }

    [Fact]
    public void Except_ReturnsEmptyWhenSameAsSecond()
    {
        var dates = CreateDateRange();
        var except = dates.Except(dates);

        except.Should().BeEmpty();
    }

    [Fact]
    public void Concat_WithEmptyFirst_ReturnsSecond()
    {
        var dates = new List<DateOnly>().AsDateEnumerable(Builder.Config);
        var second = new[] { new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 2) };
        var concat = dates.Concat(second);

        concat.Should().HaveCount(2);
    }
}
