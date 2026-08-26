# Thailand & Myanmar Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for Thailand and Myanmar to `TemporaLinq.Holidays`, using the newly-merged `TemporaLinq.Astronomy.SoutheastAsianBuddhistCalendar` for the three shared Theravada Buddhist holy days (Makha Bucha / Tabaung Full Moon Day, Visakha Bucha / Kason Full Moon Day, Asalha Bucha / Waso Full Moon Day), plus fixed civil holidays verified via WebSearch.

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs`, computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`), calling `SoutheastAsianBuddhistCalendar.MakhaBuchaDate/VisakhaBuchaDate/AsalhaBuchaDate(year)` for the movable Buddhist holy days. New `HolidayNames` enum members are added once, up front, then reused by both country tasks. Each country also gets a test file at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs` following the existing `TurkeyTest`/`IndiaTest` pattern, asserting exact 2026 dates.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, `docs/superpowers/specs/2026-08-26-southeast-asian-buddhist-calendar-design.md`

## Research findings (WebSearch, 2026-08-26)

### Thailand — official 2026 civil calendar (Bank of Thailand / Cabinet resolution)

Excludes one-off Cabinet-approved bonus holidays (e.g. Jan 2, 2026's ad-hoc extra day) since those are not a stable per-year formula — consistent with how this codebase already omits similar one-off bonus days for other countries.

| Date (2026) | Holiday | HolidayNames member |
|---|---|---|
| Jan 1 | New Year's Day | `NewYearsDay` |
| Mar 3 (movable) | Makha Bucha | `MakhaBuchaDay` |
| Apr 6 | Chakri Memorial Day | `ChakriMemorialDay` |
| Apr 13–15 | Songkran (Thai New Year) | `SongkranDay` (3 days) |
| May 1 | Labour Day | `LabourDay` (reuse) |
| May 4 | Coronation Day (King Vajiralongkorn) | `CoronationDayOfThailand` |
| May 31 (movable) | Visakha Bucha | `VisakhaBuchaDay` |
| Jun 3 | Queen Suthida's Birthday | `QueensBirthdayOfThailand` |
| Jul 28 | King Vajiralongkorn's Birthday | `KingsBirthdayOfThailand` |
| Jul 29 (movable) | Asalha Bucha | `AsalhaBuchaDay` |
| Jul 30 (movable, Asalha Bucha + 1) | Khao Phansa (Buddhist Lent begins) | `KhaoPhansaDay` |
| Aug 12 | Queen Sirikit the Queen Mother's Birthday / Mother's Day | `MothersDayOfThailand` |
| Oct 13 | King Bhumibol Memorial Day | `KingBhumibolMemorialDay` |
| Oct 23 | Chulalongkorn Day | `ChulalongkornDay` |
| Dec 5 | King Bhumibol's Birthday / National Day / Father's Day | `NationalDayOfThailand` |
| Dec 10 | Constitution Day | `ConstitutionDayOfThailand` |
| Dec 31 | New Year's Eve | `NewYearsEve` (reuse) |

**Khao Phansa verification:** confirmed via WebSearch (officeholidays.com, thailandnow.in.th, expatsinbangkok.com) that Khao Phansa (the day after Asalha Bucha, start of Buddhist Lent) is an actual government-sector public holiday date each year in Thailand's official calendar — not merely an unofficial long-weekend practice — though banks and much of the private sector remain open. It is included here as a government holiday, matching the same "government-sector-only" documentation style already used elsewhere in this codebase (e.g. Labour Day's private-sector note).

19 holiday instances total for 2026 (17 line items, Songkran contributing 3 days).

### Myanmar — official 2026 civil + Buddhist calendar

The Southeast Asian Buddhist lunisolar calendar underlying `SoutheastAsianBuddhistCalendar` is the same physical calendar Myanmar uses, just with different traditional month names anchored to Burmese usage: Tabaung's full moon is the same event as Makha Bucha, Kason's full moon the same as Visakha Bucha (Vesak/Buddha's Birthday), and Waso's full moon the same as Asalha Bucha (Dhamma Day, start of Buddhist Lent/Vassa) — so the existing `MakhaBuchaDate`/`VisakhaBuchaDate`/`AsalhaBuchaDate` methods are reused directly, just exposed under Myanmar's local `HolidayNames` member names. Later Burmese lunar months (Thadingyut, Tazaungmone — used for National Day, which falls 10 days after Tazaungmone's full moon) are out of scope, since `SoutheastAsianBuddhistCalendar` only computes months 3/6/8.

| Date (2026) | Holiday | HolidayNames member |
|---|---|---|
| Jan 1 | New Year's Day | `NewYearsDay` (reuse) |
| Jan 4 | Independence Day | `IndependenceDay` (reuse) |
| Feb 12 | Union Day | `UnionDayOfMyanmar` |
| Mar 2 | Peasants' Day | `PeasantsDay` |
| Mar 3 (movable) | Tabaung Full Moon Day | `TabaungFullMoonDay` |
| Mar 27 | Armed Forces Day | `ArmedForcesDayOfMyanmar` |
| Apr 13–16 | Thingyan (Myanmar New Year water festival) | `ThingyanDay` (4 days) |
| May 1 | Labour Day | `LabourDay` (reuse) |
| May 31 (movable) | Kason Full Moon Day (Buddha's Birthday / Vesak) | `KasonFullMoonDay` |
| Jul 19 | Martyrs' Day | `MartyrsDayOfMyanmar` |
| Jul 29 (movable) | Waso Full Moon Day (Dhamma Day) | `WasoFullMoonDay` |
| Dec 25 | Christmas Day | `ChristmasDay` (reuse) |

**Thingyan verification:** WebSearch confirmed the traditional/statutory 4-day span (Apr 13 Eve, Apr 14–15 water-festival days, Apr 16 Myanmar New Year's Day) is fixed by law each year, distinct from occasional government decrees adding extra bonus days around it (e.g. a 9-day span announced for 2026) — the latter is a one-off Cabinet-style addition, out of scope for the same reason Thailand's Jan 2 bonus day is out of scope.

16 holiday instances total for 2026 (12 line items, Thingyan contributing 4 days).

## Global Constraints

- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox).
- Reuse existing `HolidayNames` enum members wherever the concept matches (broadening the `//` comment to list the additional country), per the established convention. Only add new enum members for genuinely new concepts.
- Insert new enum members in alphabetical order, matching the existing convention.
- Countries live at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs`; tests at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs`.
- After both countries are done, update the checklist in `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md` marking Thailand and Myanmar done, removing their 🔴 flags.

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

