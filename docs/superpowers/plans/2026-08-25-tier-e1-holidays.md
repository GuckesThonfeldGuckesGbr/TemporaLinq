# Tier E1 Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add national (and, where well-documented and small in scope, regional) public holidays for the 8 Tier E1 countries — United Kingdom, Poland, Netherlands, Ukraine, Sweden, Switzerland, Belgium, Austria — to `TemporaLinq.Holidays`.

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record under `TemporaLinq.Holidays/Europe/<Country>/NationalHolidays.cs`, following the exact pattern already used by Germany/France/Italy/Spain: a `[Cache]`d static `GetHolidaysFor(int year)` returning an `ImmutableList<Holiday>`, using `EasterSundayCalculation.Christian` (or `.ChristianOrthodox` for Ukraine) for movable feasts, and the existing `Dates.Invariant().From(x).First(DayOfWeek.y)` combinator for "Nth weekday of month" rules. Where a country has small, well-known regional variants (UK: Scotland, Northern Ireland; Belgium: the three Communities), a `StateHolidays.cs` file adds records for just the *additional* region-specific holidays, matching the additive-only style already used by Germany/France/Italy/Spain (state files list only the extra days, not a full merged calendar — callers combine via `Merge()` themselves). Each `NationalHolidays`/`StateHolidays` file gets a matching test file under `TemporaLinq.Test/Holidays/Europe/`.

**Tech Stack:** C# / .NET 8 & .NET 10, xUnit + FluentAssertions (matching existing test files), `Memoizer.NETStandard` for `[Cache]`.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

## Global Constraints

- Follow the existing `HolidayEnumerable<T>` pattern exactly — no new abstractions. `StaticHolidayEnumerable<T>` (for 🔴 flagged countries) is explicitly out of scope for this plan; every Tier E1 country is formula-computable.
- Fixed-date holidays are modeled as literal dates with **no weekend/in-lieu substitution logic** — this matches existing precedent (Germany/France/Italy/Spain never shift Dec 25, Jan 1, etc. when they fall on a weekend). Do not add substitution logic.
- State/regional files are additive-only (list only the extra holidays beyond national), matching Germany's `StateHolidays.cs` — never re-list national holidays in a state file.
- Reuse existing `HolidayNames` enum values whenever the holiday concept and date match an existing entry (e.g. `AssumptionDay`, `LabourDay`, `ChristmasDay`, `StStephensDay`, `ArmisticeDay`, `LiberationDay`, `VictoryDay`, `IndependenceDay`) even though their doc comment currently mentions a different country — comments are non-authoritative labels, not constraints.
- All new country namespaces are `TemporaLinq.Holidays.Europe.<Country>` (e.g. `TemporaLinq.Holidays.Europe.UnitedKingdom`), matching the existing `Europe.Germany` / `Europe.France` style.
- Every movable-feast test must derive its expected date from `EasterSundayCalculation.Christian.ForYear(year)` (or `.ChristianOrthodox` for Ukraine) at test time — never hardcode an absolute calendar date for a movable feast, matching `GermanyTest.GetHolidays_ContainsVariableHolidays`.
- Test year is 2026 throughout (matching `GermanyTest`/`ItalyTest`), for which `EasterSundayCalculation.Christian.ForYear(2026)` returns April 5, 2026 — already verified by the existing `GermanyTest.CalculateEasterSunday_ReturnsCorrectDate` theory.
- Run `dotnet test` from `/Users/ctg/git/TemporaLinq/TemporaLinq` after every task.

---

## Task 1: Extend `HolidayNames` with Tier E1 holiday names

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

**Interfaces:**
- Produces: 18 new `HolidayNames` enum members consumed by Tasks 2–9: `BattleOfTheBoyneDay`, `BoxingDay`, `ConstitutionDayOfPoland`, `ConstitutionDayOfUkraine`, `DefendersDay`, `EarlyMayBankHoliday`, `FlemishCommunityDay`, `FrenchCommunityDay`, `GermanCommunityDay`, `KingsDayOfTheNetherlands`, `MidsummerDay`, `NationalDayOfAustria`, `NationalDayOfBelgium`, `NationalDayOfSweden`, `NationalDayOfSwitzerland`, `SecondJanuary`, `SpringBankHoliday`, `StAndrewsDay`, `StPatricksDay`, `SummerBankHoliday`.

