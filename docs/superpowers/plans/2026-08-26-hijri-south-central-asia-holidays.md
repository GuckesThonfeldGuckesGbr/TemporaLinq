# Hijri South/Central Asia Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for Pakistan, Bangladesh (Hijri-based and fixed civil holidays only), and Uzbekistan to `TemporaLinq.Holidays`, using the newly-merged `HijriCalendarCalculation.DatesInGregorianYear(int gregorianYear, int hijriMonth, int hijriDay) -> IEnumerable<DateOnly>` for the Islamic-calendar holidays, following the exact pattern already used by the Europe tiers.

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs` (new `Asia` folder), computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`). Multi-day Eid holidays are modeled as multiple consecutive `Holiday` entries sharing the same `HolidayNames` member, the same way Serbia's two-day Statehood Day is modeled today. Each Hijri (month, day) pair is looked up via `HijriCalendarCalculation.DatesInGregorianYear(year, month, day)`, and every date it returns for that year is added as a `Holiday` (almost always one date; the rare drift-double year yields two, which is correct behavior, not a bug to work around). New `HolidayNames` enum members are added once, up front, then reused by every country task. Each country also gets a test file at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs` following the existing pattern, with movable-feast assertions computed via `HijriCalendarCalculation` itself (the same way existing tests compute Easter-based assertions via `EasterSundayCalculation`) rather than hardcoded real-world dates.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`

## Global Constraints

