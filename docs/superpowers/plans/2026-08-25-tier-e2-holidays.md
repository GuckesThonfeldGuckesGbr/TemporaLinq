# Tier E2 Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add national public holidays for the 8 Tier E2 countries — Ireland, Denmark, Norway, Finland, Czech Republic, Romania, Portugal, Greece — to `TemporaLinq.Holidays`, continuing directly from Tier E1's completed pattern.

**Architecture:** Unchanged from Tier E1. Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record under `TemporaLinq.Holidays/Europe/<Country>/NationalHolidays.cs`, a `[Cache]`d static `GetHolidaysFor(int year)` returning `ImmutableList<Holiday>`, using `EasterSundayCalculation.Christian` (Ireland, Denmark, Norway, Finland, Czech Republic, Portugal) or `.ChristianOrthodox` (Romania, Greece — both are Orthodox-majority countries whose legal holidays follow the Orthodox Easter date) for movable feasts, and `Dates.Invariant().From(x).First(DayOfWeek.y)` for weekday-anchored rules (Ireland's bank holidays; Finland's Midsummer/All Saints, reusing the exact pattern Sweden already established in Tier E1). No country in this tier has a well-documented, small regional variant worth a `StateHolidays.cs` file (matching the precedent already set by Poland/Netherlands/Ukraine/Sweden/Switzerland/Austria in Tier E1, which also had none) — this tier is national-holidays-only. Each `NationalHolidays` file gets a matching test file under `TemporaLinq.Test/Holidays/Europe/`.

**Tech Stack:** C# / .NET 8 & .NET 10, xUnit + FluentAssertions, `Memoizer.NETStandard` for `[Cache]`.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

## Global Constraints