- [ ] **Step 1: Add the new enum members**

Edit `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs` to insert the new members (keeping the file's existing loose alphabetical grouping):

```csharp
namespace TemporaLinq.Holidays;

public enum HolidayNames
{
    AllSaintsDay,
    ArmisticeDay, // French
    AscensionDay,
    AssumptionDay,
    AugsburgPeaceFestival, // Germany
    BastilleDay, // France
    BattleOfTheBoyneDay, // Northern Ireland
    BirthdayOfMartinLutherKingJr, // USA
    BirthdayOfGeorgeWashington, // USA
    BoxingDay, // UK, Ireland, Canada, Australia, NZ
    ChristmasDay,
    ColumbusDay, // USA
    ConstitutionDayOfPoland, // Poland
    ConstitutionDayOfUkraine, // Ukraine
    CorpusChristi,
    DayOfGermanUnity, // Germany
    DefendersDay, // Ukraine
    EarlyMayBankHoliday, // UK
    EasterMonday,
    EasterSunday,
    Epiphany,
    FeastOfStJanuarius, // Italy
    FeastOfStJohnTheBaptist, // Italy
    FeastOfStPetronius, // Italy
    FlemishCommunityDay, // Belgium
    FrenchCommunityDay, // Belgium
    GermanCommunityDay, // Belgium
    GoodFriday,
    ImmaculateConception,
    IndependenceDay, // USA, Ukraine
    InternationalWomensDay,
    Juneteenth, // USA
    KingsDayOfTheNetherlands, // Netherlands
    LabourDay,
    LiberationDay, // Italy, Netherlands
    MemorialDay, // USA
    MidsummerDay, // Sweden
    NationalDayOfAustria, // Austria
    NationalDayOfBelgium, // Belgium
    NationalDayOfSpain, // Spain
    NationalDayOfSweden, // Sweden
    NationalDayOfSwitzerland, // Switzerland
    NewYearsDay,
    ReformationDay, // Germany
    RepentanceAndPrayerDay, // Germany
    RepublicDay, // Italy
    SanMarco, // Italy
    SecondJanuary, // Scotland
    SpanishConstitutionDay, // Spain
    SpringBankHoliday, // UK
    StAmbrose, // Italy
    StAndrewsDay, // Scotland
    StPatricksDay, // Northern Ireland
    StPeterAndPaul, // Italy
    StStephensDay,
    SummerBankHoliday, // UK (England, Wales, Northern Ireland)
    ThanksgivingDay, // USA
    WhitMonday,
    WhitSunday,
    WorldChildrensDay,
    VeteransDay, // USA
    VictoryDay // France, Ukraine
}
```

- [ ] **Step 2: Build to verify it compiles**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet build`
Expected: Build succeeds with 0 errors.

- [ ] **Step 3: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for Tier E1 countries"
```

---

## Task 2: United Kingdom (National + Scotland + Northern Ireland)

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/UnitedKingdom/NationalHolidays.cs`
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/UnitedKingdom/StateHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/UnitedKingdomTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`, `Dates.Invariant()...First(DayOfWeek)` (all pre-existing).
- Produces: `TemporaLinq.Holidays.Europe.UnitedKingdom.NationalHolidays`, `.Scotland`, `.NorthernIreland` — each a `HolidayEnumerable<T>` for later composition via `Merge()`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.UnitedKingdom;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class UnitedKingdomTest
{
    [Fact]
    public void NationalHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(8);
    }

    [Fact]
    public void NationalHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 4) && h.Name == EarlyMayBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 25) && h.Name == SpringBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 31) && h.Name == SummerBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == BoxingDay);
    }

    [Fact]
    public void NationalHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }

    [Fact]
    public void Scotland_HasCorrectHolidays()
    {
        var holidays = Scotland.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(2);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == SecondJanuary);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 30) && h.Name == StAndrewsDay);
    }

    [Fact]
    public void NorthernIreland_HasCorrectHolidays()
    {
        var holidays = NorthernIreland.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(2);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 17) && h.Name == StPatricksDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 12) && h.Name == BattleOfTheBoyneDay);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter UnitedKingdomTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.UnitedKingdom` namespace / types not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.UnitedKingdom;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

/// <summary>
/// Provides United Kingdom national bank holidays common to England and Wales.
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
                new(easter.AddDays(1), EasterMonday),
                new(Dates.Invariant().From(new DateOnly(year, 5, 1)).First(DayOfWeek.Monday), EarlyMayBankHoliday),
                new(Dates.Invariant().From(new DateOnly(year, 5, 25)).First(DayOfWeek.Monday), SpringBankHoliday),
                new(Dates.Invariant().From(new DateOnly(year, 8, 25)).First(DayOfWeek.Monday), SummerBankHoliday),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), BoxingDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Write `StateHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.UnitedKingdom;