- [ ] **Step 1: Edit the enum**

Insert these new members in alphabetical order:

```
    ArmedForcesDayOfMyanmar, // Myanmar
    AsalhaBuchaDay, // Thailand
    ChakriMemorialDay, // Thailand
    ChulalongkornDay, // Thailand
    ConstitutionDayOfThailand, // Thailand
    CoronationDayOfThailand, // Thailand
    KasonFullMoonDay, // Myanmar (Buddha's Birthday / Vesak)
    KhaoPhansaDay, // Thailand
    KingBhumibolMemorialDay, // Thailand
    KingsBirthdayOfThailand, // Thailand
    MakhaBuchaDay, // Thailand
    MartyrsDayOfMyanmar, // Myanmar
    MothersDayOfThailand, // Thailand
    NationalDayOfThailand, // Thailand
    PeasantsDay, // Myanmar
    QueensBirthdayOfThailand, // Thailand
    SongkranDay, // Thailand
    TabaungFullMoonDay, // Myanmar (Makha Bucha equivalent)
    ThingyanDay, // Myanmar
    UnionDayOfMyanmar, // Myanmar
    VisakhaBuchaDay, // Thailand
    WasoFullMoonDay, // Myanmar (Asalha Bucha equivalent)
```

Broaden comments on existing members:

```
    ChristmasDay, // also Egypt (Coptic Christmas), Ethiopia (Genna), Myanmar
    IndependenceDay, // ..., Myanmar
    LabourDay, // also Nigeria (Workers' Day), Egypt, Morocco, Sri Lanka, Thailand, Myanmar
    NewYearsDay, // also Nigeria, Egypt, Morocco, Thailand, Myanmar
    NewYearsEve, // Latvia, San Marino, Thailand
```

- [ ] **Step 2: Build to verify the enum compiles**

Run: `cd TemporaLinq && dotnet build`

- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for Thailand and Myanmar"
```

---

## Task 2: Add Thailand national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Thailand/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/ThailandTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `SoutheastAsianBuddhistCalendar.MakhaBuchaDate/VisakhaBuchaDate/AsalhaBuchaDate(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test** — asserts `HaveCount(19)` for 2026 and exact dates for every line item above (using `SoutheastAsianBuddhistCalendar` for the movable ones and comparing to the known 2026 values Mar 3 / May 31 / Jul 29 directly, per the task brief — day-level tolerance is not needed since the calendar mechanism is already verified).

- [ ] **Step 2: Run test to verify it fails** (compile error — namespace doesn't exist yet)

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter ThailandTest`

- [ ] **Step 3: Write the implementation** per the table above, including Khao Phansa as `AsalhaBucha.AddDays(1)`.

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter ThailandTest`

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Thailand/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/ThailandTest.cs
git commit -m "feat: add Thailand national holidays"
```

---

## Task 3: Add Myanmar national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Myanmar/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/MyanmarTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `SoutheastAsianBuddhistCalendar.MakhaBuchaDate/VisakhaBuchaDate/AsalhaBuchaDate(int) -> DateOnly` (reused under Myanmar's local names), `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test** — asserts `HaveCount(16)` for 2026 and exact dates for every line item above.

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter MyanmarTest`

- [ ] **Step 3: Write the implementation** per the table above.

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter MyanmarTest`

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Myanmar/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/MyanmarTest.cs
git commit -m "feat: add Myanmar national holidays"
```

---

## Task 4: Update the worldwide holidays checklist and run the full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1:** In the Asia section, move Thailand from the 🔴 Tier AS3 remaining line to a "Done: ✅" line, and remove Myanmar from the Tier AS4 🔴 list, adding it to a "Done: ✅" line, matching the style of neighboring done entries.

- [ ] **Step 2:** Run: `cd TemporaLinq && dotnet test --framework net10.0` — expect 0 failures.

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Thailand and Myanmar done in worldwide holidays checklist"
```
