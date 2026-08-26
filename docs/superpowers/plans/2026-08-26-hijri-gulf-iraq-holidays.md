# Hijri Gulf + Iraq Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for five Hijri-calendar-dependent Middle Eastern countries — Saudi Arabia, United Arab Emirates, Qatar, Kuwait, and Iraq — to `TemporaLinq.Holidays`, using the newly-merged `HijriCalendarCalculation.DatesInGregorianYear(int gregorianYear, int hijriMonth, int hijriDay) -> IEnumerable<DateOnly>` mechanism for all Islamic-calendar holidays.

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs` (the `Asia` folder is new — sibling to the existing `Europe` and `NorthAmerica` folders), computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`). Multi-day Islamic holidays (Eid al-Fitr, Eid al-Adha) are modeled as multiple consecutive `Holiday` entries sharing the same `HolidayNames` member, using `.Select(d => d.AddDays(n))` off the base `DatesInGregorianYear` call — the same pattern the codebase already uses for Serbia's two-day Statehood Day and Slovenia's two-day Labour Day, just generated from a computed base date instead of a literal one. Each country also gets a test file at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs` following the existing `HungaryTest`/`AndorraTest` pattern, asserting against calendar year 2026.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute, `System.Globalization.HijriCalendar` via `HijriCalendarCalculation`.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md` (Tier AS2/AS3 — Saudi Arabia, UAE, Qatar, Kuwait, Iraq)

## Global Constraints

- All five countries use `HijriCalendarCalculation.DatesInGregorianYear` for every Islamic-calendar holiday. Per that mechanism's own documented caveat, the tabular Hijri calendar is a deterministic approximation of real-world moon-sighting announcements and can differ by +/-1, rarely +/-2, days. Every `NationalHolidays.cs` file in this plan repeats that caveat in its XML doc comment, and Iraq's additionally notes that Sunni/Shia moon-sighting authorities there occasionally differ by a day from each other, independent of the tabular-vs-real-world gap.
- `HijriCalendarCalculation.DatesInGregorianYear` occasionally returns **two** dates for a single Gregorian year (drift). Multi-day spans are built by projecting each returned base date forward with `AddDays`, so a drift year correctly produces two independent multi-day spans rather than silently dropping one occurrence. This only matters for `GetHolidaysForYear`'s general correctness (not exercised by the 2026 tests below, which have exactly one occurrence of each).
- Reference Gregorian dates for calendar year 2026, computed directly against `System.Globalization.HijriCalendar` (used for every test assertion in this plan):
  - 1 Muharram 1448 (Islamic New Year) → 2026-06-16
  - 10 Muharram 1448 (Ashura) → 2026-06-25
  - 12 Rabi' al-awwal 1448 (Mawlid / Prophet's Birthday) → 2026-08-25
  - 1 Shawwal 1447 (Eid al-Fitr day 1) → 2026-03-19 (days 2-4: 03-20, 03-21, 03-22)
  - 9 Dhu al-Hijjah 1447 (Day of Arafah) → 2026-05-25
  - 10 Dhu al-Hijjah 1447 (Eid al-Adha day 1) → 2026-05-26 (days 2-4: 05-27, 05-28, 05-29)