public record Scotland : HolidayEnumerable<Scotland>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => new List<Holiday>
            {
                new(new DateOnly(year, 1, 2), SecondJanuary),
                new(new DateOnly(year, 11, 30), StAndrewsDay),
            }
            .Order()
            .ToImmutableList();
}

public record NorthernIreland : HolidayEnumerable<NorthernIreland>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => new List<Holiday>
            {
                new(new DateOnly(year, 3, 17), StPatricksDay),
                new(new DateOnly(year, 7, 12), BattleOfTheBoyneDay),
            }
            .Order()
            .ToImmutableList();
}
```

- [ ] **Step 5: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter UnitedKingdomTest`
Expected: PASS (5 tests).

- [ ] **Step 6: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/UnitedKingdom TemporaLinq.Test/Holidays/Europe/UnitedKingdomTest.cs
git commit -m "feat: add United Kingdom national, Scotland and Northern Ireland holidays"
```

---

## Task 3: Poland

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Poland/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/PolandTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`.
- Produces: `TemporaLinq.Holidays.Europe.Poland.NationalHolidays`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Poland;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class PolandTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 3) && h.Name == ConstitutionDayOfPoland);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 11) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(49) && h.Name == WhitSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(60) && h.Name == CorpusChristi);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter PolandTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Poland` not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Poland;

/// <summary>
/// Provides Polish national public holidays.
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
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 3), ConstitutionDayOfPoland),
                new(easter.AddDays(49), WhitSunday),
                new(easter.AddDays(60), CorpusChristi),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 11), IndependenceDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter PolandTest`
