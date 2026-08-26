# Tier E5 Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for seven Tier E5 European micro-states/countries (North Macedonia, Montenegro, Andorra, Monaco, San Marino, Liechtenstein, Vatican City) to `TemporaLinq.Holidays`, following the exact pattern already used by Tier E1–E4 countries.

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record at `TemporaLinq.Holidays/Europe/<Country>/NationalHolidays.cs`, computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`), using `EasterSundayCalculation.Christian` (Andorra, Monaco, San Marino, Liechtenstein, Vatican City) or `EasterSundayCalculation.ChristianOrthodox` (North Macedonia) for movable feasts, where used. Montenegro's official state calendar has no movable feast at all (purely fixed dates). New `HolidayNames` enum members are added once, up front, then reused by every country task. Each country also gets a test file at `TemporaLinq.Test/Holidays/Europe/<Country>Test.cs` following the existing `EstoniaTest`/`CyprusTest` pattern.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

## Global Constraints

- Albania and Kosovo are excluded from this tier. Both have large Muslim populations whose statutory public holidays include Eid al-Fitr and Eid al-Adha, computed from the Islamic lunar calendar — not expressible with the existing Easter-formula pattern. Task 9 updates the spec checklist to flag them 🔴, matching the treatment already given to Belarus and Bosnia and Herzegovina.
- North Macedonia is a mixed-religion state (~65% Orthodox Christian, ~33% Muslim) but its state-mandated calendar (the days closed for all citizens/institutions, as opposed to the optional additional non-working days each citizen may choose based on their own faith) is fixed-date-and-Orthodox-Easter-based — the same treatment already given to Bulgaria and Serbia in Tier E3. The optional per-faith additional days (including Islamic ones) are out of scope, consistent with how those two countries were handled.
- The remaining six countries are all computable via the existing formula-based pattern — no new base class needed.
- Countries live at `TemporaLinq.Holidays/Europe/<Country>/NationalHolidays.cs`; tests at `TemporaLinq.Test/Holidays/Europe/<Country>Test.cs`.
- Reuse existing `HolidayNames` enum members wherever the concept matches (broadening the `//` comment to list the additional country), per the established convention. Only add new enum members for genuinely new concepts.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox).
- After all seven countries are done, update the checklist in the spec doc (`docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`) marking Tier E5 done, matching the exact style of the Tier E1–E4 checklist lines, and flag Albania and Kosovo 🔴.

---

## Reference: full holiday list per country

