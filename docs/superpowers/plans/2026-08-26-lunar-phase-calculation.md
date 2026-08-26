# Lunar Phase Calculation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a `TemporaLinq.Astronomy` project computing full-moon dates via Meeus' truncated astronomical algorithm, then use it to implement Sri Lanka's national holidays (roadmap Phase 1 of closing the remaining 🔴 countries).

**Architecture:** New project `TemporaLinq.Astronomy` (net8.0;net10.0, no external dependencies), containing `LunarPhaseCalculation.FullMoonsInGregorianYear(int) -> IEnumerable<DateOnly>`. `TemporaLinq.Holidays` adds a project reference to it. Sri Lanka's `NationalHolidays.cs` follows the existing per-country pattern, using the new calculation for Poya days plus fixed-date holidays.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions.

**Spec:** `docs/superpowers/specs/2026-08-26-lunar-phase-calculation-design.md`

## Global Constraints

- `TemporaLinq.Astronomy` has zero dependencies beyond the .NET BCL, matching this codebase's convention (`EasterSundayCalculation`, `EthiopianCalendarCalculation`, etc. are all hand-rolled pure math).
- The Meeus algorithm computes lunar-phase instants in Terrestrial Dynamical Time; this implementation does not apply a ΔT correction before converting to a calendar date, because ΔT is at most a few minutes across this codebase's practical year range (documented on the class) — far smaller than the day-level granularity being computed. This is a deliberate, documented simplification, not an oversight.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task.

---

## Reference: the verified algorithm

Ported from a well-established open-source Go implementation that cites Meeus' *Astronomical
Algorithms* page numbers directly (`soniakeys/meeus`, MIT licensed) — cross-checked, not
transcribed from memory alone.

**Step 1 — lunation number k for the full moon nearest a given decimal year `y`:**
```
k = floor((y - 2000) * 12.3685 - 0.5 + 0.5) + 0.5   // q = 0.5 for full moon
```

**Step 2 — T (Julian millennia since J2000) and mean JDE:**
```
T = k / 1236.85
JDE_mean = 2451550.09766 + 29.530588861*k + 0.00015437*T^2 - 0.00000015*T^3 + 0.00000000073*T^4
```

**Step 3 — correction angles (degrees, converted to radians for `Math.Sin`/`Math.Cos`):**
```
E  = 1 - 0.002516*T - 0.0000074*T^2
M  = 2.5534 + 29.1053567*k  - 0.0000014*T^2 - 0.00000011*T^3           // Sun's mean anomaly
M' = 201.5643 + 385.81693528*k + 0.0107582*T^2 + 0.00001238*T^3 - 0.000000058*T^4   // Moon's mean anomaly
F  = 160.7108 + 390.67050284*k - 0.0016118*T^2 - 0.00000227*T^3 + 0.000000011*T^4   // Moon's argument of latitude
Ω  = 124.7746 - 1.56375588*k + 0.0020672*T^2 + 0.00000215*T^3                       // longitude of ascending node
```

**Step 4 — periodic correction for full moon** (all arguments in radians; `fc[0..24]` below):
```
correction =
    fc[0]*sin(M') + fc[1]*sin(M)*E + fc[2]*sin(2*M') + fc[3]*sin(2*F) +
    fc[4]*sin(M'-M)*E + fc[5]*sin(M'+M)*E + fc[6]*sin(2*M)*E*E +
    fc[7]*sin(M'-2*F) + fc[8]*sin(M'+2*F) + fc[9]*sin(2*M'+M)*E +
    fc[10]*sin(3*M') + fc[11]*sin(M+2*F)*E + fc[12]*sin(M-2*F)*E +
    fc[13]*sin(2*M'-M)*E + fc[14]*sin(Ω) + fc[15]*sin(M'+2*M) +
    fc[16]*sin(2*(M'-F)) + fc[17]*sin(3*M) + fc[18]*sin(M'+M-2*F) +
    fc[19]*sin(2*(M'+F)) + fc[20]*sin(M'+M+2*F) + fc[21]*sin(M'-M+2*F) +
    fc[22]*sin(M'-M-2*F) + fc[23]*sin(3*M'+M) + fc[24]*sin(4*M')

fc = [-0.40614, 0.17302, 0.01614, 0.01043, 0.00734, -0.00515, 0.00209, -0.00111,
      -0.00057, 0.00056, -0.00042, 0.00042, 0.00038, -0.00024, -0.00017, -0.00007,
       0.00004, 0.00004, 0.00003, 0.00003, -0.00003, 0.00003, -0.00002, -0.00002, 0.00002]
```

