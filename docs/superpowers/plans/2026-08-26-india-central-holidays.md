# India Central Government Gazetted Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add India's central-government Gazetted public holidays to `TemporaLinq.Holidays`, restricted to the subset that is formula-computable per the calendar-calculation-mechanisms design: 3 fixed civil days, Christian-Easter-based Good Friday plus fixed Christmas Day, and the four Hijri-calendar-based holidays that appear on the central Gazetted list (Eid al-Fitr, Eid al-Adha/Bakrid, Muharram, Milad-un-Nabi). This is a **deliberately partial** country implementation — Hindu-calendar holidays (Diwali, Holi, Dussehra, Raksha Bandhan, Janmashtami, etc.), Buddha Purnima, Jain/Sikh lunar-date holidays, and state-specific additions (Pongal, Onam, Bihu, etc.) are explicitly out of scope, per `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`'s "No re-litigating India as a whole" note and the Asia Tier AS1 entry in `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`. Do not expand scope beyond what is listed here.

**Verification (via WebSearch, 2026-08-26):** India's DoPT central government Gazetted holiday list for 2026 (17 total; 14 uniform nationwide) confirms Republic Day (Jan 26), Independence Day (Aug 15), Gandhi Jayanti (Oct 2), Good Friday, Christmas Day, and — printed against fixed dates in Annexure-I, subject to moon-sighting — Id-ul-Fitr (Eid al-Fitr, Sat Mar 21 2026), Id-ul-Zuha/Bakrid (Eid al-Adha, Wed May 27 2026), Muharram (Fri Jun 26 2026), and Milad-un-Nabi (Wed Aug 26 2026). Cross-checking these against `HijriCalendarCalculation.DatesInGregorianYear(2026, ...)` for the standard Hijri (month, day) pairs (1 Shawwal, 10 Dhu al-Hijjah, 1 Muharram, 12 Rabi al-Awwal) is expected to reproduce the same four dates, confirming the tabular Hijri calculation is a good approximation for this year. The remaining Gazetted-list items (Holi, Dussehra, Diwali, Guru Nanak's Birthday, Buddha Purnima, etc.) are Hindu/Sikh/Buddhist lunar-calendar-based and are the explicitly out-of-scope portion.

**Architecture:** `NationalHolidays : HolidayEnumerable<NationalHolidays>` record at `TemporaLinq.Holidays/Asia/India/NationalHolidays.cs` (new `Asia` folder), computing a per-year `ImmutableList<Holiday>` memoized via `[Cache]`, using `EasterSundayCalculation.Christian` for Good Friday and `HijriCalendarCalculation.DatesInGregorianYear` for the four Islamic-calendar holidays (each call may yield one or, rarely, two dates in a given Gregorian year — add a `Holiday` for every date returned). Test at `TemporaLinq.Test/Holidays/Asia/IndiaTest.cs` (new `Asia` folder), following the `HungaryTest`/`AndorraTest` pattern, for year 2026.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`

## Reference: full holiday list

| Date (2026) | HolidayNames member | Basis |
|---|---|---|
| Jan 26 | `RepublicDay` (reuse — Italy, Portugal, Malta; broaden comment) | Fixed |
| Mar 21 | `EidAlFitr` (new) | `HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)` (1 Shawwal) |
| Good Friday (Easter - 2) | `GoodFriday` (reuse) | `EasterSundayCalculation.Christian.ForYear(year).AddDays(-2)` |
| May 27 | `EidAlAdha` (new) | `HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10)` (10 Dhu al-Hijjah) |
| Jun 26 | `Muharram` (new) | `HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1)` (1 Muharram) |
| Aug 15 | `IndependenceDay` (reuse — broaden comment) | Fixed |
| Aug 26 | `MiladUnNabi` (new) | `HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12)` (12 Rabi al-Awwal) |
| Oct 2 | `GandhiJayanti` (new) | Fixed |
| Dec 25 | `ChristmasDay` (reuse) | Fixed |

Note: each `DatesInGregorianYear` call can rarely return two dates within one Gregorian year (Hijri year drift) — the implementation must handle this via a loop/`SelectMany`, not by assuming exactly one date.

## Global Constraints

- Do not add any Hindu/Buddhist/Jain/Sikh lunar-calendar holiday or any state-specific holiday. This is a deliberate, already-agreed scope boundary — do not second-guess it.
- `Asia` is a new top-level folder in both `TemporaLinq.Holidays` and `TemporaLinq.Test/Holidays` — create it following the exact same structure as `Europe`.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox).
- The `NationalHolidays` record's XML doc comment must explicitly state the central-Gazetted-only scope and name the deferred categories (Hindu-calendar, Buddhist/Jain/Sikh lunar, state-specific), noting they await a future Hindu/Buddhist calendar calculation mechanism.
- After implementation, update the Asia section of `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md` to mark India done (✅) for the central Gazetted list, keeping the existing wording about deferred Hindu-calendar/state-specific holidays.

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

- [ ] **Step 1: Edit the enum**

Insert these new members in alphabetical order (read the current file first to find each exact slot):

```
    EidAlAdha, // India
    EidAlFitr, // India
    GandhiJayanti, // India
    MiladUnNabi, // India
    Muharram, // India
