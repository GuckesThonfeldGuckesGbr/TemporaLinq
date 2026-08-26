# Korean/Taiwanese Lunisolar Calculations + South Korea/Taiwan Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add `KoreanLunisolarCalendarCalculation` and `TaiwanLunisolarCalendarCalculation` (mirroring the existing `ChineseLunisolarCalendarCalculation`), then use them to implement national public holidays for South Korea (Tier AS1) and Taiwan (Tier AS3) in `TemporaLinq.Holidays`.

**Architecture:** Two new static calculation classes at `TemporaLinq.Holidays/{Korean,Taiwan}LunisolarCalendarCalculation.cs`, each wrapping `System.Globalization.{Korean,Taiwan}LunisolarCalendar` with the identical `DateOnly DateInGregorianYear(int gregorianYear, int lunisolarMonth, int lunisolarDay)` shape as `ChineseLunisolarCalendarCalculation`. Then two country `NationalHolidays : HolidayEnumerable<NationalHolidays>` records at `TemporaLinq.Holidays/Asia/{SouthKorea,Taiwan}/NationalHolidays.cs` (new `Asia` folder), each computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`), using `HolidayNames` enum members via `using static TemporaLinq.Holidays.HolidayNames;`. Matching tests at `TemporaLinq.Test/Holidays/Asia/{SouthKorea,Taiwan}Test.cs`.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`

## Empirical verification (done before writing this plan)

A standalone scratch console app (outside the repo) confirmed, using the real
`System.Globalization.KoreanLunisolarCalendar` and `TaiwanLunisolarCalendar`:

- Both calendars round-trip via the same `GetYear`/`ToDateTime` technique already used by
  `ChineseLunisolarCalendarCalculation` — no code changes needed to the algorithm shape.
- Korean lunisolar year numbering matches the raw Gregorian year (e.g. `GetYear(2024-02-10) ==
  2024`). **Taiwan's internal year numbering uses the ROC/Minguo era** (`GetYear(2024-02-10) ==
  113`, i.e. `Gregorian - 1911`) — irrelevant to callers of `DateInGregorianYear` since it always
  derives the native year via `GetYear`, but documented in the class's XML comment as a caveat for
  anyone reading `GetLeapMonth` results directly.
- Seollal/Taiwan Lunar New Year (month 1, day 1) 2024 = 2024-02-10 (same day as Chinese New Year,
  confirming all three calendars track the same underlying lunisolar system).
- 2025 has a leap 7th month in both Korean (`GetLeapMonth(2025) == 7`) and Taiwan
  (`GetLeapMonth(114) == 7`) native year numbering. Mid-Autumn/Chuseok (month 8, day 15) in 2025
  requires the shifted month 9 to land on 2025-10-06, matching the real-world observed date —
  confirms the leap-month-shift caveat applies identically to these two calendars.
- 2026 reference dates used by the country holiday tests: Seollal/Lunar New Year 2026-02-17,
  Buddha's Birthday 2026-05-24, Chuseok 2026-09-25, Dragon Boat Festival 2026-06-19, Mid-Autumn
  Festival 2026-09-25 (2026 has no leap month in either calendar, so unshifted month numbers
  apply).
- WebSearch cross-checked these against independent holiday-calendar sources for 2026: Seollal
  Feb 16-18 2026 (day-of Feb 17 confirmed), Chuseok Sep 24-26 2026 (day-of Sep 25 confirmed),
  Taiwan Dragon Boat Jun 19 2026 (confirmed), Taiwan Mid-Autumn Sep 25 2026 (confirmed).
- Taiwan's Tomb Sweeping Day is **not** computed from the floating Qingming solar term (which
  ranges Apr 4-6) — Taiwanese law fixes the statutory public holiday to **April 5** every year
  (a fixed civil date, unrelated to the astronomical solar term), per multiple independent
  holiday-calendar sources. Implemented as a plain fixed `DateOnly(year, 4, 5)`.
- Taiwan's Lunar New Year statutory holiday is Chinese New Year's Eve (last day of the 12th lunar
  month, i.e. the day before month 1 day 1) plus the first three days of month 1 (days 1-3) — a
  4-day core span. (Some years' publicized "9-day" breaks include weekend-bridging days decided
  administratively year-to-year; those are out of scope, consistent with this codebase's
  formula-only approach — only the core 4-day statutory span is implemented.)

## Global Constraints

- Leap-month shift handling: both new calculation classes' `DateInGregorianYear` already handles
  finding the correct native year via `GetYear`, but the **month number passed in** must already
  be pre-shifted for years where the target month falls after a leap month. Each country's
  `NationalHolidays.cs` computes this itself with a small private helper using
  `new {Korean,Taiwan}LunisolarCalendar().GetLeapMonth(nativeYear)`, where `nativeYear =
  calendar.GetYear(new DateTime(year, 7, 1))` (a mid-year date, safely inside the lunar year that
  started at that Gregorian year's Lunar New Year).
- New `HolidayNames` enum members are added once, up front (Task 3), then reused by both country
  tasks. `LunarNewYearsEve` and `LunarNewYearsDay` are shared between South Korea and Taiwan (same
  concept); `DayAfterLunarNewYear` is Korea-specific (its 3-day span), `SecondDayOfLunarNewYear`/
  `ThirdDayOfLunarNewYear` are Taiwan-specific (its 4-day span). `ChildrensDay`, `LiberationDay`,
  `MemorialDay` are reused from existing entries with broadened `//` comments.