**Step 5 — additional planetary corrections** (all in radians; `A[0..13]`, `ac[0..13]`):
```
A[0]  = 299.7  + 0.107408*k  - 0.009173*T*T
A[1]  = 251.88 + 0.016321*k
A[2]  = 251.83 + 26.651886*k
A[3]  = 349.42 + 36.412478*k
A[4]  = 84.66  + 18.206239*k
A[5]  = 141.74 + 53.303771*k
A[6]  = 207.17 + 2.453732*k
A[7]  = 154.84 + 7.30686*k
A[8]  = 34.52  + 27.261239*k
A[9]  = 207.19 + 0.121824*k
A[10] = 291.34 + 1.844379*k
A[11] = 161.72 + 24.198154*k
A[12] = 239.56 + 25.513099*k
A[13] = 331.55 + 3.592518*k

additional = sum(ac[i] * sin(A[i]) for i in 0..13)
ac = [0.000325, 0.000165, 0.000164, 0.000126, 0.00011, 0.000062, 0.00006,
      0.000056, 0.000047, 0.000042, 0.00004, 0.000037, 0.000035, 0.000023]
```

**Step 6 — final JDE, then convert to a Gregorian calendar date:**
```
JDE = JDE_mean + correction + additional
```

Convert JDE to a Gregorian `DateOnly` via the standard Meeus JD→calendar-date algorithm (chapter
7): let `JD = JDE + 0.5`, `Z = floor(JD)`, `Fpart = JD - Z`; if `Z >= 2299161` then
`alpha = floor((Z - 1867216.25) / 36524.25)`, `A = Z + 1 + alpha - floor(alpha/4)`, else `A = Z`;
`B = A + 1524`, `C = floor((B - 122.1) / 365.25)`, `D = floor(365.25 * C)`,
`E2 = floor((B - D) / 30.6001)`; day-of-month (integer part) `= B - D - floor(30.6001*E2)`;
month `= E2 < 14 ? E2 - 1 : E2 - 13`; year `= month > 2 ? C - 4716 : C - 4715`.

## Reference: verified full-moon dates for testing

Cross-checked against multiple independent published sources (not a single source, and not
memorized):

- **2024** (12 full moons): Jan 25, Feb 24, Mar 25, Apr 23, May 23, Jun 21, Jul 21, Aug 19, Sep 17,
  Oct 17, Nov 15, Dec 15.
- **2026** (13 full moons — a year with two full moons in one calendar month, since 13×29.53 days
  spans slightly more than 12 calendar months): Jan 3, Feb 1, Mar 3, Apr 1, May 1, May 31, Jun 29,
  Jul 29, Aug 28, Sep 26, Oct 26, Nov 24, Dec 23.

If the implemented algorithm's output differs from these by more than one calendar day for any
entry, stop and re-derive the algorithm rather than adjusting the test to match — these reference
dates were cross-checked against multiple sources and are the ground truth here, not the code
under test. A one-day mismatch right at a reference date's month boundary is worth double-checking
against a third source before concluding the algorithm is wrong, since some published lists use a
local time zone rather than UTC for the "date."

---

## Task 1: Create the TemporaLinq.Astronomy project

