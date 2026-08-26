# Hijri Balkans + Turkey Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for Albania, Kosovo, and Turkey — the "Balkans + Turkey" batch newly unblocked by `HijriCalendarCalculation` — plus a scoped-down state-level-only treatment of Bosnia and Herzegovina if it turns out to be clean, following the exact pattern already used by Tier E1–E5 countries.

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record, computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`), using `EasterSundayCalculation.Christian`/`.ChristianOrthodox` for Christian movable feasts and `HijriCalendarCalculation.DatesInGregorianYear(year, hijriMonth, hijriDay)` for Eid al-Fitr (1 Shawwal = month 10, day 1) and Eid al-Adha (10 Dhu al-Hijjah = month 12, day 10), exactly as documented in `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`. Because `DatesInGregorianYear` returns a sequence (usually 1, occasionally 2, dates per Gregorian year), each Eid is added via `SelectMany`/`.Select(...)` over that sequence rather than a single `new Holiday(...)` line. Albania and Kosovo live at `TemporaLinq.Holidays/Europe/<Country>/NationalHolidays.cs` (tests at `TemporaLinq.Test/Holidays/Europe/<Country>Test.cs`). Turkey is transcontinental but conventionally covered by Asia-focused holiday tiers per the worldwide-holidays spec, so it gets a new `TemporaLinq.Holidays/Asia/Turkey/NationalHolidays.cs` and `TemporaLinq.Test/Holidays/Asia/TurkeyTest.cs`, establishing the `Asia` folder convention for future Asian-tier work.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`

## Research findings (verified via web search, since this is user-facing data)

- **Albania** (mixed Muslim/Orthodox/Catholic/secular state; 15 official non-working dates/year): New Year's Day (Jan 1–2), Summer Day (Mar 14, fixed), Nevruz Day (Mar 22, fixed), Catholic Easter Sunday (movable), Eid al-Fitr (1 Shawwal), International Labour Day (May 1), Orthodox Easter Sunday (movable), Eid al-Adha (10 Dhu al-Hijjah), Mother Teresa Day (Sep 5, fixed), Alphabet Day (Nov 22, fixed — Congress of Manastir), Independence Day (Nov 28, fixed), Liberation Day (Nov 29, fixed), National Youth Day (Dec 8, fixed), Christmas Day (Dec 25, fixed). Both Eids and both Easters are single-day holidays under Albanian law (no Monday-after or multi-day extension).
- **Kosovo** (majority Muslim; 11 named holidays under Law No. 03/L-064, some spanning 2 days): New Year's Day (Jan 1–2), Orthodox Christmas (Jan 7 — recognized for the Serb minority), Independence Day (Feb 17), Constitution Day (Apr 9), International Labour Day (May 1), Europe Day (May 9), Eid al-Fitr (1 Shawwal), Eid al-Adha (10 Dhu al-Hijjah), Catholic Easter Sunday (movable), Orthodox Easter Sunday (movable), Catholic Christmas (Dec 25). Both Eids and both Easters are single-day.
- **Turkey**: fixed civil holidays — New Year's Day (Jan 1), National Sovereignty and Children's Day (Apr 23), Labour and Solidarity Day (May 1), Commemoration of Atatürk, Youth and Sports Day (May 19), Democracy and National Unity Day (Jul 15), Victory Day (Aug 30), Republic Day (Oct 29). Ramazan Bayramı (Eid al-Fitr) is a 3-full-day holiday (1–3 Shawwal); Kurban Bayramı (Eid al-Adha) is a 4-full-day holiday (10–13 Dhu al-Hijjah). Turkish law (2429 sayılı Kanun) additionally grants a half-day "arife" (eve) before each — starting at 13:00 the prior day — but since `Holiday` has day granularity only (no time-of-day concept), the half-day eve is out of scope; only the full official days are modeled. This is documented in the XML doc comment.
- **Bosnia and Herzegovina**: confirmed via multiple sources that the *only* holidays genuinely observed nationwide by BiH state institutions (as opposed to entity law — Federation of BiH vs. Republika Srpska, which diverge on everything else including which religious holidays apply and to whom) are New Year's Day (Jan 1–2) and Labour Day (May 1–2). Attempts to pass a unified state-level holiday law covering religious holidays (including the Eids) have repeatedly stalled in the Parliamentary Assembly specifically *because* entities disagree on which additional dates qualify as national. This means, contrary to this plan's initial assumption, the Eid holidays are **not** part of the genuinely state-wide list — they are RS-law/community-specific (excused-absence) provisions, not BiH-wide non-working state holidays. The clean, accurate state-level scope is therefore 4 fixed dates only, with no Hijri calculation involved for Bosnia specifically. This is still implemented (it's small but accurate), rather than skipped, since it is not fragmented or ambiguous at the state level — see Task 5.

## Global Constraints

- New `HolidayNames` enum members are added once up front (Task 1), then reused by every country task.
- `EidAlFitr` and `EidAlAdha` are computed via `HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)` and `DatesInGregorianYear(year, 12, 10)` respectively for the single-day countries (Albania, Kosovo); Turkey additionally adds the following 2 (Fitr) / 3 (Adha) days by mapping each returned date through `.AddDays(1)`/`.AddDays(2)`/`.AddDays(3)`.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox).
- Tests assert against year 2026 (matching the codebase convention) and additionally verify the Hijri-derived dates by calling `HijriCalendarCalculation.DatesInGregorianYear` directly in the test, mirroring how existing tests call `EasterSundayCalculation` directly rather than hardcoding movable-feast dates.
- After all countries are done, update the checklist in `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, matching the exact style of prior tiers.

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

- [ ] **Step 1: Edit the enum**

Insert these new members in alphabetical order:

```
    AlphabetDay, // Albania
    ConstitutionDayOfKosovo, // Kosovo
    DemocracyAndNationalUnityDay, // Turkey
    EidAlAdha, // Albania, Kosovo, Turkey
    EidAlFitr, // Albania, Kosovo, Turkey
    EuropeDay, // Kosovo
    MotherTeresaDay, // Albania
    NationalSovereigntyAndChildrensDay, // Turkey
    NationalYouthDay, // Albania
    NevruzDay, // Albania
    SummerDay, // Albania
    YouthAndSportsDay, // Turkey