- Countries live at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs` (new `Asia` folder —
  doesn't exist yet); tests at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs` (new `Asia` test
  folder).
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost
  is unavailable in this sandbox).
- After both countries are done, update both spec docs per Task 8.

---

## Reference: full holiday list per country (year 2026 reference dates)

### South Korea — 15 holidays
| Date (2026) | HolidayNames member | Basis |
|---|---|---|
| Jan 1 | `NewYearsDay` | fixed |
| Feb 16 | `LunarNewYearsEve` | lunar month1 day1 - 1 |
| Feb 17 | `LunarNewYearsDay` | lunar month1 day1 |
| Feb 18 | `DayAfterLunarNewYear` (new) | lunar month1 day1 + 1 |
| Mar 1 | `IndependenceMovementDayOfKorea` (new) | fixed |
| May 5 | `ChildrensDay` (reuse) | fixed |
| May 24 | `BuddhasBirthday` (new) | lunar month4 day8 |
| Jun 6 | `MemorialDay` (reuse) | fixed |
| Aug 15 | `LiberationDay` (reuse) | fixed |
| Sep 24 | `ChuseokEve` (new) | lunar month8 day15 - 1 |
| Sep 25 | `Chuseok` (new) | lunar month8 day15 |
| Sep 26 | `DayAfterChuseok` (new) | lunar month8 day15 + 1 |
| Oct 3 | `NationalFoundationDayOfKorea` (new) | fixed |
| Oct 9 | `HangeulDay` (new) | fixed |
| Dec 25 | `ChristmasDay` (reuse) | fixed |

### Taiwan — 12 holidays
| Date (2026) | HolidayNames member | Basis |
|---|---|---|
| Jan 1 | `NewYearsDay` | fixed |
| Feb 28 | `PeaceMemorialDayOfTaiwan` (new) | fixed |
| lunar m1d1 - 1 | `LunarNewYearsEve` | lunar |
| lunar m1d1 | `LunarNewYearsDay` | lunar |
| lunar m1d1 + 1 | `SecondDayOfLunarNewYear` (new) | lunar |
| lunar m1d1 + 2 | `ThirdDayOfLunarNewYear` (new) | lunar |
| Apr 4 | `ChildrensDay` (reuse) | fixed |
| Apr 5 | `TombSweepingDay` (new) | fixed (statutory, not the floating Qingming solar term) |
| May 1 | `LabourDay` (reuse) | fixed |
| Jun 19 | `DragonBoatFestival` (new) | lunar month5 day5 |
| Sep 25 | `MidAutumnFestival` (new) | lunar month8 day15 |
| Oct 10 | `NationalDayOfTaiwan` (new) | fixed |