Expected: PASS (3 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Poland TemporaLinq.Test/Holidays/Europe/PolandTest.cs
git commit -m "feat: add Poland national holidays"
```

---

## Task 4: Netherlands

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Netherlands/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/NetherlandsTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`.
- Produces: `TemporaLinq.Holidays.Europe.Netherlands.NationalHolidays`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Netherlands;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class NetherlandsTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 5) && h.Name == LiberationDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == AscensionDay);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(49) && h.Name == WhitSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == WhitMonday);
    }

    [Theory]
    [InlineData(2025, 4, 26)] // April 27, 2025 is a Sunday -> observed Saturday April 26
    [InlineData(2026, 4, 27)] // April 27, 2026 is a Monday -> observed on the day itself
    public void KingsDay_ShiftsToSaturday_WhenApril27IsSunday(int year, int expectedMonth, int expectedDay)
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(year, 1, 1)).To(new DateOnly(year, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(year, expectedMonth, expectedDay) && h.Name == KingsDayOfTheNetherlands);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter NetherlandsTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Netherlands` not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Netherlands;

/// <summary>
/// Provides Dutch national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        var kingsDayRaw = new DateOnly(year, 4, 27);
        var kingsDay = kingsDayRaw.DayOfWeek == DayOfWeek.Sunday ? kingsDayRaw.AddDays(-1) : kingsDayRaw;

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(kingsDay, KingsDayOfTheNetherlands),
                new(new DateOnly(year, 5, 5), LiberationDay),
                new(easter.AddDays(39), AscensionDay),
                new(easter.AddDays(49), WhitSunday),
                new(easter.AddDays(50), WhitMonday),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter NetherlandsTest`
Expected: PASS (4 tests, 6 cases counting the `[Theory]`).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Netherlands TemporaLinq.Test/Holidays/Europe/NetherlandsTest.cs
git commit -m "feat: add Netherlands national holidays"
```

---

## Task 5: Ukraine

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Ukraine/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/UkraineTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.ChristianOrthodox`.
- Produces: `TemporaLinq.Holidays.Europe.Ukraine.NationalHolidays`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Ukraine;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class UkraineTest
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

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 8) && h.Name == InternationalWomensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 8) && h.Name == VictoryDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 28) && h.Name == ConstitutionDayOfUkraine);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 24) && h.Name == IndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 1) && h.Name == DefendersDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsOrthodoxEaster()
    {
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter UkraineTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Ukraine` not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Ukraine;

/// <summary>
/// Provides Ukrainian national public holidays.
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
                new(new DateOnly(year, 3, 8), InternationalWomensDay),
                new(orthodoxEaster, EasterSunday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 8), VictoryDay),
                new(new DateOnly(year, 6, 28), ConstitutionDayOfUkraine),
                new(new DateOnly(year, 8, 24), IndependenceDay),
                new(new DateOnly(year, 10, 1), DefendersDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter UkraineTest`
Expected: PASS (3 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Ukraine TemporaLinq.Test/Holidays/Europe/UkraineTest.cs
git commit -m "feat: add Ukraine national holidays"
```

---

## Task 6: Sweden

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Sweden/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/SwedenTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`, `Dates.Invariant()...First(DayOfWeek)`.
- Produces: `TemporaLinq.Holidays.Europe.Sweden.NationalHolidays`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Sweden;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class SwedenTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 6) && h.Name == NationalDayOfSweden);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == AscensionDay);
    }

    [Fact]
    public void MidsummerDay_FallsOnSaturdayBetweenJune20And26()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31)).ToList();

        var midsummer = holidays.First(h => h.Name == MidsummerDay);

        midsummer.Date.DayOfWeek.Should().Be(DayOfWeek.Saturday);
        midsummer.Date.Day.Should().BeInRange(20, 26);
        midsummer.Date.Month.Should().Be(6);
    }

    [Fact]
    public void AllSaintsDay_FallsOnSaturdayBetweenOct31AndNov6()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31)).ToList();

        var allSaints = holidays.First(h => h.Name == AllSaintsDay);

        allSaints.Date.DayOfWeek.Should().Be(DayOfWeek.Saturday);
        (allSaints.Date.Month == 10 && allSaints.Date.Day == 31 || allSaints.Date.Month == 11 && allSaints.Date.Day <= 6)
            .Should().BeTrue();
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter SwedenTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Sweden` not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Sweden;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

/// <summary>
/// Provides Swedish national public holidays.
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
                new(easter, EasterSunday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(easter.AddDays(39), AscensionDay),
                new(new DateOnly(year, 6, 6), NationalDayOfSweden),
                new(Dates.Invariant().From(new DateOnly(year, 6, 20)).First(DayOfWeek.Saturday), MidsummerDay),
                new(Dates.Invariant().From(new DateOnly(year, 10, 31)).First(DayOfWeek.Saturday), AllSaintsDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter SwedenTest`