```

Broaden these existing comments:

```
    IndependenceDay, // USA, Ukraine, Finland, Bulgaria, Estonia, Iceland, Malta, Cyprus, Moldova, Montenegro, North Macedonia, Albania, Kosovo
    LiberationDay, // Italy, Netherlands, Albania
    RepublicDay, // Italy, Portugal, Malta, Turkey
    VictoryDay, // France, Ukraine, Czech Republic, Slovakia, Estonia, Moldova, Turkey
```

- [ ] **Step 2: Build to verify the enum compiles**

Run: `cd TemporaLinq && dotnet build`
Expected: `Build succeeded.` with 0 errors.

- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for Albania, Kosovo, Turkey, Bosnia"
```

---

## Task 2: Add Albania national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Albania/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/AlbaniaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian`/`.ChristianOrthodox`, `HijriCalendarCalculation.DatesInGregorianYear`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test** asserting `GetHolidays_For2026_ReturnsAllHolidays` has count 15, `GetHolidays_ContainsFixedHolidays` for each fixed date, `GetHolidays_ContainsMovableFeasts` cross-checking Catholic/Orthodox Easter via `EasterSundayCalculation`, and `GetHolidays_ContainsHijriHolidays` cross-checking Eid al-Fitr/al-Adha via `HijriCalendarCalculation.DatesInGregorianYear(2026, 10, 1)` / `(2026, 12, 10)`.

- [ ] **Step 2: Run test to verify it fails** — `cd TemporaLinq && dotnet test --framework net10.0 --filter AlbaniaTest` — Expected: FAIL (namespace does not exist).

- [ ] **Step 3: Write the implementation** at `Europe/Albania/NationalHolidays.cs`:

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Albania;

