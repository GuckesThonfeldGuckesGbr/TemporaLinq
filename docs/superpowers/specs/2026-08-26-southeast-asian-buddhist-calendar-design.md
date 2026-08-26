# Southeast Asian Buddhist Lunisolar Calendar Design (Roadmap Phase 2)

## Context

Phase 2 of the remaining-gaps roadmap (`docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`).
Thailand, Myanmar, Cambodia, and Laos share a Buddhist lunisolar calendar tradition whose holy
days — Makha Bucha, Visakha Bucha (Vesak), Asalha Bucha — are each "the full moon of a specific
traditional lunar month." Singapore, Malaysia, and Indonesia observe the same Vesak (their Hijri,
Chinese-lunisolar, and Christian components are already implemented; Vesak was their remaining
Buddhist-calendar gap).

An initial hypothesis — that these holidays are simply "the full moon nearest a target Gregorian
month," directly reusable from Phase 1's `LunarPhaseCalculation` — turned out to be wrong. Verified
against Thailand's 2026 Vesak (May 31, not May 1, even though 2026 has full moons on both dates):
which full moon counts as "the 6th lunar month" depends on whether a leap month was inserted
earlier in that lunar year, which shifts every later month's count. The one existing open-source
implementation found during research (a 2005-era C# library, `novalis78/Buddhist-Calendar-Library`)
does not handle this correctly either — it naively maps "the full moon in Gregorian month N" to
"lunar month N," which cannot represent a 13-month leap year at all. There is no trustworthy
reference implementation to port, unlike Phase 1's moon-phase algorithm.

## Algorithm

### Building blocks (both extend `TemporaLinq.Astronomy`, both directly ported from the same
### verified source as Phase 1's full-moon calculation — `soniakeys/meeus`, MIT licensed, page-cited
### against Meeus' *Astronomical Algorithms*)

**`LunarPhaseCalculation.NewMoonsInGregorianYear(int) -> IEnumerable<DateOnly>`** — identical
structure to the existing `FullMoonsInGregorianYear`, but with `q = 0` instead of `q = 0.5` in the
lunation-snap formula, and the sibling "new moon" periodic-term coefficient table (chapter 49):

```
nc = [-0.4072, 0.17241, 0.01608, 0.01039, 0.00739, -0.00514, 0.00208, -0.00111, -0.00057,
       0.00056, -0.00042, 0.00042, 0.00038, -0.00024, -0.00017, -0.00007, 0.00004, 0.00004,
       0.00003, 0.00003, -0.00003, 0.00003, -0.00002, -0.00002, 0.00002]
```

used in the exact same `nfc(...)` periodic-correction formula as full moon (same M, M', F, Ω
angles, same additional-corrections A[0..13]/ac[0..13] table) — only the coefficient table and the
`q` offset in the lunation number differ. Refactor `FullMoonJde`/the new `NewMoonJde` to share the
angle/additional-correction computation, parameterized by the periodic-term table, rather than
duplicating that code.

**`DecemberSolsticeCalculation.SolsticeDate(int gregorianYear) -> DateOnly`** (new file in
`TemporaLinq.Astronomy`) — Meeus chapter 27, low-precision variant (accurate to within a minute
for 1951-2050, far more than the day-level precision this needs):

```
JDE0 = Horner((year - 2000) * 0.001, 2451900.05952, 365242.74049, -0.06223, -0.00823, 0.00032)
T = (JDE0 - 2451545.0) / 36525
W = (35999.373*T - 2.47) in radians
Δλ = 1 + 0.0334*cos(W) + 0.0007*cos(2*W)
S = sum(a * cos(radians(b + c*T)) for (a, b, c) in the 24-term table below)
JDE = JDE0 + 0.00001 * S / Δλ
```

24-term table (shared across all four equinox/solstice functions in the source; only the ones
that matter for December are being used here, but the table itself is common to the algorithm and
should be copied whole, not trimmed):

```
(485, 324.96, 1934.136), (203, 337.23, 32964.467), (199, 342.08, 20.186),
(182, 27.85, 445267.112), (156, 73.14, 45036.886), (136, 171.52, 22518.443),
(77, 222.54, 65928.934), (74, 296.72, 3034.906), (70, 243.58, 9037.513),
(58, 119.81, 33718.147), (52, 297.17, 150.678), (50, 21.02, 2281.226),
(45, 247.54, 29929.562), (44, 325.15, 31555.956), (29, 60.93, 4443.417),
(18, 155.12, 67555.328), (17, 288.79, 4562.452), (16, 198.04, 62894.029),
(14, 199.76, 31436.921), (12, 95.39, 14577.848), (12, 287.11, 31931.756),
(12, 320.81, 34777.259), (9, 227.73, 1222.114), (8, 15.45, 16859.074)
```

Convert JDE to a `DateOnly` using the same JD-to-Gregorian conversion already implemented for
lunar phases.

### The Southeast Asian Buddhist lunisolar calendar itself (new: `SoutheastAsianBuddhistCalendar`)

**Leap-month (adhikamas) test**, per the traditional determination rule (cross-checked across
multiple sources during brainstorming): a lunar year is a leap-month year (13 lunar months, with
the 8th month repeated) if the new moon nearest the December solstice falls within 11 days
*before* that solstice. This is directly computable from `NewMoonsInGregorianYear` and
`DecemberSolsticeCalculation.SolsticeDate` — no static table of known leap years, consistent with
this codebase's formula-not-table philosophy (a "known leap years" list was found during research
but different sources disagreed on it, which is itself a reason not to trust or embed one).