Expected: PASS (5 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Sweden TemporaLinq.Test/Holidays/Europe/SwedenTest.cs
git commit -m "feat: add Sweden national holidays"
```

---

## Task 7: Switzerland

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Switzerland/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/SwitzerlandTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`.
- Produces: `TemporaLinq.Holidays.Europe.Switzerland.NationalHolidays`.

**Note:** Switzerland's public holidays are constitutionally almost entirely cantonal (26 cantons); only August 1st is a federally mandated holiday. This task models the set of holidays observed in the large majority of cantons as `NationalHolidays`, matching the level of simplification already accepted for Spain/Italy regional variation. Full per-canton coverage is out of scope for this plan — call it out as a candidate for a future design if requested.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Switzerland;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class SwitzerlandTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(8);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 1) && h.Name == NationalDayOfSwitzerland);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == AscensionDay);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == WhitMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter SwitzerlandTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Switzerland` not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Switzerland;

/// <summary>
/// Provides the Swiss public holidays observed in the large majority of cantons.
/// Full per-canton coverage is out of scope; see the type-level remarks.
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
                new(easter.AddDays(1), EasterMonday),
                new(easter.AddDays(39), AscensionDay),
                new(easter.AddDays(50), WhitMonday),
                new(new DateOnly(year, 8, 1), NationalDayOfSwitzerland),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter SwitzerlandTest`
Expected: PASS (3 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Switzerland TemporaLinq.Test/Holidays/Europe/SwitzerlandTest.cs
git commit -m "feat: add Switzerland national holidays"
```

---

## Task 8: Belgium (National + 3 Communities)

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Belgium/NationalHolidays.cs`
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Belgium/StateHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/BelgiumTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`.
- Produces: `TemporaLinq.Holidays.Europe.Belgium.NationalHolidays`, `.FlemishCommunity`, `.FrenchCommunity`, `.GermanSpeakingCommunity`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Belgium;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class BelgiumTest
{
    [Fact]
    public void NationalHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(10);
    }

    [Fact]
    public void NationalHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 21) && h.Name == NationalDayOfBelgium);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 11) && h.Name == ArmisticeDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void NationalHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(39) && h.Name == AscensionDay);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(50) && h.Name == WhitMonday);
    }

    [Fact]
    public void FlemishCommunity_HasCorrectHoliday()
    {
        var holidays = FlemishCommunity.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 11) && h.Name == FlemishCommunityDay);
    }

    [Fact]
    public void FrenchCommunity_HasCorrectHoliday()
    {
        var holidays = FrenchCommunity.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 27) && h.Name == FrenchCommunityDay);
    }

    [Fact]
    public void GermanSpeakingCommunity_HasCorrectHoliday()
    {
        var holidays = GermanSpeakingCommunity.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(1);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 15) && h.Name == GermanCommunityDay);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter BelgiumTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Belgium` not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Belgium;

/// <summary>
/// Provides Belgian national public holidays.
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
                new(new DateOnly(year, 7, 21), NationalDayOfBelgium),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 11, 11), ArmisticeDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Write `StateHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Belgium;

public record FlemishCommunity : HolidayEnumerable<FlemishCommunity>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 7, 11), FlemishCommunityDay));
}

public record FrenchCommunity : HolidayEnumerable<FrenchCommunity>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 9, 27), FrenchCommunityDay));
}

public record GermanSpeakingCommunity : HolidayEnumerable<GermanSpeakingCommunity>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
        => ImmutableList.Create(new Holiday(new DateOnly(year, 11, 15), GermanCommunityDay));
}
```

- [ ] **Step 5: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter BelgiumTest`
Expected: PASS (6 tests).

- [ ] **Step 6: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Belgium TemporaLinq.Test/Holidays/Europe/BelgiumTest.cs
git commit -m "feat: add Belgium national and community holidays"
```

---

## Task 9: Austria

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Austria/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/AustriaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`.
- Produces: `TemporaLinq.Holidays.Europe.Austria.NationalHolidays`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Austria;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class AustriaTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 26) && h.Name == NationalDayOfAustria);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == ImmaculateConception);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
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

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter AustriaTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Austria` not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Austria;

/// <summary>
/// Provides Austrian national public holidays.
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
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(easter.AddDays(39), AscensionDay),
                new(easter.AddDays(50), WhitMonday),
                new(easter.AddDays(60), CorpusChristi),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 10, 26), NationalDayOfAustria),
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

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test --filter AustriaTest`
Expected: PASS (3 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Austria TemporaLinq.Test/Holidays/Europe/AustriaTest.cs
git commit -m "feat: add Austria national holidays"
```

---

## Task 10: Full suite verification and checklist update

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md` (mark Tier E1 done)

- [ ] **Step 1: Run the full test suite**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test`
Expected: PASS, 0 failures, including all pre-existing tests plus the new UnitedKingdom/Poland/Netherlands/Ukraine/Sweden/Switzerland/Belgium/Austria tests.

- [ ] **Step 2: Update the checklist in the spec**

Edit `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, in the Europe section, change:

```
- Tier E1: United Kingdom, Poland, Netherlands, Ukraine, Sweden, Switzerland, Belgium, Austria
```

to:

```
- Done: ✅ United Kingdom, ✅ Poland, ✅ Netherlands, ✅ Ukraine, ✅ Sweden, ✅ Switzerland, ✅ Belgium, ✅ Austria (Tier E1)
```

- [ ] **Step 3: Commit**

```bash
cd /Users/ctg/git/TemporaLinq
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Tier E1 countries done in worldwide holidays checklist"
```