- Follow the existing `HolidayEnumerable<T>` pattern exactly — no new abstractions.
- Fixed-date holidays: no weekend/in-lieu substitution logic, EXCEPT Ireland's St. Brigid's Day, which has its own explicit, legally-defined shift rule (see Task 2) — that is the one sanctioned exception, matching how Tier E1 sanctioned exactly one exception (Netherlands' King's Day).
- No state/regional files in this tier — national holidays only, per the Architecture note above.
- Reuse existing `HolidayNames` enum values whenever the holiday concept and date match an existing entry, even though the doc comment may currently name a different country (e.g. `StAndrewsDay` for Romania's Nov 30 patron-saint holiday, `SecondJanuary` for Romania's Jan 2, `EarlyMayBankHoliday` for Ireland's May bank holiday, `RepublicDay` for Portugal's Oct 5, `VictoryDay` for Czech Republic's May 8, `MidsummerDay`/`AllSaintsDay` for Finland reusing Sweden's Saturday-anchored rule, `IndependenceDay` for Finland's Dec 6).
- Denmark's "Store Bededag" (General Prayer Day) was abolished as an official public holiday starting in 2024 — do NOT model it, and the Denmark implementation's doc comment must say so explicitly (this is a deliberate historical-accuracy decision, not an oversight — a reviewer unfamiliar with the 2024 law change might otherwise flag its absence).
- All new country namespaces are `TemporaLinq.Holidays.Europe.<Country>` (e.g. `TemporaLinq.Holidays.Europe.CzechRepublic`), matching Tier E1's style. "Czech Republic" becomes the single identifier `CzechRepublic` (no space).
- Every movable-feast test must derive its expected date from `EasterSundayCalculation.Christian.ForYear(year)` or `.ChristianOrthodox.ForYear(year)` at test time — never hardcode an absolute calendar date for a movable feast.
- Test year is 2026 throughout (matching Tier E1), for which `EasterSundayCalculation.Christian.ForYear(2026)` returns April 5, 2026 (already verified by `GermanyTest.CalculateEasterSunday_ReturnsCorrectDate`). St. Brigid's Day's `[Theory]` test in Task 2 additionally uses 2029 and 2030 to exercise its special-case branch — see that task for the verified weekdays.
- Run `dotnet test -f net10.0` from `/Users/ctg/git/TemporaLinq/TemporaLinq` after every task. Only the .NET 10 runtime is installed in this environment; the net8.0 target of this multi-targeted project will fail to launch with a framework-not-found error — that's a pre-existing environment condition, not something to fix.

---

## Task 1: Extend `HolidayNames` with Tier E2 holiday names

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

**Interfaces:**
- Produces: 22 new `HolidayNames` enum members consumed by Tasks 2–9: `AugustBankHoliday`, `ChildrensDay`, `ChristmasEve`, `CleanMonday`, `ConstitutionDayOfDenmark`, `ConstitutionDayOfNorway`, `CzechStatehoodDay`, `FreedomDayOfPortugal`, `GreekIndependenceDay`, `IndependentCzechoslovakStateDay`, `JanHusDay`, `JuneBankHoliday`, `MaundyThursday`, `NationalDayOfRomania`, `OctoberBankHoliday`, `OhiDay`, `PortugalDay`, `RestorationOfIndependenceDay`, `SaintsCyrilAndMethodiusDay`, `StBrigidsDay`, `StruggleForFreedomAndDemocracyDay`, `SynaxisOfStJohnTheBaptist`.
- Also updates existing comments on `EarlyMayBankHoliday`, `IndependenceDay`, `MidsummerDay`, `RepublicDay`, `SecondJanuary`, `StAndrewsDay`, `VictoryDay` to note the additional countries reusing them (comments only — no behavior change).

- [ ] **Step 1: Replace the enum body**

Edit `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs` so its body reads exactly:

```csharp
namespace TemporaLinq.Holidays;

public enum HolidayNames
{
    AllSaintsDay,
    ArmisticeDay, // French
    AscensionDay,
    AssumptionDay,
    AugsburgPeaceFestival, // Germany
    AugustBankHoliday, // Ireland
    BastilleDay, // France
    BattleOfTheBoyneDay, // Northern Ireland
    BirthdayOfMartinLutherKingJr, // USA
    BirthdayOfGeorgeWashington, // USA
    BoxingDay, // UK, Ireland, Canada, Australia, NZ
    ChildrensDay, // Romania
    ChristmasDay,
    ChristmasEve, // Czech Republic
    CleanMonday, // Greece
    ColumbusDay, // USA
    ConstitutionDayOfDenmark, // Denmark
    ConstitutionDayOfNorway, // Norway
    ConstitutionDayOfPoland, // Poland
    ConstitutionDayOfUkraine, // Ukraine
    CorpusChristi,
    CzechStatehoodDay, // Czech Republic
    DayOfGermanUnity, // Germany
    DefendersDay, // Ukraine
    EarlyMayBankHoliday, // UK, Ireland
    EasterMonday,
    EasterSunday,
    Epiphany,
    FeastOfStJanuarius, // Italy
    FeastOfStJohnTheBaptist, // Italy
    FeastOfStPetronius, // Italy
    FlemishCommunityDay, // Belgium
    FreedomDayOfPortugal, // Portugal
    FrenchCommunityDay, // Belgium
    GermanCommunityDay, // Belgium
    GoodFriday,
    GreekIndependenceDay, // Greece
    ImmaculateConception,
    IndependenceDay, // USA, Ukraine, Finland
    IndependentCzechoslovakStateDay, // Czech Republic
    InternationalWomensDay,
    JanHusDay, // Czech Republic
    JuneBankHoliday, // Ireland
    Juneteenth, // USA
    KingsDayOfTheNetherlands, // Netherlands
    LabourDay,
    LiberationDay, // Italy, Netherlands
    MaundyThursday, // Denmark, Norway
    MemorialDay, // USA
    MidsummerDay, // Sweden, Finland
    NationalDayOfAustria, // Austria
    NationalDayOfBelgium, // Belgium
    NationalDayOfRomania, // Romania
    NationalDayOfSpain, // Spain
    NationalDayOfSweden, // Sweden
    NationalDayOfSwitzerland, // Switzerland
    NewYearsDay,
    OctoberBankHoliday, // Ireland
    OhiDay, // Greece
    PortugalDay, // Portugal
    ReformationDay, // Germany
    RepentanceAndPrayerDay, // Germany
    RepublicDay, // Italy, Portugal
    RestorationOfIndependenceDay, // Portugal
    SaintsCyrilAndMethodiusDay, // Czech Republic
    SanMarco, // Italy
    SecondJanuary, // Scotland, Romania
    SpanishConstitutionDay, // Spain
    SpringBankHoliday, // UK
    StAmbrose, // Italy
    StAndrewsDay, // Scotland, Romania
    StBrigidsDay, // Ireland
    StPatricksDay, // Northern Ireland
    StPeterAndPaul, // Italy
    StStephensDay,
    StruggleForFreedomAndDemocracyDay, // Czech Republic
    SummerBankHoliday, // UK (England, Wales, Northern Ireland)
    SynaxisOfStJohnTheBaptist, // Romania
    ThanksgivingDay, // USA
    WhitMonday,
    WhitSunday,
    WorldChildrensDay,
    VeteransDay, // USA
    VictoryDay // France, Ukraine, Czech Republic
}
```

- [ ] **Step 2: Build to verify it compiles**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet build`
Expected: Build succeeds with 0 errors.

- [ ] **Step 3: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for Tier E2 countries"
```

---

## Task 2: Ireland

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Ireland/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/IrelandTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`, `Dates.Invariant()...First(DayOfWeek)`.
- Produces: `TemporaLinq.Holidays.Europe.Ireland.NationalHolidays`.

**Verified weekday facts for the `[Theory]` test below** (Feb 1 weekday by year, computed leap-year-aware from a Jan 1 2026 = Thursday anchor): Feb 1 2026 = Sunday, Feb 1 2029 = Thursday, Feb 1 2030 = Friday. St. Brigid's Day's legal rule (Ireland, effective 2023): if Feb 1 falls on a Friday, the holiday IS Feb 1; otherwise, it's the first Monday of February.
- 2026: Feb 1 is Sunday → first Monday on/after = **Feb 2**.
- 2029: Feb 1 is Thursday → first Monday on/after = **Feb 5**.
- 2030: Feb 1 is Friday → the special case applies, so the holiday stays on **Feb 1** itself (without the special case, "first Monday on/after a Friday" would incorrectly give Feb 4 — this year is chosen specifically because it discriminates the special-case branch from the generic one).

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Ireland;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class IrelandTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 17) && h.Name == StPatricksDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 4) && h.Name == EarlyMayBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 1) && h.Name == JuneBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 3) && h.Name == AugustBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 26) && h.Name == OctoberBankHoliday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }

    [Theory]
    [InlineData(2026, 2, 2)]
    [InlineData(2029, 2, 5)]
    [InlineData(2030, 2, 1)]
    public void StBrigidsDay_FollowsSpecialFridayRule(int year, int expectedMonth, int expectedDay)
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(year, 1, 1)).To(new DateOnly(year, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(year, expectedMonth, expectedDay) && h.Name == StBrigidsDay);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter IrelandTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Ireland` namespace not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Ireland;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

/// <summary>
/// Provides Irish national public holidays.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        var february1 = new DateOnly(year, 2, 1);
        var stBrigidsDay = february1.DayOfWeek == DayOfWeek.Friday
            ? february1
            : Dates.Invariant().From(february1).First(DayOfWeek.Monday);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(stBrigidsDay, StBrigidsDay),
                new(new DateOnly(year, 3, 17), StPatricksDay),
                new(easter.AddDays(1), EasterMonday),
                new(Dates.Invariant().From(new DateOnly(year, 5, 1)).First(DayOfWeek.Monday), EarlyMayBankHoliday),
                new(Dates.Invariant().From(new DateOnly(year, 6, 1)).First(DayOfWeek.Monday), JuneBankHoliday),
                new(Dates.Invariant().From(new DateOnly(year, 8, 1)).First(DayOfWeek.Monday), AugustBankHoliday),
                new(Dates.Invariant().From(new DateOnly(year, 10, 25)).First(DayOfWeek.Monday), OctoberBankHoliday),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter IrelandTest`
Expected: PASS (4 test methods, 6 total cases counting the `[Theory]`).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Ireland TemporaLinq.Test/Holidays/Europe/IrelandTest.cs
git commit -m "feat: add Ireland national holidays"
```

---

## Task 3: Denmark

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Denmark/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/DenmarkTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`.
- Produces: `TemporaLinq.Holidays.Europe.Denmark.NationalHolidays`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Denmark;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class DenmarkTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 5) && h.Name == ConstitutionDayOfDenmark);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
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
    public void GetHolidays_DoesNotContainAbolishedGeneralPrayerDay()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().NotContain(h => h.Date == easter2026.AddDays(26));
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter DenmarkTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Denmark` namespace not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Denmark;

/// <summary>
/// Provides Danish national public holidays. Note: "Store Bededag" (General Prayer
/// Day) was abolished as an official public holiday in Denmark starting in 2024
/// and is deliberately not modeled here.
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
                new(easter.AddDays(39), AscensionDay),
                new(easter.AddDays(49), WhitSunday),
                new(easter.AddDays(50), WhitMonday),
                new(new DateOnly(year, 6, 5), ConstitutionDayOfDenmark),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter DenmarkTest`
Expected: PASS (4 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Denmark TemporaLinq.Test/Holidays/Europe/DenmarkTest.cs
git commit -m "feat: add Denmark national holidays"
```

---

## Task 4: Norway

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Norway/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/NorwayTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`.
- Produces: `TemporaLinq.Holidays.Europe.Norway.NationalHolidays`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Norway;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class NorwayTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 17) && h.Name == ConstitutionDayOfNorway);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
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
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter NorwayTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Norway` namespace not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Norway;

/// <summary>
/// Provides Norwegian national public holidays.
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
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 17), ConstitutionDayOfNorway),
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

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter NorwayTest`
Expected: PASS (3 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Norway TemporaLinq.Test/Holidays/Europe/NorwayTest.cs
git commit -m "feat: add Norway national holidays"
```

