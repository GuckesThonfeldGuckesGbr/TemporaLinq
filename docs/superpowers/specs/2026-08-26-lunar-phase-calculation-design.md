# Lunar Phase Calculation Design (Phase 1 of the remaining-gaps roadmap)

## Context

This is Phase 1 of a multi-phase roadmap to close the remaining 🔴-flagged countries in
`docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`:

1. **Full-moon primitive + Sri Lanka** (this design)
2. Southeast Asian Buddhist lunisolar calendar (Thailand, Myanmar, Cambodia, Laos; also completes
   Vesak for Singapore/Malaysia/Indonesia)
3. Nepal's civil calendar (Bikram Sambat)
4. Hindu lunisolar calendar (completes India, Nepal, Malaysia, Singapore, Bangladesh)
5. Balinese Saka calendar (completes Indonesia)
6. Haiti and Venezuela (documented-caveat partial implementation, not a calendar mechanism)
7. Mongolia (Tibetan Buddhist lunar calendar) — deliberately last, the hardest remaining case

Sri Lanka's Poya holidays are a public holiday on every full moon of the year — no leap-month
bookkeeping, no calendar-month-name mapping, just "the date of each full moon." That makes a
standalone lunar-phase calculation the natural, minimal building block for this phase, and it
also becomes the foundation Phase 2's Southeast Asian Buddhist calendar builds on.

## Packaging note

The user's long-term plan is to distribute `TemporaLinq` as multiple selectable NuGet packages.
Lunar phase calculation is generically useful (not holiday-specific) and is called out as a
candidate standalone "moon phases" package. To make that split free later, this design puts the
calculation in its own project, `TemporaLinq.Astronomy`, from the start — `TemporaLinq.Holidays`
references it like any other dependency, and packaging it separately later is purely a matter of
adding NuGet metadata to an already-independent project, no code motion required.

## Algorithm

**Chosen approach: Meeus' truncated astronomical algorithm** (Jean Meeus, *Astronomical
Algorithms*, chapter 49, "Phases of the Moon"). A polynomial approximation for the time of the
k-th lunation (in Julian centuries since J2000), corrected by a periodic-term series derived from
lunar theory (solar/lunar mean anomalies, the Moon's argument of latitude, eccentricity
correction, etc.). Pure arithmetic, no external dependencies — consistent with how
`EasterSundayCalculation` and `EthiopianCalendarCalculation` are already hand-rolled in this
codebase.

Rejected alternatives:
- **Naive mean-synodic-month approximation** (reference full moon + N × 29.53059 days) — a
  one-line formula, but the Moon's orbital speed varies through each month (elliptical orbit,
  solar perturbation), so this approach genuinely drifts and would misdate full moons by up to a
  day in many months. Not accurate enough for holiday dates.
- **Third-party astronomy NuGet package** — avoids writing the math, but breaks this codebase's
  dependency-free convention and adds an external maintenance/versioning risk for a well-documented,
  implementable formula.

### Accuracy characteristics (documented on the class, and here for future reference)

Meeus' periodic-term series is not a linear approximation — it tracks the Moon's true position
and oscillates around the real full-moon instant with sub-minute accuracy for centuries around the
present; it does not compound or drift the way the naive approach would. The one real limitation
is ΔT (the difference between Terrestrial/Dynamical Time, which the formula computes in, and
Earth's actual, slightly irregular rotation, which civil dates are measured against). ΔT is
well-constrained for the recent past and near future — within roughly ±200–300 years of today, its
uncertainty is well under a minute, never enough to shift which calendar date a full moon lands
on. Many centuries further out, ΔT's uncertainty grows and the computed date could eventually be
off by an hour or more. This library's existing test range (roughly 1950–2200, per `CacheTest`)
sits solidly inside the accurate regime.

## API

```csharp
namespace TemporaLinq.Astronomy;

/// <summary>
/// Computes lunar phase events using Meeus' truncated astronomical algorithm (a periodic-term
/// series derived from lunar theory, not a linear approximation). Accurate to well under a
/// minute for the foreseeable past and future (roughly +/-200-300 years of the present); accuracy
/// slowly degrades many centuries further out because of growing uncertainty in Delta-T (the gap
/// between Terrestrial Time and Earth's actual, slightly irregular rotation).
/// </summary>
public static class LunarPhaseCalculation
{
    /// <summary>
    /// Returns the Gregorian date of every full moon that falls within the given Gregorian year
    /// (typically 12, occasionally 13 per year).
    /// </summary>
    public static IEnumerable<DateOnly> FullMoonsInGregorianYear(int gregorianYear);
}
```

Implementation approach: iterate lunation numbers `k` (integers, each representing one full lunar
cycle since the J2000 reference new moon) covering the target Gregorian year with margin on both
sides, compute each candidate full moon's Julian Ephemeris Day via Meeus' polynomial-plus-periodic-terms
formula, convert to a Gregorian calendar date, and keep the ones landing inside
`[gregorianYear-01-01, gregorianYear-12-31]`.

## Testing

`LunarPhaseCalculationTest` asserts `FullMoonsInGregorianYear` against a batch of independently
verified reference full-moon dates spanning multiple years (verified via WebSearch against
published astronomical sources, the same diligence used for `EthiopianCalendarCalculation`), plus
a count check (12 or 13 per year) across a range of years.

## Sri Lanka

`TemporaLinq.Holidays/Asia/SriLanka/NationalHolidays.cs`: one `PoyaDay` holiday (new
`HolidayNames` member) for every date `LunarPhaseCalculation.FullMoonsInGregorianYear(year)`
returns, plus Sri Lanka's fixed civil/religious holidays (Independence Day, National Day, Tamil
Thai Pongal Day, Sinhala and Tamil New Year, Christian and Hindu fixed-date holidays, etc. — exact
list and dates verified via WebSearch, not assumed from memory, matching every other country
tier's process). Poya days that coincide with another named holiday are still emitted as
`PoyaDay` for that date in addition to the other holiday (matching the existing convention of
multiple `Holiday` entries sharing a date, e.g. Serbia's Jan 1/2).

## Out of scope for this design

- Phases 2–7 of the roadmap — each gets its own design when its turn comes.
- Any non-full-moon lunar phase (new moon, quarters) — add only if a later phase actually needs
  one; YAGNI for now.
- Actual NuGet packaging/publishing of `TemporaLinq.Astronomy` as a standalone package — only the
  project-boundary seam is established now.