**Month numbering**: month 1 begins at the first new moon on or after the preceding December
solstice. Months run new-moon to new-moon; each month's named full moon is the full moon falling
within that month's span. In a leap-month year, an extra month is inserted after month 8 (i.e.
month 8 occurs twice), shifting every subsequent month's number for the rest of that lunar year.

**Holy days**: Magha Bucha = month 3's full moon; Visakha Bucha (Vesak) = month 6's full moon;
Asalha Bucha = the *last* occurrence of month 8's full moon (the second one, in a leap-month
year) — Asalha Bucha traditionally marks the day immediately before Buddhist Lent (Vassa) begins,
so it always falls at the end of the (possibly doubled) 8th month regardless of leap status.

**This month-numbering design is this design's own synthesis**, not a verified port like Phase 1's
moon-phase algorithm — there is no trustworthy reference implementation for it (see Context). It
is expected to need refinement during implementation. Per the "iterate until verified" decision:
implement it, then test against published Makha/Visakha/Asalha Bucha dates across at least 4-5
years — deliberately including at least one confirmed leap-month year (2026, where Vesak = May 31
is already independently confirmed) and at least one ordinary year — refining the month-counting
logic until it matches, before considering this design done. If a specific structural detail (e.g.
exactly which occurrence of the repeated 8th month is correct) remains genuinely unresolvable after
real effort, document it as a known approximation on the class, the same way Hijri's moon-sighting
caveat is documented — do not ship a silent guess.

## API

```csharp
namespace TemporaLinq.Astronomy;

public static class SoutheastAsianBuddhistCalendar
{
    public static DateOnly MakhaBuchaDate(int gregorianYear);
    public static DateOnly VisakhaBuchaDate(int gregorianYear);
    public static DateOnly AsalhaBuchaDate(int gregorianYear);
}
```

One date per Gregorian year per holiday (each of these traditionally falls once per solar year;
no multi-occurrence complication like Hijri's drift, since this calendar's leap-month mechanism
exists specifically to keep it aligned with the solar year).

## Country scope

- **Thailand**: Makha Bucha, Visakha Bucha, Asalha Bucha (Buddhist Lent begins the day after —
  not itself a public holiday in Thailand), plus fixed civil holidays (verify via WebSearch:
  Chakri Day, Songkran Apr 13-15, Coronation Day, Queen's Birthday, King's Birthday, Chulalongkorn
  Day, etc.) and Christian/Hijri components only if actually on Thailand's official list.
- **Myanmar**: the same three Buddhist holy days (verify exact local names/dates via WebSearch),
  plus fixed civil holidays (Independence Day, Union Day, etc.).
- **Cambodia**: the same three Buddhist holy days, plus fixed civil holidays (verify via
  WebSearch — Cambodia's list is extensive and includes some dates tied to the Khmer New Year,
  which is a separate solar-calendar event, not this lunisolar one).
- **Laos**: the same three Buddhist holy days, plus fixed civil holidays.
- **Singapore, Malaysia, Indonesia**: add `VisakhaBuchaDate` (Vesak) to each country's existing
  `NationalHolidays.cs` (these already have Hijri/Chinese-lunisolar/Christian components from
  earlier tiers) — this closes the remaining gap for all three except Indonesia's Nyepi (Balinese
  Saka calendar, Phase 5) and any residual Hindu-minority holidays (Phase 4).

Exact per-country civil-holiday lists are deliberately not pinned down in this design — verify via
WebSearch during implementation, same as every prior country tier, rather than trusting memory for
four more countries' holiday laws.

## Testing

- `LunarPhaseCalculationTest` gets new cases for `NewMoonsInGregorianYear` (reference dates the
  same way full moons were verified).
- `DecemberSolsticeCalculationTest` asserts solstice dates against published reference dates for a
  few years.
- `SoutheastAsianBuddhistCalendarTest` asserts `MakhaBuchaDate`/`VisakhaBuchaDate`/`AsalhaBuchaDate`
  against published reference dates across the 4-5 verification years described above, including
  the confirmed leap-month year.
- Each country's test file follows the existing per-country pattern.

## Out of scope for this design

- Mongolia, Nepal's Hindu-calendar holidays, Indonesia's Nyepi, the broader Hindu lunisolar
  calendar — separate phases.
- Modeling the historical, administratively-adjusted irregularities in the pre-modern Thai
  calendar — this design computes the modern astronomical rule only, with the same spirit of
  approximation-with-documented-caveat already used for Hijri.