- Multi-day holiday spans follow each country's own statutory/official-decree convention (verified via web search against multiple 2026 sources), not a single Gulf-wide default:
  - **Saudi Arabia** (Ministry of Human Resources and Social Development rule): Eid al-Fitr = 4 days (1-4 Shawwal). Eid al-Adha = 4 days starting from the Day of Arafah (9 Dhu al-Hijjah) through 12 Dhu al-Hijjah — modeled as one `ArafahDay` entry (9 Dhu al-Hijjah) plus three `EidAlAdha` entries (10-12 Dhu al-Hijjah).
  - **UAE**: Eid al-Fitr = 4 days (1-4 Shawwal, matches the officially announced March 19-22, 2026 span). Eid al-Adha = `ArafahDay` (9 Dhu al-Hijjah) plus 3 days `EidAlAdha` (10-12 Dhu al-Hijjah), matching the UAE Cabinet's Arafat-Day-plus-3 pattern.
  - **Qatar**: Statutory minimum under Qatari Labour Law Article 74 — Eid al-Fitr = 3 days (1-3 Shawwal), Eid al-Adha = 3 days (10-12 Dhu al-Hijjah). (Qatar's government sector often decrees longer ad-hoc extensions in a given year — e.g. a 7-day Eid al-Fitr break was announced for 2026 — but that is an annual discretionary decree, not a stable formula, so this implementation uses the codified statutory minimum, consistent with how the rest of this codebase favors formula-stable definitions over year-specific decrees.)
  - **Kuwait** (Labour Law Article 68): Islamic New Year = 1 day. Eid al-Fitr = 3 days (1-3 Shawwal). Waqfat Arafat (Day of Arafah, reusing `ArafahDay`) = 1 day (9 Dhu al-Hijjah). Eid al-Adha = 3 days (10-12 Dhu al-Hijjah). Prophet's Birthday = 1 day. Ashura is included per this plan's explicit scope (government offices observe it) even though it is not itemized in the private-sector labor-law article — noted in the implementation's doc comment.
  - **Iraq**: Eid al-Fitr = 3 days (1-3 Shawwal). Eid al-Adha = 4 days (10-13 Dhu al-Hijjah, "Feast of Sacrifice"). Islamic New Year, Ashura, and Mawlid = 1 day each. Fixed civil holidays limited to New Year's Day (Jan 1) and Republic Day (Jul 14, 1958 revolution) per this tier's explicit scope — Iraq's broader fixed-holiday calendar (Army Day, national-unity days, etc.) is deliberately out of scope for this pass, to avoid asserting politically-contested or frequently-revised dates without a firmer source.
- Qatar's National Sports Day is a movable civil (non-Hijri) holiday: the second Tuesday of February, computed the same way the codebase already computes "Nth weekday of month" holidays (USA's `BirthdayOfMartinLutherKingJr`, UK's `EarlyMayBankHoliday`, etc.) — `Dates.Invariant().From(new DateOnly(year, 2, 8)).First(DayOfWeek.Tuesday)` (the second Tuesday always falls between Feb 8-14).
- Countries live at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs`; tests at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs`. The `Asia` directory does not exist yet in either project — Task 2 creates it.
- Reuse existing `HolidayNames` enum members wherever the concept matches (`NewYearsDay`, `RepublicDay` for Iraq, `LiberationDay` for Kuwait), broadening the `//` comment to list the additional country, per the established convention. Only add new enum members for genuinely new concepts.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox).
- After all five countries are done, update the checklist in the spec doc (`docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`), marking these five done in the Asia tier lines, matching the exact style of the Europe-tier checklist lines.

---

## Reference: full holiday list per country (2026 dates)

### Saudi Arabia — 9 holidays
| Date(s) | HolidayNames member |
|---|---|
| Mar 19-22 (1-4 Shawwal) | `EidAlFitr` (4 entries) |
| May 25 (9 Dhu al-Hijjah) | `ArafahDay` |
| May 26-28 (10-12 Dhu al-Hijjah) | `EidAlAdha` (3 entries) |
| Sep 23 | `NationalDayOfSaudiArabia` (new) |

