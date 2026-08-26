# Tier E4 Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for six Tier E4 European countries (Estonia, Iceland, Luxembourg, Malta, Cyprus, Moldova) to `TemporaLinq.Holidays`, following the exact pattern already used by Tier E1/E2/E3 countries.

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record at `TemporaLinq.Holidays/Europe/<Country>/NationalHolidays.cs`, computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`), using `EasterSundayCalculation.Christian` (Estonia, Iceland, Luxembourg, Malta) or `EasterSundayCalculation.ChristianOrthodox` (Cyprus, Moldova) for movable feasts. Iceland additionally needs two homegrown formula holidays (First Day of Summer, Commerce Day) computed directly from `DateOnly`, not from Easter. New `HolidayNames` enum members are added once, up front, then reused by every country task. Each country also gets a test file at `TemporaLinq.Test/Holidays/Europe/<Country>Test.cs` following the existing `HungaryTest`/`BulgariaTest` pattern: one test asserting total holiday count for 2026, one asserting fixed-date holidays, one asserting movable-feast dates computed independently via `EasterSundayCalculation`.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

## Global Constraints

- Belarus (already flagged 🔴 in the spec) and Bosnia and Herzegovina are excluded from this tier. Bosnia and Herzegovina's holiday calendar is fragmented by entity (Federation vs. Republika Srpska) and includes Islamic lunar-calendar holidays (Eid al-Fitr, Eid al-Adha) that cannot be computed with the existing Easter-formula pattern — it needs the same `StaticHolidayEnumerable<T>` mechanism as other 🔴 countries. Task 8 updates the spec checklist to flag it 🔴 and moves it out of this tier's "done" line.
- The remaining six countries are all computable via the existing formula-based pattern — no new base class needed. Iceland needs two additional pure-`DateOnly` formulas (not Easter-based): First Day of Summer (first Thursday on/after April 19) and Commerce Day (first Monday of August).
- Countries live at `TemporaLinq.Holidays/Europe/<Country>/NationalHolidays.cs`; tests at `TemporaLinq.Test/Holidays/Europe/<Country>Test.cs`.
- Reuse existing `HolidayNames` enum members wherever the concept matches (broadening the `//` comment to list the additional country), per the established convention (e.g. `SecondJanuary // Scotland, Romania`). Only add new enum members for genuinely new concepts.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox).
- After all six countries are done, update the checklist in the spec doc (`docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`) marking Tier E4 done, matching the exact style of the Tier E1/E2/E3 checklist lines, and flag Bosnia and Herzegovina 🔴.

---

## Reference: full holiday list per country

### Estonia (`EasterSundayCalculation.Christian`) — 12 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Feb 24 | `IndependenceDay` (reuse — Estonian Independence Day) |
| easter - 2 | `GoodFriday` |
| easter | `EasterSunday` |
| May 1 | `LabourDay` (Estonia: Spring Day, same date/concept) |
| easter + 49 | `WhitSunday` |
| Jun 23 | `VictoryDay` (reuse — Estonian Victory Day, Battle of Võnnu) |
| Jun 24 | `MidsummerDay` (reuse) |
| Aug 20 | `RestorationOfIndependenceDay` (reuse — Day of Restoration of Independence, 1991) |
| Dec 24 | `ChristmasEve` (reuse) |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `BoxingDay` (reuse) |

### Iceland (`EasterSundayCalculation.Christian` + two standalone formulas) — 14 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| easter - 3 | `MaundyThursday` (reuse) |
| easter - 2 | `GoodFriday` |
| easter | `EasterSunday` |
| easter + 1 | `EasterMonday` |
| first Thursday on/after Apr 19 | `FirstDayOfSummer` (new) |
| May 1 | `LabourDay` |
| easter + 39 | `AscensionDay` |
| easter + 49 | `WhitSunday` |
| easter + 50 | `WhitMonday` |
| Jun 17 | `IndependenceDay` (reuse — Icelandic National Day) |
| first Monday of August | `CommerceDay` (new) |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `BoxingDay` (reuse) |

