# Southeast Asian Buddhist Lunisolar Calendar Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Extend `TemporaLinq.Astronomy` with new-moon and December-solstice calculations, then a `SoutheastAsianBuddhistCalendar` computing Makha/Visakha/Asalha Bucha dates, verified against real reference dates, then use it for Thailand/Myanmar/Cambodia/Laos and to complete Vesak for Singapore/Malaysia/Indonesia.

**Architecture:** See `docs/superpowers/specs/2026-08-26-southeast-asian-buddhist-calendar-design.md` for the full algorithm derivation and verified formula sources. This plan's Task 3 (the month-counting logic) is this project's own synthesis, not a verified port — it is expected to need iteration against the reference dates in Task 3 before it's correct. Do not skip that verification step or treat a first-pass implementation as done.

**Tech Stack:** C#/.NET (net8.0 + net10.0), xUnit + FluentAssertions.

**Spec:** `docs/superpowers/specs/2026-08-26-southeast-asian-buddhist-calendar-design.md`

## Global Constraints

- `dotnet build` and `dotnet test --framework net10.0` must pass after every task.
- No new external dependencies — pure math, consistent with the rest of `TemporaLinq.Astronomy`.

## Reference dates for verification (Task 3)

Cross-checked against multiple independent sources during brainstorming/design, not memorized:

| Year | Makha Bucha | Visakha Bucha (Vesak) | Asalha Bucha |
|---|---|---|---|
| 2024 (ordinary year) | Feb 24 | May 22 | Jul 20 |
| 2025 (ordinary year) | Feb 12 | May 12 | Jul 10 |
| 2026 (confirmed leap-month/13-full-moon year) | Mar 3 | May 31 | Jul 29 |