### UAE — 14 holidays
| Date(s) | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Mar 19-22 (1-4 Shawwal) | `EidAlFitr` (4 entries) |
| May 25 (9 Dhu al-Hijjah) | `ArafahDay` |
| May 26-28 (10-12 Dhu al-Hijjah) | `EidAlAdha` (3 entries) |
| Jun 16 (1 Muharram) | `IslamicNewYear` (new) |
| Aug 25 (12 Rabi' al-awwal) | `ProphetsBirthday` (new) |
| Dec 1 | `CommemorationDayOfUae` (new) |
| Dec 2-3 | `NationalDayOfUae` (new, 2 entries) |

### Qatar — 8 holidays
| Date(s) | HolidayNames member |
|---|---|
| Feb 10 (2nd Tuesday of Feb) | `SportsDayOfQatar` (new) |
| Mar 19-21 (1-3 Shawwal) | `EidAlFitr` (3 entries) |
| May 26-28 (10-12 Dhu al-Hijjah) | `EidAlAdha` (3 entries) |
| Dec 18 | `NationalDayOfQatar` (new) |

### Kuwait — 12 holidays
| Date(s) | HolidayNames member |
|---|---|
| Jun 16 (1 Muharram) | `IslamicNewYear` |
| Mar 19-21 (1-3 Shawwal) | `EidAlFitr` (3 entries) |
| May 25 (9 Dhu al-Hijjah) | `ArafahDay` |
| May 26-28 (10-12 Dhu al-Hijjah) | `EidAlAdha` (3 entries) |
| Jun 25 (10 Muharram) | `AshuraDay` (new) |
| Aug 25 (12 Rabi' al-awwal) | `ProphetsBirthday` |
| Feb 25 | `NationalDayOfKuwait` (new) |
| Feb 26 | `LiberationDay` (reuse — broaden comment) |

### Iraq — 13 holidays
| Date(s) | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Mar 19-21 (1-3 Shawwal) | `EidAlFitr` (3 entries) |
| May 26-29 (10-13 Dhu al-Hijjah) | `EidAlAdha` (4 entries) |
| Jun 16 (1 Muharram) | `IslamicNewYear` |
| Jun 25 (10 Muharram) | `AshuraDay` |
| Jul 14 | `RepublicDay` (reuse — broaden comment) |
| Aug 25 (12 Rabi' al-awwal) | `ProphetsBirthday` |

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

**Interfaces:**
- Produces: the enum members every later task's `NationalHolidays.cs` references by name (via `using static TemporaLinq.Holidays.HolidayNames;`).

- [ ] **Step 1: Edit the enum**

Open `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`. Insert these new members, keeping the existing alphabetical ordering (insert each new line in alphabetical position among the existing members):

```
    ArafahDay, // Saudi Arabia, UAE, Kuwait
    AshuraDay, // Kuwait, Iraq
    CommemorationDayOfUae, // UAE
    EidAlAdha, // Saudi Arabia, UAE, Qatar, Kuwait, Iraq
    EidAlFitr, // Saudi Arabia, UAE, Qatar, Kuwait, Iraq
    IslamicNewYear, // UAE, Kuwait, Iraq
    NationalDayOfKuwait, // Kuwait
    NationalDayOfQatar, // Qatar
    NationalDayOfSaudiArabia, // Saudi Arabia
    NationalDayOfUae, // UAE
    ProphetsBirthday, // UAE, Kuwait, Iraq
    SportsDayOfQatar, // Qatar
```

Also broaden the `//` comments on these **existing** members to add the new countries reusing them:

```
    LiberationDay, // Italy, Netherlands, Kuwait
    RepublicDay, // Italy, Portugal, Malta, Iraq
```

- [ ] **Step 2: Build to verify the enum compiles**

Run: `cd TemporaLinq && dotnet build`
Expected: `Build succeeded.` with 0 errors.

- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for Hijri Gulf + Iraq countries"
```

---

## Task 2: Add Saudi Arabia national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/SaudiArabia/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/SaudiArabiaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>` base, `HijriCalendarCalculation.DatesInGregorianYear(int, int, int) -> IEnumerable<DateOnly>`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.SaudiArabia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class SaudiArabiaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(9);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 23) && h.Name == NationalDayOfSaudiArabia);
    }

    [Fact]
    public void GetHolidays_ContainsEidAlFitr()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 20) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 22) && h.Name == EidAlFitr);
    }

    [Fact]
    public void GetHolidays_ContainsArafahDayAndEidAlAdha()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 25) && h.Name == ArafahDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 26) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 27) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 28) && h.Name == EidAlAdha);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SaudiArabiaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.SaudiArabia;

/// <summary>
/// Provides Saudi Arabian national public holidays. Eid al-Fitr and Eid al-Adha
/// (including the Day of Arafah) are computed from the tabular Hijri calendar via
/// <see cref="HijriCalendarCalculation"/>; real-world moon-sighting announcements
/// can differ from this calculation by +/-1, rarely +/-2, days. Saudi Arabia has
/// no statutory Gregorian New Year's Day holiday.
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
            new(new DateOnly(year, 9, 23), NationalDayOfSaudiArabia),
        };

        foreach (var eidAlFitrStart in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
        {
            for (var day = 0; day < 4; day++)
                holidays.Add(new Holiday(eidAlFitrStart.AddDays(day), EidAlFitr));
        }

        foreach (var arafahDay in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 9))
        {
            holidays.Add(new Holiday(arafahDay, ArafahDay));
            for (var day = 1; day <= 3; day++)
                holidays.Add(new Holiday(arafahDay.AddDays(day), EidAlAdha));
        }

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SaudiArabiaTest`
Expected: PASS, 4 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/SaudiArabia/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/SaudiArabiaTest.cs
git commit -m "feat: add Saudi Arabia national holidays"
```

---

## Task 3: Add UAE national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Uae/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/UaeTest.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Uae;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class UaeTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(14);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 1) && h.Name == CommemorationDayOfUae);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 2) && h.Name == NationalDayOfUae);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 3) && h.Name == NationalDayOfUae);
    }

    [Fact]
    public void GetHolidays_ContainsEidAlFitrAndEidAlAdha()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 22) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 25) && h.Name == ArafahDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 26) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 28) && h.Name == EidAlAdha);
    }

    [Fact]
    public void GetHolidays_ContainsIslamicNewYearAndMawlid()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 16) && h.Name == IslamicNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 25) && h.Name == ProphetsBirthday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter UaeTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Uae;

