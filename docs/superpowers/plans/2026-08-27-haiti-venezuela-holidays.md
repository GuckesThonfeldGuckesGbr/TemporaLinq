# Haiti and Venezuela Holidays Implementation Plan (Roadmap Phase 6)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement Haiti and Venezuela's national holidays. Both were originally flagged 🔴 for political/administrative reasons (governments periodically shift or add holidays by decree, e.g. Venezuela's "puente" bridge-day moves), not because their calendars are uncomputable — both run on the standard Gregorian/Christian-Easter calendar throughout. This ships the stable, well-documented subset with a caveat about decree-based volatility, the same treatment already given to Bosnia and Herzegovina's entity-fragmentation issue.

**Architecture:** Standard `HolidayEnumerable<T>` pattern. Haiti at `TemporaLinq.Holidays/NorthAmerica/Haiti/`, Venezuela at `TemporaLinq.Holidays/SouthAmerica/Venezuela/` (new `SouthAmerica` folder). Both use `EasterSundayCalculation.Christian` for their movable feasts (Carnival Monday/Tuesday, Maundy Thursday, Good Friday).

**Tech Stack:** C#/.NET (net8.0 + net10.0), xUnit + FluentAssertions.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

## Global Constraints

- Both countries' governments periodically shift specific holidays by decree (e.g. to create long weekends) or add one-off commemorative days. This implementation covers the stable, consistently-observed annual holidays only, documented as a caveat on each class analogous to the Hijri ±1-2 day moon-sighting note.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task.

## Reference: holiday lists (verified via WebSearch, not memorized)

### Haiti — 12 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `IndependenceDay` (reuse — Haiti's Jan 1 is both New Year's Day and Independence Day, 1804) |
| Jan 2 | `AncestryDay` (new) |
| easter - 48 | `CarnivalMonday` (new) |
| easter - 47 | `CarnivalTuesday` (new) |
| easter - 2 | `GoodFriday` |
| May 1 | `LabourDay` (Labour and Agriculture Day) |
| May 18 | `FlagAndUniversitiesDay` (new) |
| Aug 15 | `AssumptionDay` |
| Oct 17 | `DessalinesMemorialDay` (new) |
| Nov 1 | `AllSaintsDay` |
| Nov 2 | `AllSoulsDay` |
| Dec 25 | `ChristmasDay` |

### Venezuela — 13 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| easter - 48 | `CarnivalMonday` (reuse) |
| easter - 47 | `CarnivalTuesday` (reuse) |
| easter - 3 | `MaundyThursday` |
| easter - 2 | `GoodFriday` |
| May 1 | `LabourDay` |
| Jun 24 | `BattleOfCarababoDay` (new) |
| Jul 5 | `IndependenceDay` (reuse) |
| Jul 24 | `BolivarsBirthday` (new) |
| Oct 12 | `IndigenousResistanceDay` (new) |
| Dec 24 | `ChristmasEve` (reuse) |
| Dec 25 | `ChristmasDay` |
| Dec 31 | `NewYearsEve` (reuse) |

---

## Task 1: Add new HolidayNames enum members

- [ ] **Step 1: Edit the enum** in `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`, inserting alphabetically:
`AncestryDay` (Haiti), `BattleOfCarababoDay` (Venezuela), `BolivarsBirthday` (Venezuela),
`CarnivalMonday` (Haiti, Venezuela), `CarnivalTuesday` (Haiti, Venezuela),
`DessalinesMemorialDay` (Haiti), `FlagAndUniversitiesDay` (Haiti),
`IndigenousResistanceDay` (Venezuela). Broaden `IndependenceDay` and `ChristmasEve`/`NewYearsEve` comments to add Haiti/Venezuela.
- [ ] **Step 2:** `cd TemporaLinq && dotnet build` — expect success.
- [ ] **Step 3:** `git add` + `git commit -m "feat: add HolidayNames values for Haiti and Venezuela"`

## Task 2: Add Haiti national holidays

- [ ] **Step 1:** Write `TemporaLinq/TemporaLinq.Test/Holidays/NorthAmerica/HaitiTest.cs` (failing test) asserting the 12-holiday list above for 2026, following the exact per-country test pattern (e.g. `TemporaLinq.Test/Holidays/Asia/TurkeyTest.cs`).
- [ ] **Step 2:** Run, confirm it fails to compile.
- [ ] **Step 3:** Write `TemporaLinq/TemporaLinq.Holidays/NorthAmerica/Haiti/NationalHolidays.cs` implementing the table above via `EasterSundayCalculation.Christian`, `.Order().ToImmutableList()`, `[Cache]`-memoized, matching every other country's shape. Include an XML doc comment noting decree-based holiday shifts are out of scope.
- [ ] **Step 4:** Run, confirm pass.
- [ ] **Step 5:** `git add` + `git commit -m "feat: add Haiti national holidays"`

## Task 3: Add Venezuela national holidays

- [ ] **Step 1:** Write `TemporaLinq/TemporaLinq.Test/Holidays/SouthAmerica/VenezuelaTest.cs` (failing test) asserting the 13-holiday list above for 2026 (create the `SouthAmerica` test folder).
- [ ] **Step 2:** Run, confirm it fails to compile.
- [ ] **Step 3:** Write `TemporaLinq/TemporaLinq.Holidays/SouthAmerica/Venezuela/NationalHolidays.cs` (create the `SouthAmerica` folder), same shape as Haiti's, with the same decree-shift caveat documented.
- [ ] **Step 4:** Run, confirm pass.
- [ ] **Step 5:** `git add` + `git commit -m "feat: add Venezuela national holidays"`

## Task 4: Update checklist and run full suite

- [ ] **Step 1:** Update `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`'s North America / South America sections to mark Haiti and Venezuela done (✅), with the decree-volatility caveat noted, matching prior tiers' style.
- [ ] **Step 2:** `cd TemporaLinq && dotnet test --framework net10.0` — expect 0 failures.
- [ ] **Step 3:** `git add` + `git commit -m "docs: mark Haiti and Venezuela done in worldwide holidays checklist"`