**Files:**
- Create: `TemporaLinq/TemporaLinq.Astronomy/TemporaLinq.Astronomy.csproj`
- Modify: `TemporaLinq/TemporaLinq.sln` (add the new project)
- Modify: `TemporaLinq/TemporaLinq.Holidays/TemporaLinq.Holidays.csproj` (add project reference)

**Interfaces:**
- Produces: an empty, buildable `TemporaLinq.Astronomy` project that `TemporaLinq.Holidays` can reference.

- [ ] **Step 1: Create the project file**

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <TargetFrameworks>net8.0;net10.0</TargetFrameworks>
    </PropertyGroup>

</Project>
```

Save as `TemporaLinq/TemporaLinq.Astronomy/TemporaLinq.Astronomy.csproj` (matching the exact shape of `TemporaLinq/TemporaLinq/TemporaLinq.csproj`).

- [ ] **Step 2: Add the project to the solution**

Run: `cd TemporaLinq && dotnet sln add TemporaLinq.Astronomy/TemporaLinq.Astronomy.csproj`

- [ ] **Step 3: Add a project reference from TemporaLinq.Holidays**

Run: `cd TemporaLinq && dotnet add TemporaLinq.Holidays/TemporaLinq.Holidays.csproj reference TemporaLinq.Astronomy/TemporaLinq.Astronomy.csproj`

- [ ] **Step 4: Add a test project reference and verify the build**

Run: `cd TemporaLinq && dotnet add TemporaLinq.Test/TemporaLinq.Test.csproj reference TemporaLinq.Astronomy/TemporaLinq.Astronomy.csproj`
Then run: `dotnet build`
Expected: `Build succeeded.` with 0 errors.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Astronomy/TemporaLinq.Astronomy.csproj TemporaLinq/TemporaLinq.sln TemporaLinq/TemporaLinq.Holidays/TemporaLinq.Holidays.csproj TemporaLinq/TemporaLinq.Test/TemporaLinq.Test.csproj
git commit -m "feat: add TemporaLinq.Astronomy project"
```

---

## Task 2: Implement LunarPhaseCalculation

**Files:**
- Create: `TemporaLinq/TemporaLinq.Astronomy/LunarPhaseCalculation.cs`
- Test: `TemporaLinq/TemporaLinq.Test/LunarPhaseCalculationTest.cs`

**Interfaces:**
- Produces: `TemporaLinq.Astronomy.LunarPhaseCalculation.FullMoonsInGregorianYear(int gregorianYear) -> IEnumerable<DateOnly>`, consumed by Task 3 (Sri Lanka).

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Astronomy;

namespace TemporaLinq.Test;

public class LunarPhaseCalculationTest
{
    [Fact]
    public void FullMoonsInGregorianYear_2024_MatchesPublishedReferenceDates()
    {
        var expected = new[]
        {
            new DateOnly(2024, 1, 25), new DateOnly(2024, 2, 24), new DateOnly(2024, 3, 25),
            new DateOnly(2024, 4, 23), new DateOnly(2024, 5, 23), new DateOnly(2024, 6, 21),
            new DateOnly(2024, 7, 21), new DateOnly(2024, 8, 19), new DateOnly(2024, 9, 17),
            new DateOnly(2024, 10, 17), new DateOnly(2024, 11, 15), new DateOnly(2024, 12, 15),
        };

        var actual = LunarPhaseCalculation.FullMoonsInGregorianYear(2024).ToList();

        actual.Should().HaveCount(12);
        for (var i = 0; i < expected.Length; i++)
        {
            actual[i].Should().BeCloseTo(expected[i], 1,
                $"full moon #{i} in 2024 should be within 1 day of the published reference date");
        }
    }

