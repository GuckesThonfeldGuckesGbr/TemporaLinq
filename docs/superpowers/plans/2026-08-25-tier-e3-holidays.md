# Tier E3 Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for the eight Tier E3 European countries (Hungary, Bulgaria, Serbia, Croatia, Slovakia, Slovenia, Lithuania, Latvia) to `TemporaLinq.Holidays`, following the exact pattern already used by Tier E1/E2 countries.

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record at `TemporaLinq.Holidays/Europe/<Country>/NationalHolidays.cs`, computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`), using `EasterSundayCalculation.Christian` (Catholic/Protestant countries: Hungary, Croatia, Slovakia, Slovenia, Lithuania, Latvia) or `EasterSundayCalculation.ChristianOrthodox` (Bulgaria, Serbia) for movable feasts. New `HolidayNames` enum members are added once, up front, then reused by every country task. Each country also gets a test file at `TemporaLinq.Test/Holidays/Europe/<Country>Test.cs` following the existing `RomaniaTest`/`CzechRepublicTest` pattern: one test asserting total holiday count for 2026, one asserting fixed-date holidays, one asserting movable-feast dates computed independently via `EasterSundayCalculation`.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

## Global Constraints

- All eight countries are computable via the existing formula-based pattern (no 🔴 flagged countries in this tier) — no new base class needed.
- Countries live at `TemporaLinq.Holidays/Europe/<Country>/NationalHolidays.cs`; tests at `TemporaLinq.Test/Holidays/Europe/<Country>Test.cs`.
- Reuse existing `HolidayNames` enum members wherever the concept matches (broadening the `//` comment to list the additional country), per the established convention (e.g. `SecondJanuary // Scotland, Romania`). Only add new enum members for genuinely new concepts.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox — see Task 1 for how to verify).
- After all eight countries are done, update the checklist in the spec doc (`docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`) marking Tier E3 done, matching the exact style of the Tier E1/E2 checklist lines.

---

## Reference: full holiday list per country

### Hungary (`EasterSundayCalculation.Christian`) — 13 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Mar 15 | `RevolutionDayOfHungary` (new) |
| easter - 2 | `GoodFriday` |
| easter | `EasterSunday` |
| easter + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| easter + 49 | `WhitSunday` |
| easter + 50 | `WhitMonday` |
| Aug 20 | `StateFoundationDayOfHungary` (new) |
| Oct 23 | `NationalDayOfHungary` (new) |
| Nov 1 | `AllSaintsDay` |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` |

### Bulgaria (`EasterSundayCalculation.ChristianOrthodox`) — 14 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Mar 3 | `LiberationDayOfBulgaria` (new) |
| orthodoxEaster - 2 | `GoodFriday` |
| orthodoxEaster - 1 | `HolySaturday` (new) |
| orthodoxEaster | `EasterSunday` |
| orthodoxEaster + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| May 6 | `StGeorgesDay` (new) |
| May 24 | `SaintsCyrilAndMethodiusDay` (reuse) |
| Sep 6 | `UnificationDayOfBulgaria` (new) |
| Sep 22 | `IndependenceDay` (reuse) |
| Dec 24 | `ChristmasEve` |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` |

### Serbia (`EasterSundayCalculation.ChristianOrthodox`) — 12 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 2 | `NewYearsDay` |
| Jan 7 | `ChristmasDay` (Orthodox Christmas) |
| Feb 15 | `StatehoodDayOfSerbia` (new) |
| Feb 16 | `StatehoodDayOfSerbia` |
| orthodoxEaster - 2 | `GoodFriday` |
| orthodoxEaster - 1 | `HolySaturday` (reuse) |
| orthodoxEaster | `EasterSunday` |
| orthodoxEaster + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| May 2 | `LabourDay` |
| Nov 11 | `ArmisticeDay` (reuse) |