### North Macedonia (`EasterSundayCalculation.ChristianOrthodox`) — 13 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 2 | `NewYearsDay` |
| Jan 7 | `ChristmasDay` (reuse — Orthodox Nativity) |
| orthodoxEaster | `EasterSunday` |
| orthodoxEaster + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| May 2 | `LabourDay` |
| May 24 | `SaintsCyrilAndMethodiusDay` (reuse) |
| Aug 2 | `IlindenDay` (new — Ilinden / St. Elijah's Day, 1903 uprising) |
| Sep 8 | `IndependenceDay` (reuse) |
| Oct 11 | `DayOfMacedonianUprising` (new — 1941 anti-fascist uprising) |
| Oct 23 | `RevolutionaryStruggleDayOfMacedonia` (new — founding of VMRO) |
| Dec 8 | `StClementOfOhridDay` (new — Saint Clement of Ohrid Day) |

### Montenegro (no movable feast — fixed dates only) — 7 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 2 | `NewYearsDay` |
| Jan 7 | `ChristmasDay` (reuse — Orthodox Nativity) |
| May 1 | `LabourDay` |
| May 2 | `LabourDay` |
| May 21 | `IndependenceDay` (reuse — 2006 independence referendum) |
| Jul 13 | `StatehoodDayOfMontenegro` (new) |

### Andorra (`EasterSundayCalculation.Christian`) — 12 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 6 | `Epiphany` |
| easter - 2 | `GoodFriday` |
| easter + 1 | `EasterMonday` |
| Mar 14 | `ConstitutionDayOfAndorra` (new) |
| May 1 | `LabourDay` |
| Aug 15 | `AssumptionDay` |
| Sep 8 | `OurLadyOfMeritxellDay` (new — National Day of Andorra) |
| Nov 1 | `AllSaintsDay` |
| Dec 8 | `ImmaculateConception` |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` (reuse) |

### Monaco (`EasterSundayCalculation.Christian`) — 12 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 27 | `SaintDevoteDay` (new — Monaco's patron saint) |
| easter + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| easter + 39 | `AscensionDay` |
| easter + 50 | `WhitMonday` |
| easter + 60 | `CorpusChristi` |
| Aug 15 | `AssumptionDay` |
| Nov 1 | `AllSaintsDay` |
| Nov 19 | `NationalDayOfMonaco` (new — Sovereign Prince's Day) |
| Dec 8 | `ImmaculateConception` |
| Dec 25 | `ChristmasDay` |

### San Marino (`EasterSundayCalculation.Christian`) — 18 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 6 | `Epiphany` |
| Feb 5 | `FeastOfSaintAgatha` (new — patron saint) |
| Mar 25 | `AnniversaryOfArengo` (new — first parliament, 1906) |
| easter + 1 | `EasterMonday` |
| Apr 1 | `InvestitureOfCaptainsRegent` (new — semi-annual installation) |
| May 1 | `LabourDay` |
| Jul 28 | `FallOfFascismDay` (new — 1943 liberation) |
| Aug 15 | `AssumptionDay` |
| Sep 3 | `FoundingOfTheRepublicDay` (new — Feast of San Marino) |
| Oct 1 | `InvestitureOfCaptainsRegent` (reuse — the other semi-annual installation) |
| Nov 1 | `AllSaintsDay` |
| Nov 2 | `AllSoulsDay` |
| Dec 8 | `ImmaculateConception` |
| Dec 24 | `ChristmasEve` (reuse) |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` (reuse) |
| Dec 31 | `NewYearsEve` (reuse) |

### Liechtenstein (`EasterSundayCalculation.Christian`) — 15 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 2 | `BerchtoldsDay` (new) |
| Jan 6 | `Epiphany` |
| Feb 2 | `CandlemasDay` (new) |
| easter + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| easter + 39 | `AscensionDay` |
| easter + 50 | `WhitMonday` |
| easter + 60 | `CorpusChristi` |
| Aug 15 | `AssumptionDay` (also National Day of Liechtenstein) |
| Sep 8 | `NativityOfMaryDay` (new) |
| Nov 1 | `AllSaintsDay` |
| Dec 8 | `ImmaculateConception` |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `StStephensDay` (reuse) |

### Vatican City (`EasterSundayCalculation.Christian`) — 12 holidays
| Date | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Jan 6 | `Epiphany` |
| Feb 11 | `FoundationOfVaticanCityDay` (new — 1929 Lateran Treaty) |
| Mar 19 | `FeastOfStJoseph` (reuse) |
| easter | `EasterSunday` |
| easter + 1 | `EasterMonday` |
| May 1 | `LabourDay` |
| Jun 29 | `StPeterAndPaul` (reuse) |
| Aug 15 | `AssumptionDay` |
| Nov 1 | `AllSaintsDay` |
| Dec 8 | `ImmaculateConception` |
| Dec 25 | `ChristmasDay` |

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

**Interfaces:**
- Produces: the enum members every later task's `NationalHolidays.cs` references by name (via `using static TemporaLinq.Holidays.HolidayNames;`).

- [ ] **Step 1: Edit the enum**

Open `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`. Insert these new members, keeping the existing alphabetical ordering of the enum (insert each new line in alphabetical position among the existing members — read the full current file first to find each exact slot):

```
    AnniversaryOfArengo, // San Marino
    BerchtoldsDay, // Liechtenstein
    CandlemasDay, // Liechtenstein
    ConstitutionDayOfAndorra, // Andorra
    DayOfMacedonianUprising, // North Macedonia
    FallOfFascismDay, // San Marino
    FeastOfSaintAgatha, // San Marino
    FoundationOfVaticanCityDay, // Vatican City
    FoundingOfTheRepublicDay, // San Marino
    IlindenDay, // North Macedonia
    InvestitureOfCaptainsRegent, // San Marino
    NationalDayOfMonaco, // Monaco
    NativityOfMaryDay, // Liechtenstein
    OurLadyOfMeritxellDay, // Andorra
    RevolutionaryStruggleDayOfMacedonia, // North Macedonia
    SaintDevoteDay, // Monaco
    StatehoodDayOfMontenegro, // Montenegro
    StClementOfOhridDay, // North Macedonia
```

Also broaden the `//` comments on these **existing** members to add the new countries reusing them:

```
    ChristmasEve, // Czech Republic, Estonia, San Marino
    FeastOfStJoseph, // Malta, Liechtenstein, Vatican City
    IndependenceDay, // USA, Ukraine, Finland, Bulgaria, Estonia, Iceland, Malta, Cyprus, Moldova, Montenegro, North Macedonia
    NewYearsEve, // Latvia, San Marino
    SaintsCyrilAndMethodiusDay, // Czech Republic, Bulgaria, Slovakia, North Macedonia
    StPeterAndPaul, // Italy, Malta, Vatican City
```

- [ ] **Step 2: Build to verify the enum compiles**

Run: `cd TemporaLinq && dotnet build`
Expected: `Build succeeded.` with 0 errors (pre-existing warnings unrelated to this change are fine).

- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for Tier E5 countries"
```

---

## Task 2: Add North Macedonia national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/NorthMacedonia/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/NorthMacedoniaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>` base (`TemporaLinq.Holidays`), `EasterSundayCalculation.ChristianOrthodox.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.NorthMacedonia;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class NorthMacedoniaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 7) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 2) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 24) && h.Name == SaintsCyrilAndMethodiusDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 2) && h.Name == IlindenDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 8) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 11) && h.Name == DayOfMacedonianUprising);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 23) && h.Name == RevolutionaryStruggleDayOfMacedonia);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == StClementOfOhridDay);
    }

    [Fact]
    public void GetHolidays_ContainsOrthodoxMovableFeasts()
    {
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(1) && h.Name == EasterMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter NorthMacedoniaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.NorthMacedonia;

/// <summary>
/// Provides the state-mandated national public holidays of North Macedonia.
/// Movable feasts follow the Orthodox Easter calculation. Additional
/// religion-specific non-working days each citizen may choose (including
/// Islamic ones) are out of scope.
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
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 2), LabourDay),
                new(new DateOnly(year, 5, 24), SaintsCyrilAndMethodiusDay),
                new(new DateOnly(year, 8, 2), IlindenDay),
                new(new DateOnly(year, 9, 8), IndependenceDay),
                new(new DateOnly(year, 10, 11), DayOfMacedonianUprising),
                new(new DateOnly(year, 10, 23), RevolutionaryStruggleDayOfMacedonia),
                new(new DateOnly(year, 12, 8), StClementOfOhridDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter NorthMacedoniaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/NorthMacedonia/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/NorthMacedoniaTest.cs
git commit -m "feat: add North Macedonia national holidays"
```

---

## Task 3: Add Montenegro national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Montenegro/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/MontenegroTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `HolidayNames` members from Task 1. No Easter calculation needed — Montenegro's state calendar is entirely fixed-date.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Montenegro;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class MontenegroTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(7);
    }

    [Fact]
    public void GetHolidays_ContainsAllFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 7) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 2) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 21) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 13) && h.Name == StatehoodDayOfMontenegro);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter MontenegroTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Montenegro;