---

## Task 1: Add KoreanLunisolarCalendarCalculation

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/KoreanLunisolarCalendarCalculation.cs`
- Create: `TemporaLinq/TemporaLinq.Test/KoreanLunisolarCalendarCalculationTest.cs`

- [ ] **Step 1: Write failing tests first**

Create the test file with facts asserting (values from the empirical verification above):
- `DateInGregorianYear(2024, 1, 1)` == `2024-02-10` (Seollal)
- Leap month shift: `new KoreanLunisolarCalendar().GetLeapMonth(2025)` == `7`, then
  `DateInGregorianYear(2025, 9, 15)` == `2025-10-06` (shifted Chuseok)
- Ordinary year: `GetLeapMonth(2024)` == `0`, then `DateInGregorianYear(2024, 8, 15)` ==
  `2024-09-17` (unshifted Chuseok)
- `DateInGregorianYear(2024, 4, 8)` == `2024-05-15` (Buddha's Birthday)

Run `cd TemporaLinq && dotnet test --framework net10.0 --filter KoreanLunisolarCalendarCalculationTest` — expect compile failure (class doesn't exist yet), confirming red.

- [ ] **Step 2: Implement the calculation class**

Mirror `ChineseLunisolarCalendarCalculation.cs` exactly (same structure, same XML doc shape),
swapping in `KoreanLunisolarCalendar` and adjusting doc text to say "Korean".

- [ ] **Step 3: Verify green**

Run: `cd TemporaLinq && dotnet build && dotnet test --framework net10.0 --filter KoreanLunisolarCalendarCalculationTest`
Expected: build succeeds, all facts pass.

- [ ] **Step 4: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/KoreanLunisolarCalendarCalculation.cs TemporaLinq/TemporaLinq.Test/KoreanLunisolarCalendarCalculationTest.cs
git commit -m "feat: add KoreanLunisolarCalendarCalculation"
```

## Task 2: Add TaiwanLunisolarCalendarCalculation

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/TaiwanLunisolarCalendarCalculation.cs`
- Create: `TemporaLinq/TemporaLinq.Test/TaiwanLunisolarCalendarCalculationTest.cs`

- [ ] **Step 1: Write failing tests first**

Create the test file with facts asserting (values from the empirical verification above):
- `DateInGregorianYear(2024, 1, 1)` == `2024-02-10` (Lunar New Year)
- ROC era note: `new TaiwanLunisolarCalendar().GetYear(new DateTime(2024, 2, 10))` == `113`
- Leap month shift: `GetLeapMonth(114)` == `7` (native ROC year for Gregorian 2025), then
  `DateInGregorianYear(2025, 9, 15)` == `2025-10-06`
- Ordinary year: `GetLeapMonth(113)` == `0`, then `DateInGregorianYear(2024, 8, 15)` ==
  `2024-09-17`
- `DateInGregorianYear(2026, 5, 5)` == `2026-06-19` (Dragon Boat Festival)

Run `cd TemporaLinq && dotnet test --framework net10.0 --filter TaiwanLunisolarCalendarCalculationTest` — expect compile failure, confirming red.

- [ ] **Step 2: Implement the calculation class**

Mirror `ChineseLunisolarCalendarCalculation.cs`, swapping in `TaiwanLunisolarCalendar`, and add
the extra doc-comment sentence noting the ROC/Minguo era internal year numbering (see the class
listing above under "Empirical verification").

- [ ] **Step 3: Verify green**

Run: `cd TemporaLinq && dotnet build && dotnet test --framework net10.0 --filter TaiwanLunisolarCalendarCalculationTest`
Expected: build succeeds, all facts pass.

- [ ] **Step 4: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/TaiwanLunisolarCalendarCalculation.cs TemporaLinq/TemporaLinq.Test/TaiwanLunisolarCalendarCalculationTest.cs
git commit -m "feat: add TaiwanLunisolarCalendarCalculation"
```