/// <summary>
/// Provides Albanian national public holidays. Albania officially observes major
/// holidays of all its main religious communities (Muslim, Orthodox Christian,
/// Catholic) as state holidays, alongside secular/civil ones. Eid al-Fitr and
/// Eid al-Adha are computed from the Hijri calendar via
/// <see cref="HijriCalendarCalculation"/> — a deterministic approximation that can
/// differ by +/-1-2 days from the real-world moon-sighting-confirmed date.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var catholicEaster = EasterSundayCalculation.Christian.ForYear(year);
        var orthodoxEaster = EasterSundayCalculation.ChristianOrthodox.ForYear(year);

        var holidays = new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 1, 2), NewYearsDay),
                new(new DateOnly(year, 3, 14), SummerDay),
                new(new DateOnly(year, 3, 22), NevruzDay),
                new(catholicEaster, EasterSunday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(orthodoxEaster, EasterSunday),
                new(new DateOnly(year, 9, 5), MotherTeresaDay),
                new(new DateOnly(year, 11, 22), AlphabetDay),
                new(new DateOnly(year, 11, 28), IndependenceDay),
                new(new DateOnly(year, 11, 29), LiberationDay),
                new(new DateOnly(year, 12, 8), NationalYouthDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)
            .Select(date => new Holiday(date, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10)
            .Select(date => new Holiday(date, EidAlAdha)));

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes** — Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Albania TemporaLinq/TemporaLinq.Test/Holidays/Europe/AlbaniaTest.cs
git commit -m "feat: add Albania national holidays"
```

---

## Task 3: Add Kosovo national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Kosovo/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/KosovoTest.cs`

Same shape as Task 2. Fixed dates: Jan 1 (`NewYearsDay`), Jan 2 (`NewYearsDay`), Jan 7 (`ChristmasDay` — Orthodox Nativity, Serb minority), Feb 17 (`IndependenceDay`), Apr 9 (`ConstitutionDayOfKosovo`), May 1 (`LabourDay`), May 9 (`EuropeDay`), Dec 25 (`ChristmasDay` — Catholic Nativity). Movable: Catholic Easter and Orthodox Easter (both `EasterSunday`, via `EasterSundayCalculation.Christian`/`.ChristianOrthodox`). Hijri: `EidAlFitr` (10,1) and `EidAlAdha` (12,10), same `HijriCalendarCalculation` pattern as Task 2.

- [ ] **Step 1: Write the failing test** (count 12 for 2026: 8 fixed dates + 2 Easters + 2 Eid dates, assuming no double-occurrence year).
- [ ] **Step 2: Run test to verify it fails.**
- [ ] **Step 3: Write the implementation** mirroring Task 2's structure.
- [ ] **Step 4: Run test to verify it passes.**
- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Kosovo TemporaLinq/TemporaLinq.Test/Holidays/Europe/KosovoTest.cs
git commit -m "feat: add Kosovo national holidays"
```

---

## Task 4: Add Turkey national holidays (establishes the Asia folder convention)

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Turkey/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/TurkeyTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `HijriCalendarCalculation.DatesInGregorianYear`, `HolidayNames` members from Task 1. No Easter calculation (Turkey's official holidays are entirely civil + Hijri).

- [ ] **Step 1: Write the failing test.** Fixed: Jan 1 (`NewYearsDay`), Apr 23 (`NationalSovereigntyAndChildrensDay`), May 1 (`LabourDay`), May 19 (`YouthAndSportsDay`), Jul 15 (`DemocracyAndNationalUnityDay`), Aug 30 (`VictoryDay`), Oct 29 (`RepublicDay`) = 7 fixed. Eid al-Fitr: 3 consecutive days from `HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)` (each occurrence plus the following 2 days). Eid al-Adha: 4 consecutive days from `DatesInGregorianYear(year, 12, 10)` (each occurrence plus the following 3 days). For 2026 expect 7 + 3 + 4 = 14 total (verify against the actual single-occurrence count returned for 2026 — if the calculation yields a double-occurrence for either Eid in 2026, adjust the expected count accordingly by computing it from `HijriCalendarCalculation` in the test rather than hardcoding).

- [ ] **Step 2: Run test to verify it fails** — note this also creates the new `Asia` namespace/folder for the first time.

- [ ] **Step 3: Write the implementation:**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Turkey;

/// <summary>
/// Provides Turkish national public holidays. Ramazan Bayramı (Eid al-Fitr) and
/// Kurban Bayramı (Eid al-Adha) are computed from the Hijri calendar via
/// <see cref="HijriCalendarCalculation"/> — a deterministic approximation that can
/// differ by +/-1-2 days from the real-world moon-sighting-confirmed date. Turkish
/// law additionally grants a half-day "arife" (eve) before each bayram starting at
/// 13:00 the previous day; since <see cref="Holiday"/> has day granularity only,
/// that half-day is out of scope and only the full official days are modeled here
/// (3 full days for Ramazan Bayramı, 4 full days for Kurban Bayramı).
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var holidays = new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 4, 23), NationalSovereigntyAndChildrensDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 19), YouthAndSportsDay),
                new(new DateOnly(year, 7, 15), DemocracyAndNationalUnityDay),
                new(new DateOnly(year, 8, 30), VictoryDay),
                new(new DateOnly(year, 10, 29), RepublicDay),
            };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)
            .SelectMany(start => new[] { start, start.AddDays(1), start.AddDays(2) })
            .Select(date => new Holiday(date, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10)
            .SelectMany(start => new[] { start, start.AddDays(1), start.AddDays(2), start.AddDays(3) })
            .Select(date => new Holiday(date, EidAlAdha)));

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes.**
- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Turkey TemporaLinq/TemporaLinq.Test/Holidays/Asia/TurkeyTest.cs
git commit -m "feat: add Turkey national holidays"
```