    [Fact]
    public void FullMoonsInGregorianYear_2026_HasThirteenFullMoonsAndMatchesReferenceDates()
    {
        // 2026 has 13 full moons (two in May: May 1 and May 31), since the ~354-day span of
        // 12 lunations is shorter than a calendar year and periodically an extra one fits.
        var expected = new[]
        {
            new DateOnly(2026, 1, 3), new DateOnly(2026, 2, 1), new DateOnly(2026, 3, 3),
            new DateOnly(2026, 4, 1), new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31),
            new DateOnly(2026, 6, 29), new DateOnly(2026, 7, 29), new DateOnly(2026, 8, 28),
            new DateOnly(2026, 9, 26), new DateOnly(2026, 10, 26), new DateOnly(2026, 11, 24),
            new DateOnly(2026, 12, 23),
        };

        var actual = LunarPhaseCalculation.FullMoonsInGregorianYear(2026).ToList();

        actual.Should().HaveCount(13);
        for (var i = 0; i < expected.Length; i++)
        {
            actual[i].Should().BeCloseTo(expected[i], 1,
                $"full moon #{i} in 2026 should be within 1 day of the published reference date");
        }
    }

    [Fact]
    public void FullMoonsInGregorianYear_ReturnsDatesInAscendingOrderWithinTheYear()
    {
        var actual = LunarPhaseCalculation.FullMoonsInGregorianYear(2025).ToList();

        actual.Should().BeInAscendingOrder();
        actual.Should().OnlyContain(d => d.Year == 2025);
    }
}
```

Note: FluentAssertions' `DateOnly` `BeCloseTo` overload takes a day tolerance as an int (number of
days), not a `TimeSpan` — confirm the exact overload available in this codebase's FluentAssertions
version when writing this; if `BeCloseTo` isn't available for `DateOnly`, assert
`Math.Abs(actual[i].DayNumber - expected[i].DayNumber).Should().BeLessOrEqualTo(1)` instead.

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LunarPhaseCalculationTest`
Expected: FAIL (compile error — `LunarPhaseCalculation` does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
namespace TemporaLinq.Astronomy;

/// <summary>
/// Computes lunar phase events using Meeus' truncated astronomical algorithm (a periodic-term
/// series derived from lunar theory, not a linear approximation - see Jean Meeus, "Astronomical
/// Algorithms," 2nd ed., chapter 49). Accurate to well under a minute for the foreseeable past
/// and future (roughly +/-200-300 years of the present); accuracy slowly degrades many centuries
/// further out because of growing uncertainty in Delta-T (the gap between Terrestrial Time and
/// Earth's actual, slightly irregular rotation). This implementation does not apply a Delta-T
/// correction, since it is at most a few minutes across this codebase's practical year range -
/// far smaller than the day-level granularity being computed here.
/// </summary>
public static class LunarPhaseCalculation
{
    private static readonly double[] FullMoonPeriodicTerms =
    [
        -0.40614, 0.17302, 0.01614, 0.01043, 0.00734, -0.00515, 0.00209, -0.00111,
        -0.00057, 0.00056, -0.00042, 0.00042, 0.00038, -0.00024, -0.00017, -0.00007,
        0.00004, 0.00004, 0.00003, 0.00003, -0.00003, 0.00003, -0.00002, -0.00002, 0.00002,
    ];

    private static readonly double[] AdditionalCorrectionCoefficients =
    [
        0.000325, 0.000165, 0.000164, 0.000126, 0.00011, 0.000062, 0.00006,
        0.000056, 0.000047, 0.000042, 0.00004, 0.000037, 0.000035, 0.000023,
    ];

    /// <summary>
    /// Returns the Gregorian date of every full moon that falls within the given Gregorian year
    /// (typically 12, occasionally 13).
    /// </summary>
    public static IEnumerable<DateOnly> FullMoonsInGregorianYear(int gregorianYear)
    {
        // Scan lunation numbers k covering the target year with a one-lunation margin on each
        // side, since a full moon computed from a "nearest to decimal year" k can land just
        // outside the target year.
        var approximateK = (gregorianYear - 2000) * 12.3685;
        var startK = Math.Floor(approximateK) - 2;
        var endK = Math.Ceiling(approximateK) + 14;

        for (var k = startK + 0.5; k <= endK; k += 1.0)
        {
            var date = DateFromJde(FullMoonJde(k));
            if (date.Year == gregorianYear)
                yield return date;
        }
    }

    private static double FullMoonJde(double k)
    {
        var t = k / 1236.85;
        var jdeMean = 2451550.09766 + 29.530588861 * k
            + 0.00015437 * t * t
            - 0.00000015 * t * t * t
            + 0.00000000073 * t * t * t * t;

        var e = 1 - 0.002516 * t - 0.0000074 * t * t;

        double Deg(double degrees) => degrees * Math.PI / 180.0;

        var m = Deg(2.5534 + 29.1053567 * k - 0.0000014 * t * t - 0.00000011 * t * t * t);
        var mPrime = Deg(201.5643 + 385.81693528 * k + 0.0107582 * t * t + 0.00001238 * t * t * t
            - 0.000000058 * t * t * t * t);
        var f = Deg(160.7108 + 390.67050284 * k - 0.0016118 * t * t - 0.00000227 * t * t * t
            + 0.000000011 * t * t * t * t);
        var omega = Deg(124.7746 - 1.56375588 * k + 0.0020672 * t * t + 0.00000215 * t * t * t);

        var fc = FullMoonPeriodicTerms;
        var correction =
            fc[0] * Math.Sin(mPrime) + fc[1] * Math.Sin(m) * e + fc[2] * Math.Sin(2 * mPrime)
            + fc[3] * Math.Sin(2 * f) + fc[4] * Math.Sin(mPrime - m) * e
            + fc[5] * Math.Sin(mPrime + m) * e + fc[6] * Math.Sin(2 * m) * e * e
            + fc[7] * Math.Sin(mPrime - 2 * f) + fc[8] * Math.Sin(mPrime + 2 * f)
            + fc[9] * Math.Sin(2 * mPrime + m) * e + fc[10] * Math.Sin(3 * mPrime)
            + fc[11] * Math.Sin(m + 2 * f) * e + fc[12] * Math.Sin(m - 2 * f) * e
            + fc[13] * Math.Sin(2 * mPrime - m) * e + fc[14] * Math.Sin(omega)
            + fc[15] * Math.Sin(mPrime + 2 * m) + fc[16] * Math.Sin(2 * (mPrime - f))
            + fc[17] * Math.Sin(3 * m) + fc[18] * Math.Sin(mPrime + m - 2 * f)
            + fc[19] * Math.Sin(2 * (mPrime + f)) + fc[20] * Math.Sin(mPrime + m + 2 * f)
            + fc[21] * Math.Sin(mPrime - m + 2 * f) + fc[22] * Math.Sin(mPrime - m - 2 * f)
            + fc[23] * Math.Sin(3 * mPrime + m) + fc[24] * Math.Sin(4 * mPrime);

        double[] a =
        [
            Deg(299.7 + 0.107408 * k - 0.009173 * t * t),
            Deg(251.88 + 0.016321 * k),
            Deg(251.83 + 26.651886 * k),
            Deg(349.42 + 36.412478 * k),
            Deg(84.66 + 18.206239 * k),
            Deg(141.74 + 53.303771 * k),
            Deg(207.17 + 2.453732 * k),
            Deg(154.84 + 7.30686 * k),
            Deg(34.52 + 27.261239 * k),
            Deg(207.19 + 0.121824 * k),
            Deg(291.34 + 1.844379 * k),
            Deg(161.72 + 24.198154 * k),
            Deg(239.56 + 25.513099 * k),
            Deg(331.55 + 3.592518 * k),
        ];

        var additional = 0.0;
        for (var i = 0; i < a.Length; i++)
            additional += AdditionalCorrectionCoefficients[i] * Math.Sin(a[i]);

        return jdeMean + correction + additional;
    }

    private static DateOnly DateFromJde(double jde)
    {
        var jd = jde + 0.5;
        var z = Math.Floor(jd);

        double aValue;
        if (z >= 2299161)
        {
            var alpha = Math.Floor((z - 1867216.25) / 36524.25);
            aValue = z + 1 + alpha - Math.Floor(alpha / 4);
        }
        else
        {
            aValue = z;
        }

        var b = aValue + 1524;
        var c = Math.Floor((b - 122.1) / 365.25);
        var d = Math.Floor(365.25 * c);
        var e = Math.Floor((b - d) / 30.6001);

        var day = (int) (b - d - Math.Floor(30.6001 * e));
        var month = (int) (e < 14 ? e - 1 : e - 13);
        var year = (int) (month > 2 ? c - 4716 : c - 4715);

        return new DateOnly(year, month, day);
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LunarPhaseCalculationTest`
Expected: PASS, 3 tests. If any reference date is off by more than 1 day, re-check the ported
formula against the plan's "Reference: the verified algorithm" section character-by-character
before touching the test data.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Astronomy/LunarPhaseCalculation.cs TemporaLinq/TemporaLinq.Test/LunarPhaseCalculationTest.cs
git commit -m "feat: add LunarPhaseCalculation"
```

---

## Task 3: Add Sri Lanka national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/SriLanka/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/SriLankaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>` base, `TemporaLinq.Astronomy.LunarPhaseCalculation.FullMoonsInGregorianYear(int) -> IEnumerable<DateOnly>` from Task 2, `HolidayNames` members (new `PoyaDay`, plus reused existing members).

- [ ] **Step 1: Research and verify Sri Lanka's fixed-date national holidays**

Use WebSearch to confirm Sri Lanka's actual official public holidays beyond Poya days for 2026 —
do not rely on memorized facts. At minimum verify: Tamil Thai Pongal Day (mid-January, fixed
Gregorian date, ~Jan 14/15), Independence Day (Feb 4), Sinhala and Tamil New Year (mid-April,
typically Apr 13-14), May Day/International Workers' Day (May 1), Christmas Day (Dec 25). Note any
additional fixed civil or religious holidays the search turns up, and record the exact dates and
sources in this task's implementation.

- [ ] **Step 2: Write the failing test**

Write `TemporaLinq/TemporaLinq.Test/Holidays/Asia/SriLankaTest.cs` following the exact shape of
existing country tests (e.g. `TemporaLinq/TemporaLinq.Test/Holidays/Asia/PakistanTest.cs`): one
test asserting the total holiday count for 2026 (12 or 13 Poya days depending on that year's full
moon count, plus the verified fixed-date holidays), one test asserting each fixed-date holiday,
and one test asserting that every date `LunarPhaseCalculation.FullMoonsInGregorianYear(2026)`
returns is present in the computed holiday set as `PoyaDay`.

- [ ] **Step 3: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SriLankaTest`
Expected: FAIL (compile error — namespace/members do not exist yet).

- [ ] **Step 4: Write the implementation**

`TemporaLinq/TemporaLinq.Holidays/Asia/SriLanka/NationalHolidays.cs`, following the exact
`HolidayEnumerable<NationalHolidays>` / `[Cache]`-memoized pattern used by every other country,
merging the fixed-date holidays with a `PoyaDay` entry for each
`LunarPhaseCalculation.FullMoonsInGregorianYear(year)` result, sorted via `.Order()` before
`.ToImmutableList()` exactly like existing implementations.

- [ ] **Step 5: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SriLankaTest`
Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/SriLanka/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/SriLankaTest.cs
git commit -m "feat: add Sri Lanka national holidays"
```

---

## Task 4: Mark Sri Lanka done in the spec checklist and run the full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1: Update the checklist**

In the Asia section's Tier AS4 line, move Sri Lanka from the 🔴 list to a "Done: ✅ Sri Lanka
(full-moon-computable via LunarPhaseCalculation)" line, matching the exact style of prior done
entries.

- [ ] **Step 2: Run the full test suite**

Run: `cd TemporaLinq && dotnet test --framework net10.0`
Expected: All tests pass, 0 failures.

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Sri Lanka done in worldwide holidays checklist"
```