## Task 3: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

- [ ] **Step 1: Edit the enum**

Insert these new members in alphabetical order (read the full current file first to find each
exact slot):

```
    BuddhasBirthday, // South Korea
    Chuseok, // South Korea
    ChuseokEve, // South Korea
    DayAfterChuseok, // South Korea
    DayAfterLunarNewYear, // South Korea
    DragonBoatFestival, // Taiwan
    HangeulDay, // South Korea
    IndependenceMovementDayOfKorea, // South Korea
    LunarNewYearsDay, // South Korea, Taiwan
    LunarNewYearsEve, // South Korea, Taiwan
    MidAutumnFestival, // Taiwan
    NationalDayOfTaiwan, // Taiwan
    NationalFoundationDayOfKorea, // South Korea
    PeaceMemorialDayOfTaiwan, // Taiwan
    SecondDayOfLunarNewYear, // Taiwan
    ThirdDayOfLunarNewYear, // Taiwan
    TombSweepingDay, // Taiwan
```

Also broaden the `//` comments on these **existing** members:

```
    ChildrensDay, // Romania, South Korea, Taiwan
    LiberationDay, // Italy, Netherlands, South Korea
    MemorialDay, // USA, Moldova, South Korea
```

- [ ] **Step 2: Build to verify the enum compiles**

Run: `cd TemporaLinq && dotnet build`
Expected: `Build succeeded.` with 0 errors.

- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for South Korea and Taiwan"
```

## Task 4: Implement South Korea national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/SouthKorea/NationalHolidays.cs`
- Create: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/SouthKoreaTest.cs`

- [ ] **Step 1: Write failing test first**

Create `SouthKoreaTest.cs` following the `HungaryTest.cs` pattern:
- `GetHolidays_For2026_ReturnsAllHolidays`: expect `HaveCount(15)`.
- `GetHolidays_ContainsFixedHolidays`: assert all 2026 fixed dates from the reference table above.
- `GetHolidays_ContainsLunarHolidays`: assert Seollal (Feb 16/17/18), Buddha's Birthday (May 24),
  Chuseok (Sep 24/25/26) for 2026.

Run `cd TemporaLinq && dotnet test --framework net10.0 --filter SouthKoreaTest` — expect compile
failure (namespace/class doesn't exist), confirming red.

- [ ] **Step 2: Implement `NationalHolidays.cs`**

Follow the `Europe/Hungary/NationalHolidays.cs` shape exactly: `namespace
TemporaLinq.Holidays.Asia.SouthKorea;`, `using static TemporaLinq.Holidays.HolidayNames;`, a
`[Cache]`-memoized `GetHolidaysFor(int year)` returning `ImmutableList<Holiday>`.

For the lunar dates, add a small private helper to compute the leap-month-shifted month number:

```csharp
private static int ShiftedLunarMonth(int year, int lunarMonth)
{
    var calendar = new System.Globalization.KoreanLunisolarCalendar();
    var nativeYear = calendar.GetYear(new DateTime(year, 7, 1));
    var leapMonth = calendar.GetLeapMonth(nativeYear);
    return leapMonth > 0 && leapMonth <= lunarMonth ? lunarMonth + 1 : lunarMonth;
}
```

Then:
```csharp
var seollal = KoreanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 1), 1);
var buddhasBirthday = KoreanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 4), 8);
var chuseok = KoreanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 8), 15);
```

Full holiday list per the reference table above (fixed dates + `seollal.AddDays(-1)`, `seollal`,
`seollal.AddDays(1)` + `buddhasBirthday` + `chuseok.AddDays(-1)`, `chuseok`, `chuseok.AddDays(1)`).

- [ ] **Step 3: Verify green**

Run: `cd TemporaLinq && dotnet build && dotnet test --framework net10.0 --filter SouthKoreaTest`
Expected: build succeeds, all facts pass.

- [ ] **Step 4: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/SouthKorea/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/SouthKoreaTest.cs
git commit -m "feat: add South Korea national holidays"
```