/// <summary>
/// Provides Montenegrin national public holidays. All dates are fixed —
/// Montenegro's state calendar has no movable feast.
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
                new(new DateOnly(year, 1, 7), ChristmasDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 2), LabourDay),
                new(new DateOnly(year, 5, 21), IndependenceDay),
                new(new DateOnly(year, 7, 13), StatehoodDayOfMontenegro),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter MontenegroTest`
Expected: PASS, 2 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Montenegro/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/MontenegroTest.cs
git commit -m "feat: add Montenegro national holidays"
```

---

## Task 4: Add Andorra national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Andorra/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/AndorraTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Andorra;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class AndorraTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 14) && h.Name == ConstitutionDayOfAndorra);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 8) && h.Name == OurLadyOfMeritxellDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == ImmaculateConception);
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

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter AndorraTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Andorra;

/// <summary>
/// Provides Andorran national public holidays.
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
                new(easter.AddDays(-2), GoodFriday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 3, 14), ConstitutionDayOfAndorra),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 9, 8), OurLadyOfMeritxellDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter AndorraTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Andorra/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/AndorraTest.cs
git commit -m "feat: add Andorra national holidays"
```

---

## Task 5: Add Monaco national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Monaco/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/MonacoTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Monaco;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class MonacoTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 27) && h.Name == SaintDevoteDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 19) && h.Name == NationalDayOfMonaco);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == ImmaculateConception);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == AscensionDay);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == WhitMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(60) && h.Name == CorpusChristi);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter MonacoTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Monaco;

/// <summary>
/// Provides Monégasque national public holidays.
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
                new(new DateOnly(year, 1, 27), SaintDevoteDay),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(easter.AddDays(39), AscensionDay),
                new(easter.AddDays(50), WhitMonday),
                new(easter.AddDays(60), CorpusChristi),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 19), NationalDayOfMonaco),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter MonacoTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Monaco/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/MonacoTest.cs
git commit -m "feat: add Monaco national holidays"
```

---