---

## Task 5: Finland

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Finland/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/FinlandTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1, plus pre-existing), `EasterSundayCalculation.Christian`, `Dates.Invariant()...First(DayOfWeek)`.
- Produces: `TemporaLinq.Holidays.Europe.Finland.NationalHolidays`.

No new `HolidayNames` values are needed for Finland — every holiday reuses an existing member (including `MidsummerDay` and `AllSaintsDay`, reusing Sweden's exact "first Saturday on/after a given date" pattern from Tier E1).

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Finland;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class FinlandTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 6) && h.Name == IndependenceDay);
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
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(49) && h.Name == WhitSunday);
    }

    [Fact]
    public void MidsummerDay_FallsOnSaturdayBetweenJune20And26()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31)).ToList();

        var midsummer = holidays.First(h => h.Name == MidsummerDay);

        midsummer.Date.DayOfWeek.Should().Be(DayOfWeek.Saturday);
        midsummer.Date.Should().BeOnOrAfter(new DateOnly(2026, 6, 20)).And.BeOnOrBefore(new DateOnly(2026, 6, 26));
    }

    [Fact]
    public void AllSaintsDay_FallsOnSaturdayBetweenOct31AndNov6()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31)).ToList();

        var allSaints = holidays.First(h => h.Name == AllSaintsDay);

        allSaints.Date.DayOfWeek.Should().Be(DayOfWeek.Saturday);
        allSaints.Date.Should().BeOnOrAfter(new DateOnly(2026, 10, 31)).And.BeOnOrBefore(new DateOnly(2026, 11, 6));
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter FinlandTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Finland` namespace not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Finland;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

/// <summary>
/// Provides Finnish national public holidays.
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
                new(easter.AddDays(49), WhitSunday),
                new(Dates.Invariant().From(new DateOnly(year, 6, 20)).First(DayOfWeek.Saturday), MidsummerDay),
                new(Dates.Invariant().From(new DateOnly(year, 10, 31)).First(DayOfWeek.Saturday), AllSaintsDay),
                new(new DateOnly(year, 12, 6), IndependenceDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter FinlandTest`