### Luxembourg (`EasterSundayCalculation.Christian`) — 10 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| easter + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| easter + 39 | `AscensionDay` |
| easter + 50 | `WhitMonday` |
| Jun 23 | `NationalDayOfLuxembourg` (new) |
| Aug 15 | `AssumptionDay` |
| Nov 1 | `AllSaintsDay` |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` (reuse) |

### Malta (`EasterSundayCalculation.Christian`) — 14 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Feb 10 | `FeastOfStPaulsShipwreck` (new) |
| Mar 19 | `FeastOfStJoseph` (new) |
| Mar 31 | `FreedomDayOfMalta` (new) |
| easter - 2 | `GoodFriday` |
| May 1 | `LabourDay` |
| Jun 7 | `SetteGiugno` (new) |
| Jun 29 | `StPeterAndPaul` (reuse — Imnarja) |
| Aug 15 | `AssumptionDay` |
| Sep 8 | `OurLadyOfVictoriesDay` (new) |
| Sep 21 | `IndependenceDay` (reuse) |
| Dec 8 | `ImmaculateConception` |
| Dec 13 | `RepublicDay` (reuse) |
| Dec 25 | `ChristmasDay` |

### Cyprus (`EasterSundayCalculation.ChristianOrthodox`) — 15 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 6 | `Epiphany` |
| orthodoxEaster - 48 | `CleanMonday` (reuse) |
| Mar 25 | `GreekIndependenceDay` (reuse) |
| Apr 1 | `NationalDayOfCyprus` (new — Cyprus National Day) |
| orthodoxEaster - 2 | `GoodFriday` |
| orthodoxEaster | `EasterSunday` |
| orthodoxEaster + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| orthodoxEaster + 50 | `WhitMonday` (reuse — Kataklysmos) |
| Aug 15 | `AssumptionDay` |
| Oct 1 | `IndependenceDay` (reuse — Cyprus Independence Day) |
| Oct 28 | `OhiDay` (reuse) |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` (reuse) |

### Moldova (`EasterSundayCalculation.ChristianOrthodox`) — 11 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 7 | `ChristmasDay` (reuse — Orthodox Nativity) |
| Mar 8 | `InternationalWomensDay` |
| orthodoxEaster | `EasterSunday` |
| orthodoxEaster + 1 | `EasterMonday` |
| orthodoxEaster + 9 | `MemorialDay` (reuse — Paștele Blajinilor / Day of Remembrance) |
| May 1 | `LabourDay` |
| May 9 | `VictoryDay` (reuse — Victory Day / Europe Day) |
| Aug 27 | `IndependenceDay` (reuse) |
| Aug 31 | `OurLanguageDay` (new — Limba Noastră) |
| Dec 25 | `ChristmasDay` (reuse — Western Christmas, also recognized since 2013) |

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

**Interfaces:**
- Produces: the enum members every later task's `NationalHolidays.cs` references by name (via `using static TemporaLinq.Holidays.HolidayNames;`).

- [ ] **Step 1: Edit the enum**

Open `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`. Insert these new members, keeping the existing alphabetical ordering of the enum (insert each new line in alphabetical position among the existing members):

```
    CommerceDay, // Iceland
    FeastOfStJoseph, // Malta
    FeastOfStPaulsShipwreck, // Malta
    FirstDayOfSummer, // Iceland
    FreedomDayOfMalta, // Malta
    NationalDayOfCyprus, // Cyprus
    NationalDayOfLuxembourg, // Luxembourg
    OurLadyOfVictoriesDay, // Malta
    OurLanguageDay, // Moldova
    SetteGiugno, // Malta
```

They must be inserted in their correct alphabetical slots (case-insensitive, matching existing style), not appended in a block — e.g. `CommerceDay` goes between `ColumbusDay` and `ConstitutionDayOfDenmark`, `FeastOfStJoseph` goes right before `FeastOfStJohnTheBaptist`... wait, alphabetically `FeastOfStJanuarius` < `FeastOfStJoseph` < `FeastOfStJohnTheBaptist`? No: compare "FeastOfStJanuarius" vs "FeastOfStJohnTheBaptist" vs "FeastOfStJoseph" vs "FeastOfStPaulsShipwreck" vs "FeastOfStPetronius" character-by-character after the shared "FeastOfStJ"/"FeastOfStP" prefix: "Janu..." < "John..." < "Joseph" (J-a < J-o, and within Jo: "John" < "Joseph" since 'h' < 's'), so the order is `FeastOfStJanuarius`, `FeastOfStJohnTheBaptist`, `FeastOfStJoseph`, then `FeastOfStPaulsShipwreck` before `FeastOfStPetronius` ("Paul" < "Petr..." since 'a' < 'e'). Read the full current file first to find each exact slot; do not guess from this description alone.

Also broaden the `//` comments on these **existing** members to add the new countries reusing them:

```
    BoxingDay, // UK, Canada, Australia, NZ, Estonia, Iceland, Cyprus
    ChristmasEve, // Czech Republic, Estonia
    CleanMonday, // Greece, Cyprus
    GreekIndependenceDay, // Greece, Cyprus
    IndependenceDay, // USA, Ukraine, Finland, Bulgaria, Estonia, Iceland, Malta, Cyprus, Moldova
    MaundyThursday, // Denmark, Norway, Iceland
    MemorialDay, // USA, Moldova
    MidsummerDay, // Sweden, Finland, Lithuania, Latvia, Estonia
    OhiDay, // Greece, Cyprus
    RepublicDay, // Italy, Portugal, Malta
    RestorationOfIndependenceDay, // Portugal, Lithuania, Latvia, Estonia
    StPeterAndPaul, // Italy, Malta
    VictoryDay // France, Ukraine, Czech Republic, Slovakia, Estonia, Moldova
```