### Croatia (`EasterSundayCalculation.Christian`) — 14 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 6 | `Epiphany` |
| easter | `EasterSunday` |
| easter + 1 | `EasterMonday` |
| easter + 60 | `CorpusChristi` |
| May 1 | `LabourDay` |
| May 30 | `StatehoodDayOfCroatia` (new) |
| Jun 22 | `AntiFascistStruggleDay` (new) |
| Aug 5 | `VictoryAndHomelandThanksgivingDay` (new) |
| Aug 15 | `AssumptionDay` |
| Nov 1 | `AllSaintsDay` |
| Nov 18 | `RemembranceDayOfCroatia` (new) |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` |

### Slovakia (`EasterSundayCalculation.Christian`) — 15 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `EstablishmentDayOfSlovakRepublic` (new) |
| Jan 6 | `Epiphany` |
| easter - 2 | `GoodFriday` |
| easter + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| May 8 | `VictoryDay` (reuse) |
| Jul 5 | `SaintsCyrilAndMethodiusDay` (reuse) |
| Aug 29 | `SlovakNationalUprisingDay` (new) |
| Sep 1 | `ConstitutionDayOfSlovakia` (new) |
| Sep 15 | `OurLadyOfSorrowsDay` (new) |
| Nov 1 | `AllSaintsDay` |
| Nov 17 | `StruggleForFreedomAndDemocracyDay` (reuse) |
| Dec 24 | `ChristmasEve` |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` |

### Slovenia (`EasterSundayCalculation.Christian`) — 14 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 2 | `SecondJanuary` (reuse) |
| Feb 8 | `PresernDay` (new) |
| easter | `EasterSunday` |
| easter + 1 | `EasterMonday` |
| Apr 27 | `DayOfUprisingAgainstOccupation` (new) |
| May 1 | `LabourDay` |
| May 2 | `LabourDay` |
| Jun 25 | `StatehoodDayOfSlovenia` (new) |
| Aug 15 | `AssumptionDay` |
| Oct 31 | `ReformationDay` (reuse) |
| Nov 1 | `AllSaintsDay` |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `IndependenceAndUnityDayOfSlovenia` (new) |

### Lithuania (`EasterSundayCalculation.Christian`) — 14 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Feb 16 | `RestorationOfStateDay` (new) |
| Mar 11 | `RestorationOfIndependenceDay` (reuse) |
| easter | `EasterSunday` |
| easter + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| Jun 24 | `MidsummerDay` (reuse) |
| Jul 6 | `StatehoodDayOfLithuania` (new) |
| Aug 15 | `AssumptionDay` |
| Nov 1 | `AllSaintsDay` |
| Nov 2 | `AllSoulsDay` (new) |
| Dec 24 | `ChristmasEve` |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` |

### Latvia (`EasterSundayCalculation.Christian`) — 13 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| easter - 2 | `GoodFriday` |
| easter | `EasterSunday` |
| easter + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| May 4 | `RestorationOfIndependenceDay` (reuse) |
| Jun 23 | `LigoDay` (new) |
| Jun 24 | `MidsummerDay` (reuse) |
| Nov 18 | `ProclamationDayOfLatvia` (new) |
| Dec 24 | `ChristmasEve` |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` |
| Dec 31 | `NewYearsEve` (new) |

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

**Interfaces:**
- Produces: the enum members every later task's `NationalHolidays.cs` references by name (via `using static TemporaLinq.Holidays.HolidayNames;`).

- [ ] **Step 1: Edit the enum**

Open `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`. Insert these new members, keeping the existing alphabetical ordering of the enum (insert each new line in alphabetical position among the existing members):

```
    AllSoulsDay,
    AntiFascistStruggleDay, // Croatia
    ConstitutionDayOfSlovakia, // Slovakia
    DayOfUprisingAgainstOccupation, // Slovenia
    EstablishmentDayOfSlovakRepublic, // Slovakia
    HolySaturday, // Bulgaria, Serbia
    IndependenceAndUnityDayOfSlovenia, // Slovenia
    LigoDay, // Latvia
    LiberationDayOfBulgaria, // Bulgaria
    NationalDayOfHungary, // Hungary
    NewYearsEve, // Latvia
    OurLadyOfSorrowsDay, // Slovakia
    PresernDay, // Slovenia
    ProclamationDayOfLatvia, // Latvia
    RemembranceDayOfCroatia, // Croatia
    RestorationOfStateDay, // Lithuania
    RevolutionDayOfHungary, // Hungary
    SlovakNationalUprisingDay, // Slovakia
    StateFoundationDayOfHungary, // Hungary
    StatehoodDayOfCroatia, // Croatia
    StatehoodDayOfLithuania, // Lithuania
    StatehoodDayOfSerbia, // Serbia
    StatehoodDayOfSlovenia, // Slovenia
    StGeorgesDay, // Bulgaria
    UnificationDayOfBulgaria, // Bulgaria
    VictoryAndHomelandThanksgivingDay, // Croatia
```