/// <summary>
/// Provides United Arab Emirates national public holidays. Islamic-calendar
/// holidays are computed from the tabular Hijri calendar via
/// <see cref="HijriCalendarCalculation"/>; real-world moon-sighting announcements
/// can differ from this calculation by +/-1, rarely +/-2, days.
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
            new(new DateOnly(year, 12, 1), CommemorationDayOfUae),
            new(new DateOnly(year, 12, 2), NationalDayOfUae),
            new(new DateOnly(year, 12, 3), NationalDayOfUae),
        };

        foreach (var eidAlFitrStart in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
        {
            for (var day = 0; day < 4; day++)
                holidays.Add(new Holiday(eidAlFitrStart.AddDays(day), EidAlFitr));
        }

        foreach (var arafahDay in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 9))
        {
            holidays.Add(new Holiday(arafahDay, ArafahDay));
            for (var day = 1; day <= 3; day++)
                holidays.Add(new Holiday(arafahDay.AddDays(day), EidAlAdha));
        }

        foreach (var islamicNewYear in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1))
            holidays.Add(new Holiday(islamicNewYear, IslamicNewYear));

        foreach (var mawlid in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12))
            holidays.Add(new Holiday(mawlid, ProphetsBirthday));

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter UaeTest`
Expected: PASS, 4 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Uae/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/UaeTest.cs
git commit -m "feat: add UAE national holidays"
```

---

## Task 4: Add Qatar national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Qatar/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/QatarTest.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Qatar;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class QatarTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(8);
    }

    [Fact]
    public void GetHolidays_ContainsFixedAndMovableCivilHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 10) && h.Name == SportsDayOfQatar);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 18) && h.Name == NationalDayOfQatar);
    }

    [Fact]
    public void GetHolidays_ContainsEidAlFitrAndEidAlAdha()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 20) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 26) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 27) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 28) && h.Name == EidAlAdha);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter QatarTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Qatar;

