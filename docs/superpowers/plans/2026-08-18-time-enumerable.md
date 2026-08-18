# ITimeEnumerable & Date×Time Combinator Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [x]`) syntax for tracking.

**Goal:** Add an `ITimeEnumerable`/`Times` type for generating ascending `TimeOnly` sequences, and a `dates.On(times)` combinator that cross-joins a date sequence with a time sequence into an ordered `DateTime` sequence — after first reorganizing the core library's namespaces so the new time code has a proper home.

**Architecture:** Split the existing flat `TemporaLinq` namespace into a domain-agnostic root (`TemporaLinq`: `IMonotonicallyAscendingEnumerable<T>`, its wrapper, generic `Merge<T>`) and a date-specific `TemporaLinq.Dates` namespace (everything that operates on `DateOnly`/`IDateEnumerable` today). Add a peer `TemporaLinq.Times` namespace for the new `Times`/`ITimeEnumerable` types, and a root-level `On` combinator that bridges the two into a plain `IMonotonicallyAscendingEnumerable<DateTime>` — no new marker interface.

**Tech Stack:** C# / .NET 8, xUnit + FluentAssertions (existing test project conventions).

**Spec:** `docs/superpowers/specs/2026-08-18-time-enumerable-design.md`

## Global Constraints

- `Times.To(time)` is exclusive (matches `Dates.To`'s convention).
- `Times.Every(TimeSpan)` is the only generation primitive — no `EveryHour()`/`EveryMinute()` shorthands.
- No midnight-wrapping time ranges — out of scope entirely.
- No new `IDateTimeEnumerable` interface — `On` returns `IMonotonicallyAscendingEnumerable<DateTime>`.
- No validation against large/unbounded/infinite sequences — laziness is intentional, not a risk.
- `Times.Every` validates eagerly and throws `ArgumentOutOfRangeException` for non-positive steps (the one deliberate guard in this feature, because the failure mode is a silent infinite loop rather than an empty result).
- Every existing test must stay green (`dotnet test`) after the namespace reorganization — no behavioral changes are permitted in Task 1.

---

## Task 1: Namespace Reorganization

Splits today's flat `TemporaLinq` namespace into `TemporaLinq` (generic core) and `TemporaLinq.Dates` (date-specific), giving `TemporaLinq.Times` (Task 2) a consistent peer namespace to land in. Purely mechanical — no behavior changes, verified by the full existing test suite staying green.

**Files:**
- Create: `TemporaLinq/Dates/` (directory)
- Move: `TemporaLinq/Dates.cs` → `TemporaLinq/Dates/Dates.cs`
- Move: `TemporaLinq/IDateEnumerable.cs` → `TemporaLinq/Dates/IDateEnumerable.cs`
- Move: `TemporaLinq/CalendarConfig.cs` → `TemporaLinq/Dates/CalendarConfig.cs`
- Move: `TemporaLinq/Weekends.cs` → `TemporaLinq/Dates/Weekends.cs`
- Move: `TemporaLinq/DateEnumerableWrapper.cs` → `TemporaLinq/Dates/DateEnumerableWrapper.cs`
- Modify: `TemporaLinq/DateLinq.cs` → moved and merged into `TemporaLinq/Dates/DateLinq.cs`
- Modify: `TemporaLinq/LinqExtensions.cs` (strip to the one generic method)
- Modify: `TemporaLinq/Operations.cs` (strip to the two generic `Merge` overloads)
- Create: `TemporaLinq/Dates/DateOperations.cs` (date-specific `Merge`/`Except`/`Union`/`Intersect`/`Distinct`, split out of `Operations.cs`)
- Modify: `TemporaLinq.Test/DatesTest.cs`, `BusinessDaysTest.cs`, `LinqExtensionsTest.cs`, `OperationsTest.cs`, `PredicatesTest.cs` (add `using TemporaLinq.Dates;`)

**Interfaces:**
- Produces: `TemporaLinq.Dates.IDateEnumerable`, `TemporaLinq.Dates.Dates`, `TemporaLinq.Dates.CalendarConfig`, `TemporaLinq.Dates.Weekends`, `TemporaLinq.Dates.DateEnumerableWrapper` (internal), `TemporaLinq.Dates.DateLinq` (static, all the `IDateEnumerable` LINQ operators), `TemporaLinq.Dates.DateOperations` (static, `Merge`/`Except`/`Union`/`Intersect`/`Distinct` for `IDateEnumerable`) — these are what Task 3's `DateTimeCombinators.cs` consumes.
- Produces (unchanged location, narrowed content): `TemporaLinq.LinqExtensions.AsMonotonicallyAscendingEnumerable<T>(this IEnumerable<T>)`, `TemporaLinq.Operations.Merge<T>(IEnumerable<IEnumerable<T>>)`, `TemporaLinq.Operations.Merge<T>(this IMonotonicallyAscendingEnumerable<T>, params IMonotonicallyAscendingEnumerable<T>[])`.

- [x] **Step 1: Move the five single-type files into `TemporaLinq/Dates/` and update their namespace**

```bash
mkdir -p TemporaLinq/Dates
for f in Dates.cs IDateEnumerable.cs CalendarConfig.cs Weekends.cs DateEnumerableWrapper.cs; do
  mv "TemporaLinq/$f" "TemporaLinq/Dates/$f"
  sed -i 's/^namespace TemporaLinq;$/namespace TemporaLinq.Dates;/' "TemporaLinq/Dates/$f"
done
```

- [x] **Step 2: Verify the five moved files now declare the right namespace**

```bash
grep -H "^namespace" TemporaLinq/Dates/Dates.cs TemporaLinq/Dates/IDateEnumerable.cs \
  TemporaLinq/Dates/CalendarConfig.cs TemporaLinq/Dates/Weekends.cs TemporaLinq/Dates/DateEnumerableWrapper.cs
```

Expected: every line reads `namespace TemporaLinq.Dates;`.

- [x] **Step 3: Replace `TemporaLinq/LinqExtensions.cs` with the generic-only version**

```csharp
namespace TemporaLinq;

public static class LinqExtensions
{
    public static IMonotonicallyAscendingEnumerable<T> AsMonotonicallyAscendingEnumerable<T>(
        this IEnumerable<T> enumerable) where T : IComparable<T>
        => new MonotonicAscendingEnumerableWrapper<T>(enumerable);
}
```

- [x] **Step 4: Delete `TemporaLinq/DateLinq.cs` and create `TemporaLinq/Dates/DateLinq.cs`**, merging in the `IDateEnumerable`-specific methods that used to live in `LinqExtensions.cs`

```bash
rm TemporaLinq/DateLinq.cs
```

```csharp
namespace TemporaLinq.Dates;

public static class DateLinq
{
    public static IDateEnumerable Where(this IDateEnumerable seq, Func<DateOnly, bool> predicate)
        => Enumerable.Where(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Where(this IDateEnumerable seq, Func<DateOnly, int, bool> predicate)
        => Enumerable.Where(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Take(this IDateEnumerable seq, int count)
        => Enumerable.Take(seq, count).AsDateEnumerable(seq.Config);

    public static IDateEnumerable TakeWhile(this IDateEnumerable seq, Func<DateOnly, bool> predicate)
        => Enumerable.TakeWhile(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable TakeWhile(this IDateEnumerable seq, Func<DateOnly, int, bool> predicate)
        => Enumerable.TakeWhile(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable TakeLast(this IDateEnumerable seq, int count)
        => Enumerable.TakeLast(seq, count).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Skip(this IDateEnumerable seq, int count)
        => Enumerable.Skip(seq, count).AsDateEnumerable(seq.Config);

    public static IDateEnumerable SkipWhile(this IDateEnumerable seq, Func<DateOnly, bool> predicate)
        => Enumerable.SkipWhile(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable SkipWhile(this IDateEnumerable seq, Func<DateOnly, int, bool> predicate)
        => Enumerable.SkipWhile(seq, predicate).AsDateEnumerable(seq.Config);

    public static IDateEnumerable SkipLast(this IDateEnumerable seq, int count)
        => Enumerable.SkipLast(seq, count).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Select(this IDateEnumerable seq, Func<DateOnly, DateOnly> selector)
        => Enumerable.Select(seq, selector).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Select(this IDateEnumerable seq, Func<DateOnly, int, DateOnly> selector)
        => Enumerable.Select(seq, selector).AsDateEnumerable(seq.Config);

    public static IDateEnumerable SelectMany(this IDateEnumerable seq, Func<DateOnly, IEnumerable<DateOnly>> selector)
        => Enumerable.SelectMany(seq, selector).AsDateEnumerable(seq.Config);

    public static IDateEnumerable SelectMany(this IDateEnumerable seq,
        Func<DateOnly, int, IEnumerable<DateOnly>> selector)
        => Enumerable.SelectMany(seq, selector).AsDateEnumerable(seq.Config);

    public static DateOnly First(this IMonotonicallyAscendingEnumerable<DateOnly> seq, DayOfWeek dayOfWeek)
    {
        var ret = seq.FirstOrDefault(dayOfWeek);
        return ret != default
            ? ret
            : throw new InvalidOperationException($"No {dayOfWeek} found in date sequence");
    }

    public static DateOnly FirstOrDefault(this IMonotonicallyAscendingEnumerable<DateOnly> seq, DayOfWeek dayOfWeek)
        => seq.FirstOrDefault(d => d.DayOfWeek == dayOfWeek);

    public static IDateEnumerable Only(this IDateEnumerable seq, params DayOfWeek[] weekdays)
        => seq.Where(date => weekdays.Contains(date.DayOfWeek)).AsDateEnumerable(seq.Config);

    public static IDateEnumerable Except(this IDateEnumerable seq, params DayOfWeek[] weekdays)
        => seq.Where(date => !weekdays.Contains(date.DayOfWeek)).AsDateEnumerable(seq.Config);

    public static IDateEnumerable EveryNth(this IDateEnumerable seq, int n)
        => seq.Where((_, idx) => idx % n == 0).AsDateEnumerable(seq.Config);

    public static IDateEnumerable BusinessDays(this IDateEnumerable seq)
        => seq.Where(d => !seq.Config.WeekendDays.Contains(d.DayOfWeek)).AsDateEnumerable(seq.Config);

    public static IDateEnumerable WithContext(this IEnumerable<DateOnly> seq, IDateEnumerable context)
        => seq.AsDateEnumerable(context.Config);

    public static IDateEnumerable AsDateEnumerable(this IEnumerable<DateOnly> seq, CalendarConfig config)
        => new DateEnumerableWrapper(seq, config);
}
```

- [x] **Step 5: Replace `TemporaLinq/Operations.cs` with the generic-only version**

```csharp
namespace TemporaLinq;

public static class Operations
{
    /// <summary>
    /// Returns a stream of values that contains all values from the given streams in ascending order.
    /// </summary>
    public static IEnumerable<T> Merge<T>(IEnumerable<IEnumerable<T>> dateStreams)
    {
        var heap = new PriorityQueue<IEnumerator<T>, T>();

        var nonEmptyEnumerators = dateStreams
            .Select(str => str.GetEnumerator())
            .Where(e => e.MoveNext());

        foreach (var enumerator in nonEmptyEnumerators)
            heap.Enqueue(enumerator, enumerator.Current);

        while (heap.Count > 0)
        {
            var smallestEnum = heap.Dequeue();

            yield return smallestEnum.Current;

            if (smallestEnum.MoveNext())
                heap.Enqueue(smallestEnum, smallestEnum.Current);
            else
                smallestEnum.Dispose();
        }
    }

    public static IMonotonicallyAscendingEnumerable<T> Merge<T>(this IMonotonicallyAscendingEnumerable<T> dates,
        params IMonotonicallyAscendingEnumerable<T>[] others) where T : IComparable<T>
        => Merge(new[] { dates }.Concat(others)).AsMonotonicallyAscendingEnumerable();
}
```

- [x] **Step 6: Create `TemporaLinq/Dates/DateOperations.cs`** with the date-specific operations split out of the old `Operations.cs` (note the `Merge` overload below now qualifies its call to the generic `Merge` with `Operations.`, since it's moved into a different class than that generic method)

```csharp
namespace TemporaLinq.Dates;

public static class DateOperations
{
    public static IDateEnumerable Merge(this IDateEnumerable dates,
        params IMonotonicallyAscendingEnumerable<DateOnly>[] others)
        => Operations.Merge(new[] { dates }.Concat(others)).AsDateEnumerable(dates.Config);

    public static IDateEnumerable Except(this IDateEnumerable seq, IMonotonicallyAscendingEnumerable<DateOnly> second)
    {
        return new DateEnumerableWrapper(ExceptImpl(), seq.Config);

        IEnumerable<DateOnly> ExceptImpl()
        {
            using var seqEnum = seq.GetEnumerator();
            using var secondEnum = second.GetEnumerator();

            var seqHasNext = seqEnum.MoveNext();
            var secondHasNext = secondEnum.MoveNext();

            while (seqHasNext)
            {
                if (!secondHasNext)
                {
                    do
                    {
                        yield return seqEnum.Current;
                    } while (seqEnum.MoveNext());

                    yield break;
                }

                if (seqEnum.Current < secondEnum.Current)
                {
                    yield return seqEnum.Current;
                    seqHasNext = seqEnum.MoveNext();
                }
                else if (seqEnum.Current > secondEnum.Current)
                {
                    secondHasNext = secondEnum.MoveNext();
                }
                else
                {
                    seqHasNext = seqEnum.MoveNext();
                    secondHasNext = secondEnum.MoveNext();
                }
            }
        }
    }

    /// <summary>
    /// Returns a stream of values that contains all unique values from both sorted streams in ascending order.
    /// Uses an efficient O(n+m) merge algorithm.
    /// </summary>
    public static IDateEnumerable Union(this IDateEnumerable first, IMonotonicallyAscendingEnumerable<DateOnly> second)
    {
        return new DateEnumerableWrapper(UnionImpl(first, second), first.Config);

        IEnumerable<DateOnly> UnionImpl(IDateEnumerable a, IMonotonicallyAscendingEnumerable<DateOnly> b)
        {
            using var firstEnum = a.GetEnumerator();
            using var secondEnum = b.GetEnumerator();

            var firstHasNext = firstEnum.MoveNext();
            var secondHasNext = secondEnum.MoveNext();

            while (firstHasNext || secondHasNext)
            {
                if (!secondHasNext)
                {
                    do
                    {
                        yield return firstEnum.Current;
                    } while (firstEnum.MoveNext());

                    yield break;
                }

                if (!firstHasNext)
                {
                    do
                    {
                        yield return secondEnum.Current;
                    } while (secondEnum.MoveNext());

                    yield break;
                }

                if (firstEnum.Current < secondEnum.Current)
                {
                    yield return firstEnum.Current;
                    firstHasNext = firstEnum.MoveNext();
                }
                else if (firstEnum.Current > secondEnum.Current)
                {
                    yield return secondEnum.Current;
                    secondHasNext = secondEnum.MoveNext();
                }
                else
                {
                    yield return firstEnum.Current;
                    firstHasNext = firstEnum.MoveNext();
                    secondHasNext = secondEnum.MoveNext();
                }
            }
        }
    }

    /// <summary>
    /// Returns a stream of values that contains only values present in both sorted streams in ascending order.
    /// Uses an efficient O(n+m) merge algorithm.
    /// </summary>
    public static IDateEnumerable Intersect(this IDateEnumerable first,
        IMonotonicallyAscendingEnumerable<DateOnly> second)
    {
        return new DateEnumerableWrapper(IntersectImpl(first, second), first.Config);

        IEnumerable<DateOnly> IntersectImpl(IDateEnumerable a, IMonotonicallyAscendingEnumerable<DateOnly> b)
        {
            using var firstEnum = a.GetEnumerator();
            using var secondEnum = b.GetEnumerator();

            var firstHasNext = firstEnum.MoveNext();
            var secondHasNext = secondEnum.MoveNext();

            while (firstHasNext && secondHasNext)
            {
                if (firstEnum.Current < secondEnum.Current)
                {
                    firstHasNext = firstEnum.MoveNext();
                }
                else if (firstEnum.Current > secondEnum.Current)
                {
                    secondHasNext = secondEnum.MoveNext();
                }
                else
                {
                    yield return firstEnum.Current;
                    firstHasNext = firstEnum.MoveNext();
                    secondHasNext = secondEnum.MoveNext();
                }
            }
        }
    }

    public static IDateEnumerable Distinct(this IDateEnumerable seq)
    {
        return new DateEnumerableWrapper(DistinctImpl(seq), seq.Config);

        IEnumerable<DateOnly> DistinctImpl(IDateEnumerable dates)
        {
            using var dateEnumerator = dates.GetEnumerator();
            DateOnly? last = null;
            while (dateEnumerator.MoveNext())
            {
                var current = dateEnumerator.Current;
                if (current != last)
                {
                    yield return current;
                    last = current;
                }
            }
        }
    }
}
```

- [x] **Step 7: Add `using TemporaLinq.Dates;` to the five affected test files**

```bash
for f in DatesTest.cs BusinessDaysTest.cs LinqExtensionsTest.cs OperationsTest.cs PredicatesTest.cs; do
  sed -i '1i using TemporaLinq.Dates;' "TemporaLinq.Test/$f"
done
```

- [x] **Step 8: Build and fix any remaining reference errors**

```bash
dotnet build
```

Expected: builds clean. If the compiler reports `CS0246`/`CS1061` in a file not listed in Step 7 (e.g. a Holidays file that turns out to reference a moved type), add `using TemporaLinq.Dates;` to that specific file and rebuild — the grep in the plan's research (no `Dates`/`IDateEnumerable`/`CalendarConfig`/`Weekends` references found under `TemporaLinq.Holidays/`) says this shouldn't be necessary, but the build is the ground truth.

- [x] **Step 9: Run the full test suite and confirm no behavior changed**

```bash
dotnet test
```

Expected: same pass count as before this task (all existing tests green — this task must not change behavior, only namespaces).

- [x] **Step 10: Commit**

```bash
git add -A
git commit -m "refactor: split TemporaLinq.Dates namespace out of the core library"
```

(If this directory has not been initialized as a git repository yet, run `git init` first and confirm with the user before this first commit — the repo was not under version control as of this plan's writing.)

---

## Task 2: `Times` / `ITimeEnumerable`

Adds the intraday time-sequence type, built test-first. No dependency on Task 1's new `TemporaLinq.Dates` types — this is a standalone `TemporaLinq.Times` namespace peer.

**Files:**
- Create: `TemporaLinq/Times/ITimeEnumerable.cs`
- Create: `TemporaLinq/Times/Times.cs`
- Test: `TemporaLinq.Test/TimesTest.cs`

**Interfaces:**
- Consumes: `TemporaLinq.IMonotonicallyAscendingEnumerable<T>` (root namespace, visible without a `using` since `TemporaLinq.Times` nests under `TemporaLinq`).
- Produces: `TemporaLinq.Times.ITimeEnumerable : IMonotonicallyAscendingEnumerable<TimeOnly>`; `TemporaLinq.Times.Times : ITimeEnumerable` with `Times.From(TimeOnly) -> Times`, `.To(TimeOnly) -> Times`, `.Every(TimeSpan) -> Times`. Task 3 consumes `Times`/`ITimeEnumerable` directly.

- [x] **Step 1: Write the failing tests**

Create `TemporaLinq.Test/TimesTest.cs`:

```csharp
using FluentAssertions;
using TemporaLinq.Times;

namespace TemporaLinq.Test;

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
```

- [x] **Step 2: Run the tests to verify they fail to compile**

```bash
dotnet test --filter "FullyQualifiedName~TimesTest"
```

Expected: build FAILS — `Times`/`TemporaLinq.Times` does not exist yet.

- [x] **Step 3: Implement `ITimeEnumerable` and `Times`**

Create `TemporaLinq/Times/ITimeEnumerable.cs`:

```csharp
namespace TemporaLinq.Times;

public interface ITimeEnumerable : IMonotonicallyAscendingEnumerable<TimeOnly>
{
}
```

Create `TemporaLinq/Times/Times.cs`:

```csharp
using System.Collections;

namespace TemporaLinq.Times;

public record Times : ITimeEnumerable
{
    public TimeOnly StartTime { get; private init; }
    public TimeOnly EndTime { get; private init; }
    public TimeSpan Step { get; private init; }

    public static Times From(TimeOnly start) => new() { StartTime = start };

    /// <summary>
    /// The end time is exclusive
    /// </summary>
    /// <param name="end"></param>
    /// <returns></returns>
    public Times To(TimeOnly end) => this with { EndTime = end };

    public Times Every(TimeSpan step)
        => step > TimeSpan.Zero
            ? this with { Step = step }
            : throw new ArgumentOutOfRangeException(nameof(step), "Step must be positive.");

    public IEnumerator<TimeOnly> GetEnumerator()
    {
        for (var time = StartTime; time < EndTime; time = time.Add(Step))
        {
            yield return time;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

- [x] **Step 4: Run the tests to verify they pass**

```bash
dotnet test --filter "FullyQualifiedName~TimesTest"
```

Expected: PASS (6/6).

- [x] **Step 5: Commit**

```bash
git add TemporaLinq/Times TemporaLinq.Test/TimesTest.cs
git commit -m "feat: add Times/ITimeEnumerable for intraday time sequences"
```

---

## Task 3: `On` Combinator

Adds the date×time cross-join. Depends on both `TemporaLinq.Dates` (Task 1) and `TemporaLinq.Times` (Task 2).

**Files:**
- Create: `TemporaLinq/DateTimeCombinators.cs`
- Test: `TemporaLinq.Test/DateTimeCombinatorsTest.cs`

**Interfaces:**
- Consumes: `TemporaLinq.Dates.IDateEnumerable` (Task 1), `TemporaLinq.Times.ITimeEnumerable` (Task 2), `TemporaLinq.LinqExtensions.AsMonotonicallyAscendingEnumerable<T>` (root, Task 1), `TemporaLinq.Operations.Merge<T>(this IMonotonicallyAscendingEnumerable<T>, ...)` (root, Task 1) — used by the test's composition scenario, not by the combinator itself.
- Produces: `TemporaLinq.DateTimeCombinators.On(this IDateEnumerable dates, ITimeEnumerable times) -> IMonotonicallyAscendingEnumerable<DateTime>`.

- [x] **Step 1: Write the failing tests**

Create `TemporaLinq.Test/DateTimeCombinatorsTest.cs`:

```csharp
using FluentAssertions;
using TemporaLinq.Dates;
using TemporaLinq.Times;

namespace TemporaLinq.Test;

public class DateTimeCombinatorsTest
{
    [Fact]
    public void On_ProducesCrossJoinOfDatesAndTimes()
    {
        var dates = Dates.Invariant().From(new DateOnly(2026, 1, 5)).To(new DateOnly(2026, 1, 7)); // Mon, Tue
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
        var dates = Dates.Invariant().From(new DateOnly(2026, 1, 5)).To(new DateOnly(2026, 1, 10));
        var times = Times.From(new TimeOnly(8, 0)).To(new TimeOnly(16, 0)).Every(TimeSpan.FromHours(1));

        var result = dates.On(times).ToList();

        result.Should().BeInAscendingOrder();
        result.Should().HaveCount(5 * 8);
    }

    [Fact]
    public void MergingTwoOnResults_CombinesDifferentSchedulesPerWeekdayGroup()
    {
        var start = new DateOnly(2026, 1, 5); // Monday
        var end = new DateOnly(2026, 1, 10);  // exclusive, through Friday

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
```

- [x] **Step 2: Run the tests to verify they fail to compile**

```bash
dotnet test --filter "FullyQualifiedName~DateTimeCombinatorsTest"
```

Expected: build FAILS — `On` does not exist yet.

- [x] **Step 3: Implement the combinator**

Create `TemporaLinq/DateTimeCombinators.cs`:

```csharp
using TemporaLinq.Dates;
using TemporaLinq.Times;

namespace TemporaLinq;

public static class DateTimeCombinators
{
    public static IMonotonicallyAscendingEnumerable<DateTime> On(this IDateEnumerable dates, ITimeEnumerable times)
        => OnImpl(dates, times).AsMonotonicallyAscendingEnumerable();

    private static IEnumerable<DateTime> OnImpl(IDateEnumerable dates, ITimeEnumerable times)
    {
        foreach (var date in dates)
        foreach (var time in times)
            yield return date.ToDateTime(time);
    }
}
```

- [x] **Step 4: Run the tests to verify they pass**

```bash
dotnet test --filter "FullyQualifiedName~DateTimeCombinatorsTest"
```

Expected: PASS (3/3).

- [x] **Step 5: Run the full solution test suite**

```bash
dotnet test
```

Expected: all tests pass (existing suite + the 6 `TimesTest` + 3 `DateTimeCombinatorsTest` added by this plan).

- [x] **Step 6: Commit**

```bash
git add TemporaLinq/DateTimeCombinators.cs TemporaLinq.Test/DateTimeCombinatorsTest.cs
git commit -m "feat: add dates.On(times) cross-join combinator"
```