- Hijri month numbers (via `System.Globalization.HijriCalendar`, matching `HijriCalendarCalculation`): Muharram=1, Rabi' al-awwal=3, Shawwal=10, Dhu al-Hijjah=12.
- Eid al-Fitr = 1 Shawwal; multi-day observances start there and run consecutive Hijri days (2, 3 Shawwal, ...).
- Eid al-Adha = 10 Dhu al-Hijjah; multi-day observances run 10, 11, 12 Dhu al-Hijjah.
- Ashura = 10 Muharram (Pakistan additionally observes 9 Muharram).
- Eid Milad-un-Nabi (Mawlid) = 12 Rabi' al-awwal.
- All Hijri-derived holidays carry the same +/-1/+/-2 day real-world moon-sighting caveat already documented on `HijriCalendarCalculation` — each country's doc comment references it rather than re-deriving it.
- Bangladesh's Hindu/Buddhist minority holidays (Durga Puja, Buddha Purnima) are explicitly out of scope — the implementation's doc comment says so, pending a future Hindu/Buddhist calendar calculation mechanism. Do not attempt them.
- Uzbekistan's Nowruz (Mar 21) is a **fixed Gregorian civil date** in Uzbekistan's calendar (unlike Iran's astronomically-calculated Nowruz) — implemented as a plain fixed date, not any solar-calendar calculation.
- Researched via web search against 2026 official gazettes/circulars (Pakistan Cabinet Division circular, Bangladesh Ministry of Public Administration circular, Uzbekistan's seven-holiday official list) to confirm day-counts per multi-day observance; see per-country notes below.
- Countries live at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs`; tests at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs`. The `Asia` folder does not yet exist in either project — create it.
- Reuse existing `HolidayNames` enum members wherever the concept matches (`IndependenceDay`, `LabourDay`, `ChristmasDay`, `NewYearsDay`, `InternationalWomensDay`, `VictoryDay`), broadening the `//` comment on `IndependenceDay` and `VictoryDay` to list the additional countries. Only add new enum members for genuinely new concepts.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox).
- After all three countries are done, update the checklist in `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`'s Asia section, matching the exact style used for prior tiers, and note Bangladesh's Hindu/Buddhist components remain deferred.

---

## Reference: full holiday list per country (2026)

### Pakistan — 6 fixed + 9 Hijri-derived (2026) = 15 holidays
| Date/Hijri | HolidayNames member |
|---|---|
| Feb 5 | `KashmirSolidarityDay` (new) |
| Mar 23 | `PakistanDay` (new) |
| May 1 | `LabourDay` (reuse) |
| Aug 14 | `IndependenceDay` (reuse) |
| Nov 9 | `IqbalDay` (new) |
| Dec 25 | `QuaidEAzamDay` (new — Muhammad Ali Jinnah's birthday; coincides with, and is gazetted alongside, Christmas Day) |
| 1, 2, 3 Shawwal | `EidAlFitr` x3 (new) |
| 10, 11, 12 Dhu al-Hijjah | `EidAlAdha` x3 (new) |
| 9, 10 Muharram | `AshuraDay` x2 (new) |
| 12 Rabi' al-awwal | `EidMiladUnNabi` x1 (new) |

Verified against 2026 Cabinet Division circular reporting: Eid-ul-Fitr Mar 21-23, Eid-ul-Azha May 27-29, Ashura Jun 24-25 (9th-10th Muharram), Eid Milad-un-Nabi Aug 26 — all consistent with the Hijri (month, day) pairs above computed via `HijriCalendarCalculation`.

### Bangladesh — 7 fixed + 8 Hijri-derived (2026) = 15 holidays
| Date/Hijri | HolidayNames member |
|---|---|
| Feb 21 | `LanguageMovementDay` (new — also International Mother Language Day) |
| Mar 26 | `IndependenceDay` (reuse) |
| Apr 14 | `BengaliNewYear` (new — Pohela Boishakh; fixed Gregorian civil date, not lunar) |
| May 1 | `LabourDay` (reuse) |
| Aug 15 | `NationalMourningDay` (new) |
| Dec 16 | `VictoryDay` (reuse) |
| Dec 25 | `ChristmasDay` (reuse) |
| 1, 2, 3 Shawwal | `EidAlFitr` x3 (new, shared with Pakistan) |
| 10, 11, 12 Dhu al-Hijjah | `EidAlAdha` x3 (new, shared with Pakistan) |
| 10 Muharram | `AshuraDay` x1 (new, shared with Pakistan — Bangladesh observes only the single day, unlike Pakistan's two) |
| 12 Rabi' al-awwal | `EidMiladUnNabi` x1 (new, shared with Pakistan) |

Hindu/Buddhist minority holidays (Durga Puja, Buddha Purnima) are explicitly out of scope — documented in the implementation's XML doc comment.

### Uzbekistan — 7 fixed + 6 Hijri-derived (2026) = 13 holidays
| Date/Hijri | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` (reuse) |
| Mar 8 | `InternationalWomensDay` (reuse) |
| Mar 21 | `NowruzDay` (new — fixed Gregorian civil date) |
| May 9 | `MemoryAndHonorDay` (new — Xotira va qadrlash kuni) |
| Sep 1 | `IndependenceDay` (reuse) |
| Oct 1 | `TeachersAndInstructorsDay` (new) |
| Dec 8 | `ConstitutionDayOfUzbekistan` (new) |
| 1, 2, 3 Shawwal | `EidAlFitr` x3 (new, shared — Ramadan Hayit) |
| 10, 11, 12 Dhu al-Hijjah | `EidAlAdha` x3 (new, shared — Kurban Hayit) |

Confirmed via web search: Uzbekistan's official list of seven fixed public holidays (New Year, Women's Day, Navruz, Memorial/Memory Day, Independence Day, Teacher's Day, Constitution Day) plus the two three-day Hijri observances (Ramadan Hayit, Kurban Hayit).

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

- [ ] **Step 1: Edit the enum**

Insert these new members in alphabetical position among the existing members:

```
    AshuraDay, // Pakistan, Bangladesh
    BengaliNewYear, // Bangladesh
    ConstitutionDayOfUzbekistan, // Uzbekistan
    EidAlAdha, // Pakistan, Bangladesh, Uzbekistan
    EidAlFitr, // Pakistan, Bangladesh, Uzbekistan
    EidMiladUnNabi, // Pakistan, Bangladesh
    IqbalDay, // Pakistan
    KashmirSolidarityDay, // Pakistan
    LanguageMovementDay, // Bangladesh
    MemoryAndHonorDay, // Uzbekistan
    NationalMourningDay, // Bangladesh
    NowruzDay, // Uzbekistan
    PakistanDay, // Pakistan
    QuaidEAzamDay, // Pakistan
    TeachersAndInstructorsDay, // Uzbekistan
```

Also broaden the `//` comments on these existing members:

```
    IndependenceDay, // USA, Ukraine, Finland, Bulgaria, Estonia, Iceland, Malta, Cyprus, Moldova, Montenegro, North Macedonia, Pakistan, Bangladesh, Uzbekistan
    VictoryDay, // France, Ukraine, Czech Republic, Slovakia, Estonia, Moldova, Bangladesh
```

- [ ] **Step 2: Build to verify the enum compiles**

Run: `cd TemporaLinq && dotnet build`
Expected: `Build succeeded.` with 0 errors.

- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for Pakistan, Bangladesh, Uzbekistan"
```

---

## Task 2: Add Pakistan national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Pakistan/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/PakistanTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `HijriCalendarCalculation.DatesInGregorianYear(int, int, int) -> IEnumerable<DateOnly>`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test** (assert 2026 count and fixed dates; movable Hijri dates computed via `HijriCalendarCalculation.DatesInGregorianYear(2026, month, day)` for each of Eid al-Fitr/al-Adha/Ashura/Milad, asserting the day-count of dates returned and that each is present with the right name)

- [ ] **Step 2: Run test to verify it fails** — `cd TemporaLinq && dotnet test --framework net10.0 --filter PakistanTest` — expect compile-error FAIL.

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Pakistan;

/// <summary>
/// Provides Pakistani national public holidays. The Islamic-calendar holidays
/// (Eid al-Fitr, Eid al-Adha, Ashura, Eid Milad-un-Nabi) are computed via
/// <see cref="HijriCalendarCalculation"/> — see that class's documentation for
/// the +/-1, rarely +/-2, day real-world moon-sighting approximation caveat.
/// December 25 is gazetted as Quaid-e-Azam Day (Muhammad Ali Jinnah's
/// birthday), coinciding with Christmas Day.
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
            new(new DateOnly(year, 2, 5), KashmirSolidarityDay),
            new(new DateOnly(year, 3, 23), PakistanDay),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(new DateOnly(year, 8, 14), IndependenceDay),
            new(new DateOnly(year, 11, 9), IqbalDay),
            new(new DateOnly(year, 12, 25), QuaidEAzamDay),
        };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 3).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 11).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 12).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 1, 9).Select(d => new Holiday(d, AshuraDay)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 1, 10).Select(d => new Holiday(d, AshuraDay)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12).Select(d => new Holiday(d, EidMiladUnNabi)));

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes** — `cd TemporaLinq && dotnet test --framework net10.0 --filter PakistanTest` — expect PASS.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Pakistan/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/PakistanTest.cs
git commit -m "feat: add Pakistan national holidays"
```

---

## Task 3: Add Bangladesh national holidays (Hijri-based and fixed civil only)

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Bangladesh/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/BangladeshTest.cs`

**Interfaces:** Same as Task 2.

- [ ] **Step 1: Write the failing test**
- [ ] **Step 2: Run test to verify it fails** — `--filter BangladeshTest`
- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Bangladesh;

/// <summary>
/// Provides Bangladeshi national public holidays: fixed civil dates and the
/// Islamic-calendar holidays (Eid al-Fitr, Eid al-Adha, Ashura, Eid
/// Milad-un-Nabi), computed via <see cref="HijriCalendarCalculation"/> — see
/// that class's documentation for the +/-1, rarely +/-2, day real-world
/// moon-sighting approximation caveat. Bangladesh's Hindu- and
/// Buddhist-calendar minority holidays (Durga Puja, Buddha Purnima) are
/// deliberately out of scope: they require a Bengali/Hindu lunisolar or
/// Buddhist calendar calculation with no .NET support today, and are deferred
/// pending a future calendar calculation mechanism for those calendars.
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
            new(new DateOnly(year, 2, 21), LanguageMovementDay),
            new(new DateOnly(year, 3, 26), IndependenceDay),
            new(new DateOnly(year, 4, 14), BengaliNewYear),
            new(new DateOnly(year, 5, 1), LabourDay),
            new(new DateOnly(year, 8, 15), NationalMourningDay),
            new(new DateOnly(year, 12, 16), VictoryDay),
            new(new DateOnly(year, 12, 25), ChristmasDay),
        };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 3).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 11).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 12).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 1, 10).Select(d => new Holiday(d, AshuraDay)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12).Select(d => new Holiday(d, EidMiladUnNabi)));

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes** — `--filter BangladeshTest`
- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Bangladesh/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/BangladeshTest.cs
git commit -m "feat: add Bangladesh national holidays (Hijri and fixed civil only)"
```

---

## Task 4: Add Uzbekistan national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Uzbekistan/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/UzbekistanTest.cs`

**Interfaces:** Same as Task 2 (no Ashura/Milad — Uzbekistan's state calendar has only the two Eids).

- [ ] **Step 1: Write the failing test**
- [ ] **Step 2: Run test to verify it fails** — `--filter UzbekistanTest`
- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Uzbekistan;

/// <summary>
/// Provides Uzbek national public holidays. Nowruz (Mar 21) is Uzbekistan's
/// fixed Gregorian civil-calendar spring holiday, not an astronomically
/// calculated date like Iran's. Eid al-Fitr (Ramadan Hayit) and Eid al-Adha
/// (Kurban Hayit) are computed via <see cref="HijriCalendarCalculation"/> —
/// see that class's documentation for the +/-1, rarely +/-2, day real-world
/// moon-sighting approximation caveat.
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
            new(new DateOnly(year, 3, 8), InternationalWomensDay),
            new(new DateOnly(year, 3, 21), NowruzDay),
            new(new DateOnly(year, 5, 9), MemoryAndHonorDay),
            new(new DateOnly(year, 9, 1), IndependenceDay),
            new(new DateOnly(year, 10, 1), TeachersAndInstructorsDay),
            new(new DateOnly(year, 12, 8), ConstitutionDayOfUzbekistan),
        };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 3).Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 11).Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 12).Select(d => new Holiday(d, EidAlAdha)));

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes** — `--filter UzbekistanTest`
- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Uzbekistan/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/UzbekistanTest.cs
git commit -m "feat: add Uzbekistan national holidays"
```

---

## Task 5: Update spec checklist and run full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1: Update the Asia checklist section**

Update the Tier AS2 line to mark Pakistan, Bangladesh, and Uzbekistan done, e.g.:

```
- Tier AS2: Vietnam (Chinese-lunisolar-computable, approximate), Philippines, Indonesia (Hijri- and Easter-computable components only — Nyepi/Vesak deferred), Malaysia (Hijri- and Chinese-lunisolar-computable components only — Hindu/Buddhist components deferred), Saudi Arabia (Hijri-computable), UAE (Hijri-computable)
- Done: ✅ Pakistan, ✅ Bangladesh (Hijri-based and fixed civil holidays only — Hindu/Buddhist minority holidays (Durga Puja, Buddha Purnima) remain deferred pending a future Hindu/Buddhist calendar calculation mechanism)
```

And move Uzbekistan out of Tier AS4:

```
- Tier AS4 (low priority): 🔴 Sri Lanka, 🔴 Nepal, 🔴 Myanmar, 🔴 Cambodia, 🔴 Laos, 🔴 Mongolia, remaining Central Asia (Hijri-computable)
- Done: ✅ Uzbekistan (Tier AS4)
```

- [ ] **Step 2: Run the full test suite**

Run: `cd TemporaLinq && dotnet test --framework net10.0`
Expected: all tests pass, 0 failures.

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Pakistan, Bangladesh, Uzbekistan done in worldwide holidays checklist"
```