- [ ] **Step 2: Build to verify the enum compiles**

Run: `cd TemporaLinq && dotnet build`
Expected: `Build succeeded.` with 0 errors (pre-existing warnings unrelated to this change are fine).

- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for Tier E4 countries"
```

---

## Task 2: Add Estonia national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Estonia/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/EstoniaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>` base (`TemporaLinq.Holidays`), `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Estonia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class EstoniaTest
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

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 24) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 23) && h.Name == VictoryDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 24) && h.Name == MidsummerDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 20) && h.Name == RestorationOfIndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 24) && h.Name == ChristmasEve);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == BoxingDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(49) && h.Name == WhitSunday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter EstoniaTest`
Expected: FAIL (compile error — `NationalHolidays`/namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Estonia;

/// <summary>
/// Provides Estonian national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 2, 24), IndependenceDay),
                new(easter.AddDays(-2), GoodFriday),
                new(easter, EasterSunday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(easter.AddDays(49), WhitSunday),
                new(new DateOnly(year, 6, 23), VictoryDay),
                new(new DateOnly(year, 6, 24), MidsummerDay),
                new(new DateOnly(year, 8, 20), RestorationOfIndependenceDay),
                new(new DateOnly(year, 12, 24), ChristmasEve),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), BoxingDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter EstoniaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Estonia/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/EstoniaTest.cs
git commit -m "feat: add Estonia national holidays"
```

---

## Task 3: Add Iceland national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Iceland/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/IcelandTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.
- Produces: the "first weekday on/after a date" and "first weekday of a month" helper patterns used inline here — Iceland is the only country in this tier needing them, so they are written directly in this file rather than shared.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Iceland;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class IcelandTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 17) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == BoxingDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-3) && h.Name == MaundyThursday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == AscensionDay);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(49) && h.Name == WhitSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == WhitMonday);
    }

    [Fact]
    public void GetHolidays_ContainsFirstDayOfSummerAndCommerceDay()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        // In 2026, April 19 falls on a Sunday, so the first Thursday on/after
        // April 19 is April 23.
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 23) && h.Name == FirstDayOfSummer);
        // In 2026, August 1 falls on a Saturday, so the first Monday of
        // August is August 3.
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 3) && h.Name == CommerceDay);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter IcelandTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Iceland;

/// <summary>
/// Provides Icelandic national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(easter.AddDays(-3), MaundyThursday),
                new(easter.AddDays(-2), GoodFriday),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(FirstDayOfSummerDate(year), FirstDayOfSummer),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(easter.AddDays(39), AscensionDay),
                new(easter.AddDays(49), WhitSunday),
                new(easter.AddDays(50), WhitMonday),
                new(new DateOnly(year, 6, 17), IndependenceDay),
                new(CommerceDayDate(year), CommerceDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), BoxingDay),
            }
            .Order()
            .ToImmutableList();
    }

    private static DateOnly FirstDayOfSummerDate(int year)
    {
        var date = new DateOnly(year, 4, 19);
        while (date.DayOfWeek != DayOfWeek.Thursday)
            date = date.AddDays(1);

        return date;
    }

    private static DateOnly CommerceDayDate(int year)
    {
        var date = new DateOnly(year, 8, 1);
        while (date.DayOfWeek != DayOfWeek.Monday)
            date = date.AddDays(1);

        return date;
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter IcelandTest`
Expected: PASS, 4 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Iceland/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/IcelandTest.cs
git commit -m "feat: add Iceland national holidays"
```

---

## Task 4: Add Luxembourg national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Luxembourg/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/LuxembourgTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Luxembourg;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class LuxembourgTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(10);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 23) && h.Name == NationalDayOfLuxembourg);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == AscensionDay);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == WhitMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LuxembourgTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Luxembourg;

/// <summary>
/// Provides Luxembourgish national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(easter.AddDays(39), AscensionDay),
                new(easter.AddDays(50), WhitMonday),
                new(new DateOnly(year, 6, 23), NationalDayOfLuxembourg),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LuxembourgTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Luxembourg/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/LuxembourgTest.cs
git commit -m "feat: add Luxembourg national holidays"
```

---

## Task 5: Add Malta national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Malta/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/MaltaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Malta;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class MaltaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 10) && h.Name == FeastOfStPaulsShipwreck);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == FeastOfStJoseph);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 31) && h.Name == FreedomDayOfMalta);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 7) && h.Name == SetteGiugno);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 29) && h.Name == StPeterAndPaul);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 8) && h.Name == OurLadyOfVictoriesDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 21) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == ImmaculateConception);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 13) && h.Name == RepublicDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter MaltaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Malta;

/// <summary>
/// Provides Maltese national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 2, 10), FeastOfStPaulsShipwreck),
                new(new DateOnly(year, 3, 19), FeastOfStJoseph),
                new(new DateOnly(year, 3, 31), FreedomDayOfMalta),
                new(easter.AddDays(-2), GoodFriday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 6, 7), SetteGiugno),
                new(new DateOnly(year, 6, 29), StPeterAndPaul),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 9, 8), OurLadyOfVictoriesDay),
                new(new DateOnly(year, 9, 21), IndependenceDay),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
                new(new DateOnly(year, 12, 13), RepublicDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter MaltaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Malta/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/MaltaTest.cs
git commit -m "feat: add Malta national holidays"
```

---

## Task 6: Add Cyprus national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Cyprus/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/CyprusTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.ChristianOrthodox.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Cyprus;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class CyprusTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(15);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 25) && h.Name == GreekIndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 1) && h.Name == NationalDayOfCyprus);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 1) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 28) && h.Name == OhiDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsOrthodoxMovableFeasts()
    {
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(-48) && h.Name == CleanMonday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(50) && h.Name == WhitMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter CyprusTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Cyprus;

/// <summary>
/// Provides Cypriot national public holidays. Movable feasts follow the Orthodox
/// Easter calculation.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var orthodoxEaster = EasterSundayCalculation.ChristianOrthodox.ForYear(year);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 1, 6), Epiphany),
                new(orthodoxEaster.AddDays(-48), CleanMonday),
                new(new DateOnly(year, 3, 25), GreekIndependenceDay),
                new(new DateOnly(year, 4, 1), NationalDayOfCyprus),
                new(orthodoxEaster.AddDays(-2), GoodFriday),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(orthodoxEaster.AddDays(50), WhitMonday),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 10, 1), IndependenceDay),
                new(new DateOnly(year, 10, 28), OhiDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter CyprusTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Cyprus/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/CyprusTest.cs
git commit -m "feat: add Cyprus national holidays"
```

---

## Task 7: Add Moldova national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Moldova/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/MoldovaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.ChristianOrthodox.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1 (including `OurLanguageDay`, introduced here).

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Moldova;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class MoldovaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(11);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 7) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 8) && h.Name == InternationalWomensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 9) && h.Name == VictoryDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 27) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 31) && h.Name == OurLanguageDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsOrthodoxMovableFeasts()
    {
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(9) && h.Name == MemorialDay);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter MoldovaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Moldova;