They must be inserted in their correct alphabetical slots (case-insensitive, matching existing style), not appended in a block — e.g. `AllSoulsDay` goes right after `AllSaintsDay`, `HolySaturday` goes between `GreekIndependenceDay` and `ImmaculateConception`, etc. Read the full current file first to find each slot.

Also broaden the `//` comments on these **existing** members to add the new countries reusing them:

```
    ArmisticeDay, // French, Serbian
    IndependenceDay, // USA, Ukraine, Finland, Bulgaria
    MidsummerDay, // Sweden, Finland, Lithuania, Latvia
    ReformationDay, // Germany, Slovenia
    RestorationOfIndependenceDay, // Portugal, Lithuania, Latvia
    SaintsCyrilAndMethodiusDay, // Czech Republic, Bulgaria, Slovakia
    SecondJanuary, // Scotland, Romania, Slovenia
    StruggleForFreedomAndDemocracyDay, // Czech Republic, Slovakia
    VictoryDay // France, Ukraine, Czech Republic, Slovakia
```

- [ ] **Step 2: Build to verify the enum compiles**

Run: `cd TemporaLinq && dotnet build`
Expected: `Build succeeded.` with 0 errors (pre-existing warnings unrelated to this change are fine).

- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for Tier E3 countries"
```

---

## Task 2: Add Hungary national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Hungary/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/HungaryTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>` base (`TemporaLinq.Holidays`), `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Hungary;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class HungaryTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 15) && h.Name == RevolutionDayOfHungary);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 20) && h.Name == StateFoundationDayOfHungary);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 23) && h.Name == NationalDayOfHungary);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(49) && h.Name == WhitSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == WhitMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter HungaryTest`
Expected: FAIL (compile error — `NationalHolidays`/namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Hungary;

/// <summary>
/// Provides Hungarian national public holidays.
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
                new(new DateOnly(year, 3, 15), RevolutionDayOfHungary),
                new(easter.AddDays(-2), GoodFriday),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(easter.AddDays(49), WhitSunday),
                new(easter.AddDays(50), WhitMonday),
                new(new DateOnly(year, 8, 20), StateFoundationDayOfHungary),
                new(new DateOnly(year, 10, 23), NationalDayOfHungary),
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

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter HungaryTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Hungary/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/HungaryTest.cs
git commit -m "feat: add Hungary national holidays"
```

---

## Task 3: Add Bulgaria national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Bulgaria/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/BulgariaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.ChristianOrthodox.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Bulgaria;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class BulgariaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 3) && h.Name == LiberationDayOfBulgaria);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 6) && h.Name == StGeorgesDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 24) && h.Name == SaintsCyrilAndMethodiusDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 6) && h.Name == UnificationDayOfBulgaria);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 22) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 24) && h.Name == ChristmasEve);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsOrthodoxMovableFeasts()
    {
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(-1) && h.Name == HolySaturday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(1) && h.Name == EasterMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter BulgariaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Bulgaria;

/// <summary>
/// Provides Bulgarian national public holidays. Movable feasts follow the Orthodox
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
                new(new DateOnly(year, 3, 3), LiberationDayOfBulgaria),
                new(orthodoxEaster.AddDays(-2), GoodFriday),
                new(orthodoxEaster.AddDays(-1), HolySaturday),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 6), StGeorgesDay),
                new(new DateOnly(year, 5, 24), SaintsCyrilAndMethodiusDay),
                new(new DateOnly(year, 9, 6), UnificationDayOfBulgaria),
                new(new DateOnly(year, 9, 22), IndependenceDay),
                new(new DateOnly(year, 12, 24), ChristmasEve),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter BulgariaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Bulgaria/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/BulgariaTest.cs
git commit -m "feat: add Bulgaria national holidays"
```

---

## Task 4: Add Serbia national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Serbia/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/SerbiaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.ChristianOrthodox.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1 (including `HolySaturday` introduced in Task 3 — it lives in the shared enum, not in Bulgaria's file, so this task can use it directly).

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Serbia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class SerbiaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 7) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 15) && h.Name == StatehoodDayOfSerbia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 16) && h.Name == StatehoodDayOfSerbia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 2) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 11) && h.Name == ArmisticeDay);
    }

    [Fact]
    public void GetHolidays_ContainsOrthodoxMovableFeasts()
    {
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(-1) && h.Name == HolySaturday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(1) && h.Name == EasterMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SerbiaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Serbia;

/// <summary>
/// Provides Serbian national public holidays. Movable feasts follow the Orthodox
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
                new(new DateOnly(year, 1, 2), NewYearsDay),
                new(new DateOnly(year, 1, 7), ChristmasDay),
                new(new DateOnly(year, 2, 15), StatehoodDayOfSerbia),
                new(new DateOnly(year, 2, 16), StatehoodDayOfSerbia),
                new(orthodoxEaster.AddDays(-2), GoodFriday),
                new(orthodoxEaster.AddDays(-1), HolySaturday),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 2), LabourDay),
                new(new DateOnly(year, 11, 11), ArmisticeDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SerbiaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Serbia/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/SerbiaTest.cs
git commit -m "feat: add Serbia national holidays"
```

---

## Task 5: Add Croatia national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Croatia/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/CroatiaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Croatia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class CroatiaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 30) && h.Name == StatehoodDayOfCroatia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 22) && h.Name == AntiFascistStruggleDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 5) && h.Name == VictoryAndHomelandThanksgivingDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 18) && h.Name == RemembranceDayOfCroatia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(60) && h.Name == CorpusChristi);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter CroatiaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Croatia;

/// <summary>
/// Provides Croatian national public holidays.
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
                new(new DateOnly(year, 1, 6), Epiphany),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(easter.AddDays(60), CorpusChristi),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 30), StatehoodDayOfCroatia),
                new(new DateOnly(year, 6, 22), AntiFascistStruggleDay),
                new(new DateOnly(year, 8, 5), VictoryAndHomelandThanksgivingDay),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 18), RemembranceDayOfCroatia),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter CroatiaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Croatia/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/CroatiaTest.cs
git commit -m "feat: add Croatia national holidays"
```

---

## Task 6: Add Slovakia national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Slovakia/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/SlovakiaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Slovakia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class SlovakiaTest
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

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == EstablishmentDayOfSlovakRepublic);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 8) && h.Name == VictoryDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 5) && h.Name == SaintsCyrilAndMethodiusDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 29) && h.Name == SlovakNationalUprisingDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 1) && h.Name == ConstitutionDayOfSlovakia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 15) && h.Name == OurLadyOfSorrowsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 17) && h.Name == StruggleForFreedomAndDemocracyDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 24) && h.Name == ChristmasEve);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SlovakiaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Slovakia;