## Task 6: Add San Marino national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/SanMarino/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/SanMarinoTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1. Note `InvestitureOfCaptainsRegent` is used twice (Apr 1 and Oct 1) — the same enum member for both installations of the Captains Regent each year.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.SanMarino;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class SanMarinoTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(18);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 5) && h.Name == FeastOfSaintAgatha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 25) && h.Name == AnniversaryOfArengo);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 1) && h.Name == InvestitureOfCaptainsRegent);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 28) && h.Name == FallOfFascismDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 3) && h.Name == FoundingOfTheRepublicDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 1) && h.Name == InvestitureOfCaptainsRegent);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 2) && h.Name == AllSoulsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == ImmaculateConception);
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

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SanMarinoTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.SanMarino;

/// <summary>
/// Provides Sammarinese national public holidays.
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
                new(new DateOnly(year, 2, 5), FeastOfSaintAgatha),
                new(new DateOnly(year, 3, 25), AnniversaryOfArengo),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 4, 1), InvestitureOfCaptainsRegent),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 7, 28), FallOfFascismDay),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 9, 3), FoundingOfTheRepublicDay),
                new(new DateOnly(year, 10, 1), InvestitureOfCaptainsRegent),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 2), AllSoulsDay),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
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

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter SanMarinoTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/SanMarino/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/SanMarinoTest.cs
git commit -m "feat: add San Marino national holidays"
```

---

## Task 7: Add Liechtenstein national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Liechtenstein/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/LiechtensteinTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Liechtenstein;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class LiechtensteinTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == BerchtoldsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 2) && h.Name == CandlemasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 8) && h.Name == NativityOfMaryDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == ImmaculateConception);
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
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(60) && h.Name == CorpusChristi);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LiechtensteinTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Liechtenstein;

/// <summary>
/// Provides Liechtensteiner national public holidays. August 15 is both
/// Assumption Day and the National Day of Liechtenstein.
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
                new(new DateOnly(year, 1, 2), BerchtoldsDay),
                new(new DateOnly(year, 1, 6), Epiphany),
                new(new DateOnly(year, 2, 2), CandlemasDay),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(easter.AddDays(39), AscensionDay),
                new(easter.AddDays(50), WhitMonday),
                new(easter.AddDays(60), CorpusChristi),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 9, 8), NativityOfMaryDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter LiechtensteinTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/Liechtenstein/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/LiechtensteinTest.cs
git commit -m "feat: add Liechtenstein national holidays"
```

---

## Task 8: Add Vatican City national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/VaticanCity/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/VaticanCityTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `EasterSundayCalculation.Christian.ForYear(int) -> DateOnly`, `HolidayNames` members from Task 1 (including `FeastOfStJoseph` and `StPeterAndPaul`, reused here with broadened comments).

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.VaticanCity;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class VaticanCityTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 11) && h.Name == FoundationOfVaticanCityDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == FeastOfStJoseph);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 29) && h.Name == StPeterAndPaul);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == ImmaculateConception);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
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

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter VaticanCityTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.VaticanCity;

/// <summary>
/// Provides Vatican City's national public holidays. Excludes the Anniversary
/// of the Pope's Election/Inauguration, since that date changes with each
/// papacy and is not a stable yearly formula.
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
                new(new DateOnly(year, 2, 11), FoundationOfVaticanCityDay),
                new(new DateOnly(year, 3, 19), FeastOfStJoseph),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 6, 29), StPeterAndPaul),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter VaticanCityTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Europe/VaticanCity/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Europe/VaticanCityTest.cs
git commit -m "feat: add Vatican City national holidays"
```

---

## Task 9: Mark Tier E5 done in the spec checklist, flag Albania and Kosovo, and run the full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1: Update the checklist**

In `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, change:

```
- Tier E5: North Macedonia, Albania, Montenegro, Andorra, Monaco, San Marino, Liechtenstein, Vatican City, Kosovo
```

to:

```
- Done: ✅ North Macedonia, ✅ Montenegro, ✅ Andorra, ✅ Monaco, ✅ San Marino, ✅ Liechtenstein, ✅ Vatican City (Tier E5)
- Tier E5 deferred: 🔴 Albania, 🔴 Kosovo (Islamic lunar-calendar holidays, Eid al-Fitr / Eid al-Adha)
```

matching the exact style of the Tier E1–E4 lines immediately above it.

- [ ] **Step 2: Run the full test suite**

Run: `cd TemporaLinq && dotnet test --framework net10.0`
Expected: All tests pass (previous total was 323; this tier adds 7 countries × 3 tests = 21 new tests, except Montenegro has 2 tests instead of 3, so expect 323 + 20 = 343 passing, 0 failing).

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Tier E5 countries done in worldwide holidays checklist"
```
