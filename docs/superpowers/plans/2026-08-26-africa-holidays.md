# Tier AF1 (partial) + Ethiopia Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for Nigeria, Egypt, and Morocco (all Hijri- and, for Nigeria/Egypt, Easter/Coptic-Easter-computable) to `TemporaLinq.Holidays`, following the exact pattern already used by the Europe tiers, and attempt Ethiopia (Ethiopian-calendar-computable, requires a brand-new `EthiopianCalendarCalculation` mechanism).

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record at `TemporaLinq.Holidays/Africa/<Country>/NationalHolidays.cs`, computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`), using `HijriCalendarCalculation.DatesInGregorianYear` for Islamic-calendar holidays, `EasterSundayCalculation.Christian` (Nigeria) / `EasterSundayCalculation.ChristianOrthodox` (Egypt, Sham el-Nessim) for movable Christian/Coptic feasts, and (if built) a new `EthiopianCalendarCalculation.DateInGregorianYear` for Ethiopia. New `HolidayNames` enum members are added once, up front, then reused by every country task. Each country also gets a test file at `TemporaLinq.Test/Holidays/Africa/<Country>Test.cs` following the existing per-country test pattern (e.g. `HungaryTest`).

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`,
`docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`

## Global Constraints

- Hijri-based holiday dates are approximations (tabular Hijri calendar via `HijriCalendarCalculation`); real-world moon-sighting announcements can differ by +/-1, rarely +/-2 days. Each country file documents this in its XML doc comment.
- `HijriCalendarCalculation.DatesInGregorianYear` returns 1 or (rarely) 2 dates for a given Hijri (month, day) within a Gregorian year; country implementations must handle both by iterating over all returned dates (matching how a multi-day Hijri holiday, e.g. a 2-day Eid, already needs two separate (month,day) calls, one per day of the holiday).
- Egypt's Sham el-Nessim is the day after Coptic (Eastern Orthodox computus) Easter Sunday — use `EasterSundayCalculation.ChristianOrthodox.ForYear(year).AddDays(1)`.
- Ethiopia is an **attempt, not a commitment**. `EthiopianCalendarCalculation` has no existing .NET support. Before writing it into the repo, its day-number-offset formula was independently verified against a maintained third-party reference implementation (Python `ethiopian-date-converter`, sourced from GitHub) across 36 (Ethiopian date, Gregorian date) reference pairs spanning 7 different Gregorian years, both Ethiopian leap and non-leap years, and the Pagume (13th month) boundary — all 36 passed exactly, plus a scripted sweep confirming zero gaps/duplicates for the six specific (month, day) pairs this plan actually uses, across Gregorian years 1900-2200. If this verification had not succeeded, Ethiopia would have been skipped entirely per the design doc's guidance — see the design doc for the full verification writeup.
- Countries live at `TemporaLinq.Holidays/Africa/<Country>/NationalHolidays.cs`; tests at `TemporaLinq.Test/Holidays/Africa/<Country>Test.cs`.
- Reuse existing `HolidayNames` enum members wherever the concept matches (broadening the `//` comment to list the additional country), per the established convention. Only add new enum members for genuinely new concepts.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox).
- Test year: 2026 (matching the codebase convention), asserting known holiday dates including movable/Hijri feasts computed the same way the implementation computes them (i.e. tests call `HijriCalendarCalculation`/`EasterSundayCalculation`/`EthiopianCalendarCalculation` directly rather than hardcoding independently-researched dates, matching the existing test style for movable feasts).
- After all countries are done (and Ethiopia's outcome is settled either way), update the checklist in `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md` (Africa section) and, if Ethiopia was implemented, the calendar-mechanisms design doc's status line.

---

## Reference: full holiday list per country

### Nigeria (`EasterSundayCalculation.Christian` + Hijri) — 11 occasions, 13 dates
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Shawwal 1, 2 | `EidAlFitr` (x2 dates) |
| easter - 2 | `GoodFriday` |
| easter + 1 | `EasterMonday` |
| May 1 | `LabourDay` (Workers' Day) |
| Dhu al-Hijjah 10, 11 | `EidAlAdha` (x2 dates) |
| Jun 12 | `DemocracyDayOfNigeria` (new) |
| Rabi' al-Awwal 12 | `ProphetsBirthday` (new — Id el-Maulud) |
| Oct 1 | `IndependenceDay` (reuse) |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `BoxingDay` |

### Egypt (`EasterSundayCalculation.ChristianOrthodox` + Hijri) — 16 dates
| Date | HolidayNames member |
|---|---|
| Jan 7 | `ChristmasDay` (reuse — Coptic Christmas) |
| Jan 25 | `RevolutionDayOfEgypt` (new — 2011 revolution) |
| Apr 25 | `SinaiLiberationDay` (new) |
| copticEaster + 1 | `ShamElNessim` (new) |
| May 1 | `LabourDay` |
| Shawwal 1, 2, 3 | `EidAlFitr` (x3 dates) |
| Dhu al-Hijjah 9 | `ArafatDay` (new — Day of Arafat) |
| Dhu al-Hijjah 10, 11, 12 | `EidAlAdha` (x3 dates) |
| Muharram 1 | `IslamicNewYear` (new) |
| Jul 23 | `RevolutionDayOfEgypt` (reuse — 1952 revolution) |
| Rabi' al-Awwal 12 | `ProphetsBirthday` |
| Oct 6 | `ArmedForcesDay` (new) |

### Morocco (Hijri only) — 15 dates
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 11 | `IndependenceManifestoDay` (new) |
| Shawwal 1, 2 | `EidAlFitr` (x2 dates) |
| May 1 | `LabourDay` |
| Dhu al-Hijjah 10, 11 | `EidAlAdha` (x2 dates) |
| Muharram 1 | `IslamicNewYear` |
| Rabi' al-Awwal 12 | `ProphetsBirthday` |
| Jul 30 | `ThroneDayOfMorocco` (new) |
| Aug 14 | `OuedEdDahabDay` (new) |
| Aug 20 | `RevolutionOfTheKingAndThePeopleDay` (new) |
| Aug 21 | `YouthDayOfMorocco` (new) |
| Nov 6 | `GreenMarchDay` (new) |
| Nov 18 | `IndependenceDay` (reuse) |

### Ethiopia (attempt — `EthiopianCalendarCalculation` + `EasterSundayCalculation.ChristianOrthodox`) — 8 dates
| Ethiopian date | HolidayNames member |
|---|---|
| 1 Meskerem | `EthiopianNewYear` (new — Enkutatash) |
| 17 Meskerem | `FindingOfTheTrueCross` (new — Meskel) |
| 29 Tahsas | `ChristmasDay` (reuse — Genna) |
| 11 Tir | `Epiphany` (reuse — Timkat) |
| 23 Yekatit | `AdwaVictoryDay` (new) |
| 27 Miazia | `PatriotsVictoryDay` (new) |
| orthodoxEaster - 2 | `GoodFriday` |
| orthodoxEaster | `EasterSunday` |

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

Add (alphabetically, per existing convention): `AdwaVictoryDay`, `ArafatDay`, `ArmedForcesDay`, `DemocracyDayOfNigeria`, `EidAlAdha`, `EidAlFitr`, `EthiopianNewYear`, `FindingOfTheTrueCross`, `GreenMarchDay`, `IndependenceManifestoDay`, `IslamicNewYear`, `OuedEdDahabDay`, `PatriotsVictoryDay`, `ProphetsBirthday`, `RevolutionDayOfEgypt`, `RevolutionOfTheKingAndThePeopleDay`, `ShamElNessim`, `SinaiLiberationDay`, `ThroneDayOfMorocco`, `YouthDayOfMorocco`. Broaden comments on `NewYearsDay`, `LabourDay`, `IndependenceDay`, `ChristmasDay`, `BoxingDay`, `GoodFriday`, `EasterMonday`, `EasterSunday`, `Epiphany` to list the new countries.

- [x] Add enum members and broaden comments; `dotnet build` passes.

## Task 2: EthiopianCalendarCalculation (attempt)

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/EthiopianCalendarCalculation.cs`
- Create: `TemporaLinq/TemporaLinq.Test/Holidays/EthiopianCalendarCalculationTest.cs`

Implement `DateInGregorianYear(int gregorianYear, int ethiopianMonth, int ethiopianDay)` using the day-number-offset formula verified in the design-doc writeup (epoch constant relative to `DateOnly.DayNumber`, 365-day years with a leap day added when `ethiopianYear % 4 == 0` relative to the running day count, i.e. one intercalary day every 4 years). Try candidate Ethiopian years `gregorianYear - 9` through `gregorianYear - 6` and return whichever converts into a date within `gregorianYear`; throw `InvalidOperationException` if none match (mirrors `HebrewCalendarCalculation`'s defensive throw).

- [x] Failing test first (reference pairs from the verification), then implementation, then green. Commit.

## Task 3: Nigeria

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Africa/Nigeria/NationalHolidays.cs`
- Create: `TemporaLinq/TemporaLinq.Test/Holidays/Africa/NigeriaTest.cs`

- [x] Red, green, commit `feat: add Nigeria national holidays`.

## Task 4: Egypt

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Africa/Egypt/NationalHolidays.cs`
- Create: `TemporaLinq/TemporaLinq.Test/Holidays/Africa/EgyptTest.cs`

- [x] Red, green, commit `feat: add Egypt national holidays`.

## Task 5: Morocco

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Africa/Morocco/NationalHolidays.cs`
- Create: `TemporaLinq/TemporaLinq.Test/Holidays/Africa/MoroccoTest.cs`

- [x] Red, green, commit `feat: add Morocco national holidays`.

## Task 6: Ethiopia (if Task 2 succeeded)

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Africa/Ethiopia/NationalHolidays.cs`
- Create: `TemporaLinq/TemporaLinq.Test/Holidays/Africa/EthiopiaTest.cs`

- [x] Red, green, commit `feat: add Ethiopia national holidays`.

## Task 7: Update checklist docs

- [x] Mark Nigeria, Egypt, Morocco (and Ethiopia, if implemented) done in the worldwide holidays checklist; update the calendar-mechanisms design doc's status line if Ethiopia shipped. Run full suite. Commit.