If the implementation's output differs from these by more than 1 day for any entry, the
month-counting logic is wrong — fix the algorithm, do not adjust these reference dates (they were
independently cross-checked; the code under test is what's unverified here).

---

## Task 1: Add LunarPhaseCalculation.NewMoonsInGregorianYear

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Astronomy/LunarPhaseCalculation.cs`
- Modify: `TemporaLinq/TemporaLinq.Test/LunarPhaseCalculationTest.cs`

**Interfaces:**
- Produces: `LunarPhaseCalculation.NewMoonsInGregorianYear(int) -> IEnumerable<DateOnly>`, consumed by Task 3.

- [ ] **Step 1: Refactor the existing full-moon computation to share the angle/additional-correction math**

Extract the shared computation (T, E, M, M', F, Ω, and the 14 additional A[]/ac[] planetary
corrections) out of `FullMoonJde` into a private helper that takes the periodic-term coefficient
array and the lunation `k` and returns the JDE, so both full-moon and new-moon calculations reuse
it without duplicating the angle math. Keep `FullMoonsInGregorianYear`'s existing behavior and
tests passing throughout this refactor.

- [ ] **Step 2: Write the failing test for new moons**

```csharp
[Fact]
public void NewMoonsInGregorianYear_2024_MatchesPublishedReferenceDates()
{
    // New moons are ~14.77 days offset from the full moons already verified for 2024
    // (Jan 25, Feb 24, Mar 25, Apr 23, May 23, Jun 21, Jul 21, Aug 19, Sep 17, Oct 17,
    // Nov 15, Dec 15) - the new moon before each of those full moons is roughly two weeks
    // earlier. Verify count and ascending order; exact dates cross-checked independently:
    var expected = new[]
    {
        new DateOnly(2024, 1, 11), new DateOnly(2024, 2, 9), new DateOnly(2024, 3, 10),
        new DateOnly(2024, 4, 8), new DateOnly(2024, 5, 8), new DateOnly(2024, 6, 6),
        new DateOnly(2024, 7, 5), new DateOnly(2024, 8, 4), new DateOnly(2024, 9, 3),
        new DateOnly(2024, 10, 2), new DateOnly(2024, 11, 1), new DateOnly(2024, 12, 1),
        new DateOnly(2024, 12, 30),
    };

    var actual = LunarPhaseCalculation.NewMoonsInGregorianYear(2024).ToList();

    actual.Should().HaveCount(expected.Length);
    for (var i = 0; i < expected.Length; i++)
    {
        Math.Abs(actual[i].DayNumber - expected[i].DayNumber).Should().BeLessThanOrEqualTo(1,
            $"new moon #{i} in 2024 should be within 1 day of the published reference date");
    }
}

[Fact]
public void NewMoonsInGregorianYear_ReturnsDatesInAscendingOrderWithinTheYear()
{
    var actual = LunarPhaseCalculation.NewMoonsInGregorianYear(2025).ToList();

    actual.Should().BeInAscendingOrder();
    actual.Should().OnlyContain(d => d.Year == 2025);
}
```

Before trusting the `expected` array above, independently verify 2024's actual new moon dates via
WebSearch (e.g. a published astronomical almanac) rather than assuming the offset-from-full-moon
approximation used to write it is exact — it's a reasonable estimate for drafting the test but the
real verification must come from an independent source, the same standard used for full moons in
Phase 1.

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter NewMoonsInGregorianYear`
Expected: FAIL (method does not exist yet).

- [ ] **Step 3: Implement `NewMoonsInGregorianYear`**

Mirror `FullMoonsInGregorianYear`'s structure exactly, but with `q = 0` in the lunation-snap
calculation (`k = floor((year-2000)*12.3685 - 0 + 0.5) + 0`, vs. `+ 0.5` for full moon) and this
periodic-term table in place of `FullMoonPeriodicTerms`:

```csharp
private static readonly double[] NewMoonPeriodicTerms =
[
    -0.4072, 0.17241, 0.01608, 0.01039, 0.00739, -0.00514, 0.00208, -0.00111, -0.00057,
    0.00056, -0.00042, 0.00042, 0.00038, -0.00024, -0.00017, -0.00007, 0.00004, 0.00004,
    0.00003, 0.00003, -0.00003, 0.00003, -0.00002, -0.00002, 0.00002,
];
```

(`AdditionalCorrectionCoefficients` and the angle formulas are shared with full moon, per Step 1's
refactor.)

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LunarPhaseCalculationTest`
Expected: PASS, all cases (old full-moon tests plus new ones).

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Astronomy/LunarPhaseCalculation.cs TemporaLinq/TemporaLinq.Test/LunarPhaseCalculationTest.cs
git commit -m "feat: add LunarPhaseCalculation.NewMoonsInGregorianYear"
```

---

## Task 2: Add DecemberSolsticeCalculation

**Files:**
- Create: `TemporaLinq/TemporaLinq.Astronomy/DecemberSolsticeCalculation.cs`
- Test: `TemporaLinq/TemporaLinq.Test/DecemberSolsticeCalculationTest.cs`

**Interfaces:**
- Produces: `DecemberSolsticeCalculation.SolsticeDate(int gregorianYear) -> DateOnly`, consumed by Task 3.

- [ ] **Step 1: Write the failing test**

Verify actual December solstice dates for a couple of years via WebSearch (day-level precision is
all that's needed) before writing the assertions, then:

```csharp
using FluentAssertions;
using TemporaLinq.Astronomy;

namespace TemporaLinq.Test;

public class DecemberSolsticeCalculationTest
{
    [Fact]
    public void SolsticeDate_ReturnsKnownReferenceDates()
    {
        // December solstices are almost always Dec 21 or Dec 22; verify against independently
        // checked reference dates for a couple of years, not memorized.
        DecemberSolsticeCalculation.SolsticeDate(2024).Should().Be(/* verified date */);
        DecemberSolsticeCalculation.SolsticeDate(2026).Should().Be(/* verified date */);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter DecemberSolsticeCalculationTest`
Expected: FAIL (class does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
namespace TemporaLinq.Astronomy;

/// <summary>
/// Computes the date of the December solstice using Meeus' low-precision algorithm
/// ("Astronomical Algorithms," 2nd ed., chapter 27). Accurate to within a minute of time for
/// 1951-2050, far more precision than the day-level granularity needed here.
/// </summary>
public static class DecemberSolsticeCalculation
{
    private static readonly (double A, double B, double C)[] PeriodicTerms =
    [
        (485, 324.96, 1934.136), (203, 337.23, 32964.467), (199, 342.08, 20.186),
        (182, 27.85, 445267.112), (156, 73.14, 45036.886), (136, 171.52, 22518.443),
        (77, 222.54, 65928.934), (74, 296.72, 3034.906), (70, 243.58, 9037.513),
        (58, 119.81, 33718.147), (52, 297.17, 150.678), (50, 21.02, 2281.226),
        (45, 247.54, 29929.562), (44, 325.15, 31555.956), (29, 60.93, 4443.417),
        (18, 155.12, 67555.328), (17, 288.79, 4562.452), (16, 198.04, 62894.029),
        (14, 199.76, 31436.921), (12, 95.39, 14577.848), (12, 287.11, 31931.756),
        (12, 320.81, 34777.259), (9, 227.73, 1222.114), (8, 15.45, 16859.074),
    ];

    public static DateOnly SolsticeDate(int gregorianYear)
    {
        var y = (gregorianYear - 2000) * 0.001;
        var jde0 = 2451900.05952 + 365242.74049 * y - 0.06223 * y * y
            - 0.00823 * y * y * y + 0.00032 * y * y * y * y;

        var t = (jde0 - 2451545.0) / 36525.0;
        var w = (35999.373 * t - 2.47) * Math.PI / 180.0;
        var deltaLambda = 1 + 0.0334 * Math.Cos(w) + 0.0007 * Math.Cos(2 * w);

        var s = 0.0;
        foreach (var (a, b, c) in PeriodicTerms)
            s += a * Math.Cos((b + c * t) * Math.PI / 180.0);

        var jde = jde0 + 0.00001 * s / deltaLambda;
        return DateFromJde(jde); // reuse/extract the existing JDE-to-DateOnly conversion
    }
}
```

Extract the existing `DateFromJde` from `LunarPhaseCalculation` into a shared internal location
(e.g. an internal static helper class in `TemporaLinq.Astronomy`) rather than duplicating it, since
both classes need the identical JD-to-Gregorian conversion.

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter DecemberSolsticeCalculationTest`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Astronomy/DecemberSolsticeCalculation.cs TemporaLinq/TemporaLinq.Test/DecemberSolsticeCalculationTest.cs
git commit -m "feat: add DecemberSolsticeCalculation"
```

---

## Task 3: Implement SoutheastAsianBuddhistCalendar (iterate until verified)

**Files:**
- Create: `TemporaLinq/TemporaLinq.Astronomy/SoutheastAsianBuddhistCalendar.cs`
- Test: `TemporaLinq/TemporaLinq.Test/SoutheastAsianBuddhistCalendarTest.cs`

**Interfaces:**
- Consumes: `LunarPhaseCalculation.NewMoonsInGregorianYear`/`FullMoonsInGregorianYear` (Task 1, prior),
  `DecemberSolsticeCalculation.SolsticeDate` (Task 2).
- Produces: `SoutheastAsianBuddhistCalendar.MakhaBuchaDate/VisakhaBuchaDate/AsalhaBuchaDate(int) -> DateOnly`,
  consumed by Task 4 onward (the country implementations).

This task is different from the others in this plan: the design doc is explicit that the
month-counting algorithm below is this project's own synthesis, not a verified port, and is
expected to need refinement. **Do not treat a first-pass implementation as done just because it
compiles.** Iterate against the reference table at the top of this plan (2024, 2025, 2026 — one
leap-month year) until all three holidays match within 1 day for all three years, per the
"iterate until verified" decision already made for this phase.

- [ ] **Step 1: Write the failing test using the reference table**

```csharp
using FluentAssertions;
using TemporaLinq.Astronomy;

namespace TemporaLinq.Test;

public class SoutheastAsianBuddhistCalendarTest
{
    [Theory]
    [InlineData(2024, "2024-02-24", "2024-05-22", "2024-07-20")]
    [InlineData(2025, "2025-02-12", "2025-05-12", "2025-07-10")]
    [InlineData(2026, "2026-03-03", "2026-05-31", "2026-07-29")] // confirmed leap-month year
    public void HolyDays_MatchPublishedReferenceDates(
        int year, string makhaBucha, string visakhaBucha, string asalhaBucha)
    {
        var expectedMakha = DateOnly.Parse(makhaBucha);
        var expectedVisakha = DateOnly.Parse(visakhaBucha);
        var expectedAsalha = DateOnly.Parse(asalhaBucha);

        Math.Abs(SoutheastAsianBuddhistCalendar.MakhaBuchaDate(year).DayNumber - expectedMakha.DayNumber)
            .Should().BeLessThanOrEqualTo(1, $"Makha Bucha {year}");
        Math.Abs(SoutheastAsianBuddhistCalendar.VisakhaBuchaDate(year).DayNumber - expectedVisakha.DayNumber)
            .Should().BeLessThanOrEqualTo(1, $"Visakha Bucha {year}");
        Math.Abs(SoutheastAsianBuddhistCalendar.AsalhaBuchaDate(year).DayNumber - expectedAsalha.DayNumber)
            .Should().BeLessThanOrEqualTo(1, $"Asalha Bucha {year}");
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SoutheastAsianBuddhistCalendarTest`
Expected: FAIL (class does not exist yet).

- [ ] **Step 3: Implement the leap-month test and month-counting logic**

Implementation approach (per the design doc):

1. For a given Gregorian year, find the December solstice of the *previous* Gregorian year (the
   lunar year containing this year's Makha/Visakha/Asalha Bucha begins around/after that solstice).
2. Find the new moon nearest that solstice (check the new moon immediately before and immediately
   after; pick whichever is closer in absolute day difference).
3. **Leap-month test**: if that nearest new moon falls strictly before the solstice and within 11
   days of it, this lunar year is a leap-month (13-month) year.
4. **Month 1 start**: the first new moon on or after that solstice.
5. Walk forward new-moon to new-moon, numbering months 1, 2, 3, ... — in a leap-month year, after
   month 8 insert an extra month before continuing to month 9 (so the month sequence is
   1,2,3,4,5,6,7,8,8,9,10,11,12 — 13 months total).
6. For each named holiday, find the full moon (via `FullMoonsInGregorianYear`, or a full-moon
   lookup within the specific month's new-moon-to-new-moon span) that falls within the target
   month's span: month 3 for Makha Bucha, month 6 for Visakha Bucha, and the *last* (second, in a
   leap year) occurrence of month 8 for Asalha Bucha.
7. If a computed date lands in a different Gregorian year than requested (e.g. month 1 starting in
   December), that's expected — only the specific target month's date matters, not which Gregorian
   year the month *started* in.

Write this as a private static method building the month-boundary list for the lunar year
containing a target date, then three public methods that build the right lunar year for the
requested Gregorian year and pick out month 3/6/8's holiday date.

- [ ] **Step 4: Run test — iterate until it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SoutheastAsianBuddhistCalendarTest`

If any of the 3 years x 3 holidays don't match within 1 day: do not adjust the reference dates.
Instead, debug the month-counting logic — likely culprits are an off-by-one in which new moon
starts month 1, an inverted leap-month test condition, or picking the wrong (first vs. last)
occurrence of month 8. Re-derive from the design doc's algorithm description, add temporary debug
output of the computed month boundaries for 2026 if needed to see where the counting diverges from
expectation, and fix. Only stop and document a caveat instead of continuing to iterate if genuinely
stuck after real effort — do not settle for a first attempt that doesn't match.

Expected once correct: PASS, all 3 theory cases.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Astronomy/SoutheastAsianBuddhistCalendar.cs TemporaLinq/TemporaLinq.Test/SoutheastAsianBuddhistCalendarTest.cs
git commit -m "feat: add SoutheastAsianBuddhistCalendar"
```

---

## Task 4: Update the design doc status and run the full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-26-southeast-asian-buddhist-calendar-design.md`

- [ ] **Step 1: Note completion status**

Add a status line noting `LunarPhaseCalculation.NewMoonsInGregorianYear`,
`DecemberSolsticeCalculation`, and `SoutheastAsianBuddhistCalendar` are implemented and verified
against the 2024/2025/2026 reference table, including the leap-month year. If any structural
detail had to be resolved differently than the design doc originally described (e.g. a different
month-1 anchor point), update the doc to reflect what was actually implemented.

- [ ] **Step 2: Run the full test suite**

Run: `cd TemporaLinq && dotnet test --framework net10.0`
Expected: All tests pass, 0 failures.

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-26-southeast-asian-buddhist-calendar-design.md
git commit -m "docs: mark Southeast Asian Buddhist calendar mechanism implemented and verified"
```

---

## Tasks 5-11: Country implementations (dispatched separately to parallel agents)

Not detailed in this plan — see the coordinating session's dispatch prompts. Each covers one or
more of: Thailand, Myanmar, Cambodia, Laos (new `NationalHolidays.cs` files using
`SoutheastAsianBuddhistCalendar`), and adding Vesak (`VisakhaBuchaDate`) to Singapore's, Malaysia's,
and Indonesia's existing `NationalHolidays.cs` files. All depend on Tasks 1-4 being merged first.