/// <summary>
/// Provides Slovak national public holidays.
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
                new(new DateOnly(year, 1, 1), EstablishmentDayOfSlovakRepublic),
                new(new DateOnly(year, 1, 6), Epiphany),
                new(easter.AddDays(-2), GoodFriday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 8), VictoryDay),
                new(new DateOnly(year, 7, 5), SaintsCyrilAndMethodiusDay),
                new(new DateOnly(year, 8, 29), SlovakNationalUprisingDay),
                new(new DateOnly(year, 9, 1), ConstitutionDayOfSlovakia),
                new(new DateOnly(year, 9, 15), OurLadyOfSorrowsDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 17), StruggleForFreedomAndDemocracyDay),
                new(new DateOnly(year, 12, 24), ChristmasEve),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SlovakiaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Slovakia/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/SlovakiaTest.cs
git commit -m "feat: add Slovakia national holidays"
```

---

## Task 7: Add Slovenia national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Slovenia/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/SloveniaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Slovenia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class SloveniaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == SecondJanuary);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 8) && h.Name == PresernDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 27) && h.Name == DayOfUprisingAgainstOccupation);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 2) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 25) && h.Name == StatehoodDayOfSlovenia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 31) && h.Name == ReformationDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == IndependenceAndUnityDayOfSlovenia);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SloveniaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Slovenia;

/// <summary>
/// Provides Slovenian national public holidays.
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
                new(new DateOnly(year, 1, 2), SecondJanuary),
                new(new DateOnly(year, 2, 8), PresernDay),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 4, 27), DayOfUprisingAgainstOccupation),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 2), LabourDay),
                new(new DateOnly(year, 6, 25), StatehoodDayOfSlovenia),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 10, 31), ReformationDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), IndependenceAndUnityDayOfSlovenia),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SloveniaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Slovenia/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/SloveniaTest.cs
git commit -m "feat: add Slovenia national holidays"
```