## Task 5: Implement Taiwan national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Taiwan/NationalHolidays.cs`
- Create: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/TaiwanTest.cs`

- [ ] **Step 1: Write failing test first**

Create `TaiwanTest.cs` following the `HungaryTest.cs` pattern:
- `GetHolidays_For2026_ReturnsAllHolidays`: expect `HaveCount(12)`.
- `GetHolidays_ContainsFixedHolidays`: assert all 2026 fixed dates from the reference table above
  (including the fixed Apr 5 Tomb Sweeping Day).
- `GetHolidays_ContainsLunarHolidays`: assert Lunar New Year span, Dragon Boat Festival (Jun 19),
  Mid-Autumn Festival (Sep 25) for 2026.

Run `cd TemporaLinq && dotnet test --framework net10.0 --filter TaiwanTest` — expect compile
failure, confirming red.

- [ ] **Step 2: Implement `NationalHolidays.cs`**

Same shape as South Korea's, `namespace TemporaLinq.Holidays.Asia.Taiwan;`, using
`TaiwanLunisolarCalendarCalculation` and a `ShiftedLunarMonth` helper backed by
`TaiwanLunisolarCalendar` (same formula — `GetYear`/`GetLeapMonth` already handle the ROC era
transparently).

```csharp
var lunarNewYear = TaiwanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 1), 1);
var dragonBoat = TaiwanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 5), 5);
var midAutumn = TaiwanLunisolarCalendarCalculation.DateInGregorianYear(year, ShiftedLunarMonth(year, 8), 15);
```

Full holiday list: fixed dates (`NewYearsDay`, `PeaceMemorialDayOfTaiwan` Feb 28, `ChildrensDay`
Apr 4, `TombSweepingDay` Apr 5 fixed, `LabourDay` May 1, `NationalDayOfTaiwan` Oct 10) +
`lunarNewYear.AddDays(-1)`/`lunarNewYear`/`lunarNewYear.AddDays(1)`/`lunarNewYear.AddDays(2)` +
`dragonBoat` + `midAutumn`.

- [ ] **Step 3: Verify green**

Run: `cd TemporaLinq && dotnet build && dotnet test --framework net10.0 --filter TaiwanTest`
Expected: build succeeds, all facts pass.

- [ ] **Step 4: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Taiwan/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/TaiwanTest.cs
git commit -m "feat: add Taiwan national holidays"
```

## Task 6: Update worldwide holidays design checklist

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] Mark South Korea done with ✅ in the Tier AS1 line and Taiwan done with ✅ in the Tier AS3
  line, matching the checklist's existing ✅ convention.
- [ ] Commit: `git commit -m "docs: mark South Korea and Taiwan done in worldwide holidays checklist"`

## Task 7: Update calendar calculation mechanisms design status

**Files:**
- Modify: `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`

- [ ] Update the "Status (2026-08-26)" line to note `KoreanLunisolarCalendarCalculation` and
  `TaiwanLunisolarCalendarCalculation` are now also implemented and tested.
- [ ] Commit: `git commit -m "docs: mark Korean and Taiwanese calculation classes implemented"`

## Task 8: Full suite verification

- [ ] Run `cd TemporaLinq && dotnet test --framework net10.0` — confirm 0 failures across the
  entire suite (not just the new tests).
- [ ] Report final test count, branch name, and worktree path back to the coordinating session.
  Do NOT merge to main; do NOT delete the worktree.