/// <summary>
/// Provides Moldovan national public holidays. Movable feasts follow the Orthodox
/// Easter calculation.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var orthodoxEaster = EasterSundayCalculation.ChristianOrthodox.ForYear(year);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(new DateOnly(year, 1, 7), ChristmasDay),
                new(new DateOnly(year, 3, 8), InternationalWomensDay),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(orthodoxEaster.AddDays(9), MemorialDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 9), VictoryDay),
                new(new DateOnly(year, 8, 27), IndependenceDay),
                new(new DateOnly(year, 8, 31), OurLanguageDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter MoldovaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Moldova/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/MoldovaTest.cs
git commit -m "feat: add Moldova national holidays"
```

---

## Task 8: Mark Tier E4 done in the spec checklist, flag Bosnia and Herzegovina, and run the full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1: Update the checklist**

In `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, change:

```
- Tier E4: Estonia, 🔴 Belarus, Iceland, Luxembourg, Malta, Cyprus, Moldova, Bosnia and Herzegovina
```

to:

```
- Done: ✅ Estonia, ✅ Iceland, ✅ Luxembourg, ✅ Malta, ✅ Cyprus, ✅ Moldova (Tier E4)
- Tier E4 deferred: 🔴 Belarus, 🔴 Bosnia and Herzegovina (entity-fragmented calendar with Islamic lunar-calendar holidays)
```

matching the exact style of the Tier E1/E2/E3 lines immediately above it.

- [ ] **Step 2: Run the full test suite**

Run: `cd TemporaLinq && dotnet test --framework net10.0`
Expected: All tests pass (previous total was 290; this tier adds 6 countries × 3 tests = 18 new tests, except Iceland has 4 tests instead of 3, so expect 290 + 19 = 309 passing, 0 failing).

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Tier E4 countries done in worldwide holidays checklist"
```