---

## Task 5: Add Bosnia and Herzegovina state-level national holidays (or document why skipped)

**Files:**
- Create (if proceeding): `TemporaLinq/TemporaLinq.Holidays/Europe/BosniaAndHerzegovina/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/BosniaAndHerzegovinaTest.cs`

Per the research findings above, the genuinely BiH-wide (state-institution) holiday list is small and clean: New Year's Day (Jan 1–2) and Labour Day (May 1–2) — 4 dates, no movable feast, no Hijri calculation. This is implemented as an ordinary fixed-date `NationalHolidays` record (same shape as Montenegro's in Tier E5) with an XML doc comment explaining the state/entity-fragmentation scoping decision and citing that entity-specific and religious-community holidays (Federation of BiH, Republika Srpska, Brčko District) are out of scope.

**If, during implementation, a source is found that materially contradicts this 4-date state-level finding** (e.g. an authoritative BiH state institution calendar listing more dates), stop and re-verify before writing code — do not guess.

- [ ] **Step 1: Write the failing test** (count 4; contains Jan 1, Jan 2, May 1, May 2, all `NewYearsDay`/`LabourDay`).
- [ ] **Step 2: Run test to verify it fails.**
- [ ] **Step 3: Write the implementation:**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.BosniaAndHerzegovina;

/// <summary>
/// Provides the state-level (BiH-wide) national public holidays of Bosnia and
/// Herzegovina — the small set of dates genuinely observed nationwide by state
/// institutions, as distinct from the much larger set of entity-specific holidays
/// (Federation of Bosnia and Herzegovina vs. Republika Srpska vs. Brčko District)
/// and community-specific religious observances (Catholic/Orthodox Christmas and
/// Easter, Eid al-Fitr, Eid al-Adha), which diverge by entity/community and are
/// out of scope here. Attempts to legislate a unified state-level holiday law
/// covering those additional dates have repeatedly stalled in the Parliamentary
/// Assembly precisely because the entities disagree on which dates qualify as
/// national — this is a genuine, ongoing political fragmentation, not an
/// oversight of this implementation.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 1, 2), NewYearsDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 2), LabourDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes.**
- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/BosniaAndHerzegovina TemporaLinq/TemporaLinq.Test/Holidays/Europe/BosniaAndHerzegovinaTest.cs
git commit -m "feat: add Bosnia and Herzegovina state-level national holidays"
```

---

## Task 6: Update the worldwide holidays checklist and run the full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1: Update the Europe section** — mark Albania, Kosovo, and Bosnia and Herzegovina ✅ done, removing them from the "Tier E4/E5 remaining" lines, matching the exact style of prior tiers.
- [ ] **Step 2: Note Turkey done under Asia** — mark Turkey ✅ in the Tier AS1 line (or add a "Done" line for it), matching style.
- [ ] **Step 3: Run the full test suite** — `cd TemporaLinq && dotnet test --framework net10.0` — Expected: all pass, 0 failures.
- [ ] **Step 4: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Albania, Kosovo, Bosnia and Herzegovina, Turkey done in worldwide holidays checklist"
```