Expected: PASS (5 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Finland TemporaLinq.Test/Holidays/Europe/FinlandTest.cs
git commit -m "feat: add Finland national holidays"
```

---

## Task 6: Czech Republic

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/CzechRepublic/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/CzechRepublicTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`.
- Produces: `TemporaLinq.Holidays.Europe.CzechRepublic.NationalHolidays`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.CzechRepublic;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class CzechRepublicTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 8) && h.Name == VictoryDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 5) && h.Name == SaintsCyrilAndMethodiusDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 6) && h.Name == JanHusDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 28) && h.Name == CzechStatehoodDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 28) && h.Name == IndependentCzechoslovakStateDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 17) && h.Name == StruggleForFreedomAndDemocracyDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 24) && h.Name == ChristmasEve);
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
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter CzechRepublicTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.CzechRepublic` namespace not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.CzechRepublic;

/// <summary>
/// Provides Czech national public holidays.
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
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 5, 8), VictoryDay),
                new(new DateOnly(year, 7, 5), SaintsCyrilAndMethodiusDay),
                new(new DateOnly(year, 7, 6), JanHusDay),
                new(new DateOnly(year, 9, 28), CzechStatehoodDay),
                new(new DateOnly(year, 10, 28), IndependentCzechoslovakStateDay),
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

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter CzechRepublicTest`
Expected: PASS (3 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/CzechRepublic TemporaLinq.Test/Holidays/Europe/CzechRepublicTest.cs
git commit -m "feat: add Czech Republic national holidays"
```

---

## Task 7: Romania

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Romania/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/RomaniaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.ChristianOrthodox`.
- Produces: `TemporaLinq.Holidays.Europe.Romania.NationalHolidays`.

This is the tier's largest holiday set (16), reflecting Romanian labour law (Art. 139 of the Labour Code). Uses `.ChristianOrthodox`, not `.Christian` — the same distinction Ukraine required in Tier E1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Romania;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class RomaniaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(16);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 2) && h.Name == SecondJanuary);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 6) && h.Name == Epiphany);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 7) && h.Name == SynaxisOfStJohnTheBaptist);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 1) && h.Name == ChildrensDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 30) && h.Name == StAndrewsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 1) && h.Name == NationalDayOfRomania);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == StStephensDay);
    }

    [Fact]
    public void GetHolidays_ContainsOrthodoxMovableFeasts()
    {
        var orthodoxEaster2026 = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(1) && h.Name == EasterMonday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(49) && h.Name == WhitSunday);
        holidays.Should().Contain(h => h.Date == orthodoxEaster2026.AddDays(50) && h.Name == WhitMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter RomaniaTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Romania` namespace not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Romania;

/// <summary>
/// Provides Romanian national public holidays. Movable feasts follow the Orthodox
/// Easter calculation, per Romanian law.
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
                new(new DateOnly(year, 1, 2), SecondJanuary),
                new(new DateOnly(year, 1, 6), Epiphany),
                new(new DateOnly(year, 1, 7), SynaxisOfStJohnTheBaptist),
                new(orthodoxEaster.AddDays(-2), GoodFriday),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 6, 1), ChildrensDay),
                new(orthodoxEaster.AddDays(49), WhitSunday),
                new(orthodoxEaster.AddDays(50), WhitMonday),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 11, 30), StAndrewsDay),
                new(new DateOnly(year, 12, 1), NationalDayOfRomania),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter RomaniaTest`
Expected: PASS (3 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Romania TemporaLinq.Test/Holidays/Europe/RomaniaTest.cs
git commit -m "feat: add Romania national holidays"
```

---

## Task 8: Portugal

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Portugal/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/PortugalTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.Christian`.
- Produces: `TemporaLinq.Holidays.Europe.Portugal.NationalHolidays`.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Portugal;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class PortugalTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 25) && h.Name == FreedomDayOfPortugal);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 10) && h.Name == PortugalDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 5) && h.Name == RepublicDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 11, 1) && h.Name == AllSaintsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 1) && h.Name == RestorationOfIndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 8) && h.Name == ImmaculateConception);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
    }

    [Fact]
    public void GetHolidays_ContainsVariableHolidays()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026 && h.Name == EasterSunday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(60) && h.Name == CorpusChristi);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter PortugalTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Portugal` namespace not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Portugal;

/// <summary>
/// Provides Portuguese national public holidays.
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
                new(new DateOnly(year, 4, 25), FreedomDayOfPortugal),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 6, 10), PortugalDay),
                new(easter.AddDays(60), CorpusChristi),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 10, 5), RepublicDay),
                new(new DateOnly(year, 11, 1), AllSaintsDay),
                new(new DateOnly(year, 12, 1), RestorationOfIndependenceDay),
                new(new DateOnly(year, 12, 8), ImmaculateConception),
                new(new DateOnly(year, 12, 25), ChristmasDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter PortugalTest`