---

## Task 8: Add Lithuania national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Lithuania/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/LithuaniaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Lithuania;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class LithuaniaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 16) && h.Name == RestorationOfStateDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 11) && h.Name == RestorationOfIndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 24) && h.Name == MidsummerDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 6) && h.Name == StatehoodDayOfLithuania);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 2) && h.Name == AllSoulsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 24) && h.Name == ChristmasEve);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LithuaniaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Lithuania;

/// <summary>
/// Provides Lithuanian national public holidays.
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
                new(new DateOnly(year, 2, 16), RestorationOfStateDay),
                new(new DateOnly(year, 3, 11), RestorationOfIndependenceDay),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 6, 24), MidsummerDay),
                new(new DateOnly(year, 7, 6), StatehoodDayOfLithuania),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 2), AllSoulsDay),
                new(new DateOnly(year, 12, 24), ChristmasEve),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LithuaniaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Lithuania/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/LithuaniaTest.cs
git commit -m "feat: add Lithuania national holidays"
```

---

## Task 9: Add Latvia national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Latvia/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/LatviaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Latvia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class LatviaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 4) && h.Name == RestorationOfIndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 23) && h.Name == LigoDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 24) && h.Name == MidsummerDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 18) && h.Name == ProclamationDayOfLatvia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 24) && h.Name == ChristmasEve);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 31) && h.Name == NewYearsEve);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LatviaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Latvia;

/// <summary>
/// Provides Latvian national public holidays.
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
                new(easter.AddDays(-2), GoodFriday),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 4), RestorationOfIndependenceDay),
                new(new DateOnly(year, 6, 23), LigoDay),
                new(new DateOnly(year, 6, 24), MidsummerDay),
                new(new DateOnly(year, 11, 18), ProclamationDayOfLatvia),
                new(new DateOnly(year, 12, 24), ChristmasEve),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
                new(new DateOnly(year, 12, 31), NewYearsEve),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LatviaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Latvia/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/LatviaTest.cs
git commit -m "feat: add Latvia national holidays"
```

---

## Task 10: Mark Tier E3 done in the spec checklist and run the full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1: Update the checklist**

In `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, change:

```
- Tier E3: Hungary, Bulgaria, Serbia, Croatia, Slovakia, Slovenia, Lithuania, Latvia
```

to:

```
- Done: ✅ Hungary, ✅ Bulgaria, ✅ Serbia, ✅ Croatia, ✅ Slovakia, ✅ Slovenia, ✅ Lithuania, ✅ Latvia (Tier E3)
```

matching the exact style of the Tier E1/E2 lines immediately above it.

- [ ] **Step 2: Run the full test suite**

Run: `cd TemporaLinq && dotnet test --framework net10.0`
Expected: All tests pass (previous total was 266; this tier adds 8 countries × 3 tests = 24 new tests, so expect 290 passing, 0 failing).

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Tier E3 countries done in worldwide holidays checklist"
```