```

Broaden comments on existing members reused here:

```
    ChristmasDay, // (no comment currently — leave as-is, it's a bare shared name)
    GoodFriday, // (bare shared name — leave as-is)
    IndependenceDay, // USA, Ukraine, Finland, Bulgaria, Estonia, Iceland, Malta, Cyprus, Moldova, Montenegro, North Macedonia, India
    RepublicDay, // Italy, Portugal, Malta, India
```

(Check the actual current file for `ChristmasDay`/`GoodFriday` comment state before editing — some shared names have no per-country comment at all; only edit comments that already list countries.)

- [ ] **Step 2: Build to verify the enum compiles**

Run: `cd TemporaLinq && dotnet build`
Expected: `Build succeeded.` with 0 errors.

- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for India"
```

---

## Task 2: Add India central Gazetted national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/India/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/IndiaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>` base, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HijriCalendarCalculation.DatesInGregorianYear(int, int, int) -> IEnumerable<DateOnly>`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.India;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class IndiaTest
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

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 26) && h.Name == RepublicDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 2) && h.Name == GandhiJayanti);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsGoodFriday()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
    }

    [Fact]
    public void GetHolidays_ContainsHijriBasedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 27) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 26) && h.Name == Muharram);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 26) && h.Name == MiladUnNabi);
    }
}
```

Verify each Hijri-derived date above against `HijriCalendarCalculation.DatesInGregorianYear(2026, ...)` before finalizing — if the tabular calculation yields a date 1 day off from the DoPT-published date (moon-sighting adjustment), use the calculated date in the assertion (not the DoPT date), since this library computes a formula, not a decree — matching the existing `EasterSundayCalculation` precedent. Note in a code comment if any date differs from the DoPT-published one.

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter IndiaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.India;

/// <summary>
/// Provides India's central-government Gazetted public holidays — the subset of the
/// national holiday calendar that is deterministically formula-computable: three fixed
/// civil days (Republic Day, Independence Day, Gandhi Jayanti), Good Friday (Christian
/// Easter calculation) and Christmas Day, and the four Islamic-calendar holidays that
/// appear on the central Gazetted list (Eid al-Fitr, Eid al-Adha/Bakrid, Muharram,
/// Milad-un-Nabi), computed via <see cref="HijriCalendarCalculation"/>.
///
/// This is a deliberately partial implementation. India's Gazetted holiday list also
/// includes several Hindu-calendar holidays (Diwali, Holi, Dussehra, Raksha Bandhan,
/// Janmashtami, etc.), Buddha Purnima, and Jain/Sikh lunar-date holidays — none of
/// which have an accepted simple arithmetic formula or .NET calendar support, and are
/// therefore out of scope pending a future Hindu/Buddhist calendar calculation
/// mechanism (see docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md).
/// State-specific additions (e.g. Pongal, Onam, Bihu) are likewise out of scope.
///
/// As with <see cref="HijriCalendarCalculation"/> generally, the Islamic-calendar dates
/// here are a tabular approximation; real-world moon-sighting-confirmed dates can differ
/// by +/-1, rarely +/-2, days from the calculated date.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);

        var holidays = new List<Holiday>
        {
            new(new DateOnly(year, 1, 26), RepublicDay),
            new(easter.AddDays(-2), GoodFriday),
            new(new DateOnly(year, 8, 15), IndependenceDay),
            new(new DateOnly(year, 10, 2), GandhiJayanti),
            new(new DateOnly(year, 12, 25), ChristmasDay),
        };

        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1)
            .Select(d => new Holiday(d, EidAlFitr)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10)
            .Select(d => new Holiday(d, EidAlAdha)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 1, 1)
            .Select(d => new Holiday(d, Muharram)));
        holidays.AddRange(HijriCalendarCalculation.DatesInGregorianYear(year, 3, 12)
            .Select(d => new Holiday(d, MiladUnNabi)));

        return holidays.Order().ToImmutableList();
    }
}
```

Adjust the exact Hijri (month, day) pairs only if research in Step 1 finds a discrepancy with the standard definitions (1 Shawwal = Eid al-Fitr, 10 Dhu al-Hijjah = Eid al-Adha, 1 Muharram = Islamic New Year, 12 Rabi al-Awwal = Milad-un-Nabi/Mawlid).

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter IndiaTest`
Expected: PASS, 4 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/India/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/IndiaTest.cs
git commit -m "feat: add India central government Gazetted holidays"
```

---

## Task 3: Update spec checklist and run full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1: Update the Asia checklist line**

Change the India entry inside the Tier AS1 line from:

```
- Tier AS1: India (Hijri- and Easter-computable; central Gazetted list only — Hindu-calendar holidays and state-specific days deferred), Israel (...
```

to:

```
- Tier AS1: ✅ India (central Gazetted list only — three fixed civil days, Good Friday, Christmas Day, and Hijri-computable Eid al-Fitr/Eid al-Adha/Muharram/Milad-un-Nabi; Hindu-calendar holidays and state-specific days remain deferred pending a future Hindu/Buddhist calendar mechanism); Tier AS1 remaining: Israel (...
```

matching the existing "Done: ✅ ..." / "remaining: ..." wording style used elsewhere in the Europe section.

- [ ] **Step 2: Run the full test suite**

Run: `cd TemporaLinq && dotnet test --framework net10.0`
Expected: All tests pass, 0 failures.

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark India central Gazetted holidays done in worldwide holidays checklist"
```
</content>