/// <summary>
/// Provides Qatari national public holidays, using the statutory minimums from
/// Qatari Labour Law Article 74 for Eid al-Fitr and Eid al-Adha (the government
/// sector often decrees longer ad-hoc extensions in a given year, which are out
/// of scope here as year-specific decrees rather than a stable formula).
/// Islamic-calendar holidays are computed from the tabular Hijri calendar via
/// <see cref="HijriCalendarCalculation"/>; real-world moon-sighting announcements
/// can differ from this calculation by +/-1, rarely +/-2, days.
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
            new(Dates.Invariant().From(new DateOnly(year, 2, 8)).First(DayOfWeek.Tuesday), SportsDayOfQatar),
            new(new DateOnly(year, 12, 18), NationalDayOfQatar),
        };

        foreach (var eidAlFitrStart in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
        {
            for (var day = 0; day < 3; day++)
                holidays.Add(new Holiday(eidAlFitrStart.AddDays(day), EidAlFitr));
        }

        foreach (var eidAlAdhaStart in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10))
        {
            for (var day = 0; day < 3; day++)
                holidays.Add(new Holiday(eidAlAdhaStart.AddDays(day), EidAlAdha));
        }

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter QatarTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Qatar/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/QatarTest.cs
git commit -m "feat: add Qatar national holidays"
```

---

## Task 5: Add Kuwait national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Kuwait/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/KuwaitTest.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Kuwait;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class KuwaitTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(12);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 25) && h.Name == NationalDayOfKuwait);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 26) && h.Name == LiberationDay);
    }

    [Fact]
    public void GetHolidays_ContainsEidAlFitrAndEidAlAdha()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 25) && h.Name == ArafahDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 26) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 28) && h.Name == EidAlAdha);
    }

    [Fact]
    public void GetHolidays_ContainsIslamicNewYearAshuraAndMawlid()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 16) && h.Name == IslamicNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 25) && h.Name == AshuraDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 25) && h.Name == ProphetsBirthday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter KuwaitTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Kuwait;

/// <summary>
/// Provides Kuwaiti national public holidays. Islamic New Year, Eid al-Fitr,
/// Waqfat Arafat, Eid al-Adha, and Prophet's Birthday follow Kuwait Labour Law
/// Article 68; Ashura is included as a government-observed holiday even though
/// it is not itemized in that private-sector article. Islamic-calendar holidays
/// are computed from the tabular Hijri calendar via
/// <see cref="HijriCalendarCalculation"/>; real-world moon-sighting announcements
/// can differ from this calculation by +/-1, rarely +/-2, days.
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
            new(new DateOnly(year, 2, 25), NationalDayOfKuwait),
            new(new DateOnly(year, 2, 26), LiberationDay),
        };

        foreach (var eidAlFitrStart in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
        {
            for (var day = 0; day < 3; day++)
                holidays.Add(new Holiday(eidAlFitrStart.AddDays(day), EidAlFitr));
        }

        foreach (var arafahDay in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 9))
        {
            holidays.Add(new Holiday(arafahDay, ArafahDay));
            for (var day = 1; day <= 3; day++)
                holidays.Add(new Holiday(arafahDay.AddDays(day), EidAlAdha));
        }

        foreach (var islamicNewYear in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1))
            holidays.Add(new Holiday(islamicNewYear, IslamicNewYear));

        foreach (var ashura in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 10))
            holidays.Add(new Holiday(ashura, AshuraDay));

        foreach (var mawlid in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12))
            holidays.Add(new Holiday(mawlid, ProphetsBirthday));

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter KuwaitTest`
Expected: PASS, 4 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Kuwait/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/KuwaitTest.cs
git commit -m "feat: add Kuwait national holidays"
```

---

## Task 6: Add Iraq national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Iraq/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/IraqTest.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Iraq;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class IraqTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(13);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 14) && h.Name == RepublicDay);
    }

    [Fact]
    public void GetHolidays_ContainsEidAlFitrAndEidAlAdha()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 20) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 26) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 27) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 28) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 29) && h.Name == EidAlAdha);
    }

    [Fact]
    public void GetHolidays_ContainsIslamicNewYearAshuraAndMawlid()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 16) && h.Name == IslamicNewYear);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 25) && h.Name == AshuraDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 25) && h.Name == ProphetsBirthday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter IraqTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Iraq;

/// <summary>
/// Provides Iraqi national public holidays. Fixed civil holidays are limited to
/// New Year's Day and Republic Day, per this implementation's deliberately
/// narrow scope (Iraq's broader fixed-holiday calendar is contested/frequently
/// revised and out of scope here). Islamic-calendar holidays are computed from
/// the tabular Hijri calendar via <see cref="HijriCalendarCalculation"/>;
/// real-world moon-sighting announcements can differ from this calculation by
/// +/-1, rarely +/-2, days — and in Iraq specifically, Sunni and Shia religious
/// authorities occasionally announce moon-sighting a day apart from each other,
/// independent of the tabular-calendar gap. This is a known, accepted
/// approximation limitation, not a bug.
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
            new(new DateOnly(year, 7, 14), RepublicDay),
        };

        foreach (var eidAlFitrStart in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
        {
            for (var day = 0; day < 3; day++)
                holidays.Add(new Holiday(eidAlFitrStart.AddDays(day), EidAlFitr));
        }

        foreach (var eidAlAdhaStart in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10))
        {
            for (var day = 0; day < 4; day++)
                holidays.Add(new Holiday(eidAlAdhaStart.AddDays(day), EidAlAdha));
        }

        foreach (var islamicNewYear in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1))
            holidays.Add(new Holiday(islamicNewYear, IslamicNewYear));

        foreach (var ashura in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 10))
            holidays.Add(new Holiday(ashura, AshuraDay));

        foreach (var mawlid in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12))
            holidays.Add(new Holiday(mawlid, ProphetsBirthday));

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter IraqTest`
Expected: PASS, 4 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Iraq/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/IraqTest.cs
git commit -m "feat: add Iraq national holidays"
```

---

## Task 7: Mark these countries done in the spec checklist and run the full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1: Update the checklist**

In `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, update the Asia tier lines (`Tier AS2`, `Tier AS3`) to mark Saudi Arabia, UAE, Qatar, Kuwait, and Iraq done, adding a `Done:` line immediately below the existing Europe-tier `Done:` lines' style, e.g.:

```
- Done: ✅ Saudi Arabia, ✅ UAE, ✅ Qatar, ✅ Kuwait, ✅ Iraq (Hijri Gulf + Iraq batch)
```

and remove those five countries' names (with their now-redundant "(Hijri-computable)" annotations) from the `Tier AS2` / `Tier AS3` lines, leaving the remaining not-yet-done countries in place — matching the exact editing style used when Tier E1-E5 were marked done in the Europe section.

- [ ] **Step 2: Run the full test suite**

Run: `cd TemporaLinq && dotnet test --framework net10.0`
Expected: All tests pass, 0 failing.

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Hijri Gulf + Iraq countries done in worldwide holidays checklist"
```