Expected: PASS (3 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Portugal TemporaLinq.Test/Holidays/Europe/PortugalTest.cs
git commit -m "feat: add Portugal national holidays"
```

---

## Task 9: Greece

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Europe/Greece/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Europe/GreeceTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `Holiday`, `HolidayNames` (Task 1), `EasterSundayCalculation.ChristianOrthodox`.
- Produces: `TemporaLinq.Holidays.Europe.Greece.NationalHolidays`.

Uses `.ChristianOrthodox`, like Romania in this tier and Ukraine in Tier E1.

- [ ] **Step 1: Write the failing test**

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Europe.Greece;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Europe;

public class GreeceTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 25) && h.Name == GreekIndependenceDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 15) && h.Name == AssumptionDay);
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

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter GreeceTest`
Expected: FAIL to compile — `TemporaLinq.Holidays.Europe.Greece` namespace not found.

- [ ] **Step 3: Write `NationalHolidays.cs`**

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Europe.Greece;

/// <summary>
/// Provides Greek national public holidays. Movable feasts follow the Orthodox
/// Easter calculation, per Greek practice.
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
                new(orthodoxEaster.AddDays(-2), GoodFriday),
                new(orthodoxEaster, EasterSunday),
                new(orthodoxEaster.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(orthodoxEaster.AddDays(50), WhitMonday),
                new(new DateOnly(year, 8, 15), AssumptionDay),
                new(new DateOnly(year, 10, 28), OhiDay),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), StStephensDay),
            }
            .Order()
            .ToImmutableList();
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0 --filter GreeceTest`
Expected: PASS (3 tests).

- [ ] **Step 5: Commit**

```bash
cd /Users/ctg/git/TemporaLinq/TemporaLinq
git add TemporaLinq.Holidays/Europe/Greece TemporaLinq.Test/Holidays/Europe/GreeceTest.cs
git commit -m "feat: add Greece national holidays"
```

---

## Task 10: Full suite verification and checklist update

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md` (mark Tier E2 done)

- [ ] **Step 1: Run the full test suite**

Run: `cd /Users/ctg/git/TemporaLinq/TemporaLinq && dotnet test -f net10.0`
Expected: PASS, 0 failures, including all pre-existing tests plus the new Ireland/Denmark/Norway/Finland/CzechRepublic/Romania/Portugal/Greece tests.

- [ ] **Step 2: Update the checklist in the spec**

Edit `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, in the Europe section, change:

```
- Tier E2: Ireland, Denmark, Norway, Finland, Czech Republic, Romania, Portugal, Greece
```

to:

```
- Done: ✅ Ireland, ✅ Denmark, ✅ Norway, ✅ Finland, ✅ Czech Republic, ✅ Romania, ✅ Portugal, ✅ Greece (Tier E2)
```

- [ ] **Step 3: Commit**

```bash
cd /Users/ctg/git/TemporaLinq
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Tier E2 countries done in worldwide holidays checklist"
```
