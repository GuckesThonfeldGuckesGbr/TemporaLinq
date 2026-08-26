# Lunisolar East Asia Holidays Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for China, Hong Kong, and Vietnam to `TemporaLinq.Holidays`, using the newly-merged `ChineseLunisolarCalendarCalculation.DateInGregorianYear(int gregorianYear, int lunisolarMonth, int lunisolarDay) -> DateOnly` for all lunisolar-anchored dates, following the exact `NationalHolidays : HolidayEnumerable<NationalHolidays>` pattern already used by every other country. South Korea and Taiwan are explicitly out of scope (different batch, need their own calendar calculation classes).

**Architecture:** Each country gets a `NationalHolidays` record at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs` (the `Asia` folder is new — created by Task 2), computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`). Movable feasts:
- Lunisolar dates go through `ChineseLunisolarCalendarCalculation.DateInGregorianYear`.
- Hong Kong's Easter-adjacent holidays use `EasterSundayCalculation.Christian`.
- Qingming/Ching Ming Festival is a **solar term**, not a lunisolar date — computed via a small documented arithmetic approximation (verified below), not `ChineseLunisolarCalendarCalculation`.

**Critical mechanism — the leap-month shift:** `ChineseLunisolarCalendar` numbers months 1–13 in a leap year, and every civil month *after* the leap month is shifted up by one slot (see the XML doc on `ChineseLunisolarCalendarCalculation`). Every holiday in this plan later than month 1 (Dragon Boat/Tuen Ng month 5, Buddha's Birthday month 4, Mid-Autumn month 8, Chung Yeung month 9, Hùng Kings month 3) needs this correction. Each country file adds a small private static helper:

```csharp
private static readonly ChineseLunisolarCalendar Calendar = new();

private static int EffectiveMonth(int gregorianYear, int civilMonth)
{
    var lunisolarYear = Calendar.GetYear(new DateTime(gregorianYear, 6, 1));
    var leapMonth = Calendar.GetLeapMonth(lunisolarYear);
    return leapMonth != 0 && leapMonth < civilMonth ? civilMonth + 1 : civilMonth;
}
```

`new DateTime(gregorianYear, 6, 1)` (June 1) is used purely to resolve which internal lunisolar year number is in effect — Chinese New Year always falls in Gregorian Jan 21–Feb 20, so June 1 (and every date through the following Chinese New Year) unambiguously belongs to the lunisolar year that started earlier that same Gregorian year, which is the same lunisolar year all of this plan's holidays (months 3–9) fall within. This has been verified against .NET's actual `ChineseLunisolarCalendar` output for 2023 (leap 3rd month; Mid-Autumn correctly resolves to Sept 29, 2023, the real-world date) and 2025 (leap 7th month), and against 2026 (no leap month) — see the "Verified reference dates" table below.

**No weekend/collision substitution rules are modeled.** Real-world Hong Kong law shifts a holiday to the next weekday when it falls on a Sunday (or, per a 2026-specific quirk, further shifts again when that substitute would collide with an already-designated holiday — Ching Ming 2026 actually falls on Sunday April 5 and is observed April 7, skipping April 6 because that is already Easter Monday). This is exactly the kind of per-year government adjustment the worldwide-holidays design explicitly excludes (the China Golden Week note makes the same call). Every date computed by this plan is the true underlying calendar/solar-term/lunisolar date, not the government's weekend-substituted observance date. This is documented in each file's XML doc comment.

**China's Golden Week/multi-day statutory extensions are explicitly out of scope**, per the task scope — China gets single-day anchors only (Chinese New Year, Qingming, Dragon Boat, Mid-Autumn), plus fixed civil holidays. Hong Kong and Vietnam's multi-day spans (Lunar New Year 3 days for HK, Tết 5 days for Vietnam) **are** modeled, since they are fixed-length statutory spans anchored to the lunisolar date, not variable weekend-driven extensions.

**Vietnam approximation caveat:** Vietnam's Tết is computed using `ChineseLunisolarCalendarCalculation`, which models China's lunisolar calendar (computed for UTC+8). Vietnam's own calendar is nominally computed for UTC+7 and can, in rare years, land a full lunar month off from China's when a new moon falls close to the day boundary between the two time zones. This is the same approximation the calendar-calculation-mechanisms design explicitly accepts for Vietnam. Documented in the XML doc comment.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute, `System.Globalization.ChineseLunisolarCalendar`.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`

## Global Constraints

- Countries live at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs`; tests at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs`. Both `Asia` directories are new.
- Reuse existing `HolidayNames` enum members wherever the concept matches (`LabourDay`, `NewYearsDay`, `ChristmasDay`, `BoxingDay`, `GoodFriday`, `EasterMonday`, `HolySaturday`). New members (`LunarNewYearsDay`, `QingmingFestival`, `DragonBoatFestival`, `MidAutumnFestival`, `NationalDayOfChina`, `BuddhasBirthday`, `HKSAREstablishmentDay`, `ChungYeungFestival`, `HungKingsCommemorationDay`, `ReunificationDay`, `NationalDayOfVietnam`) are shared across the countries that use them (e.g. `LunarNewYearsDay` covers China's Chinese New Year, Hong Kong's Lunar New Year, and Vietnam's Tết — all the same underlying holiday concept).
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox).
- All reference dates below were independently verified in 2026-08-26 via a throwaway console program directly exercising `System.Globalization.ChineseLunisolarCalendar` (the same BCL type `ChineseLunisolarCalendarCalculation` wraps) plus WebSearch cross-checks against published 2026 holiday calendars (China Briefing, Hong Kong Labour Department, Vietnam Briefing). Every computed value matched the independently-published real-world date.

## Verified reference dates (2026, unless noted)

| Concept | Lunisolar (month, day) | Effective month (leap-adjusted) | Computed date | Cross-check source |
|---|---|---|---|---|
| Chinese New Year / Lunar New Year / Tết day 1 | (1, 1) | 1 (never shifts) | **2026-02-17** | China Briefing / Vietnam Briefing: "Chinese New Year 2026: Feb 17" |
| Qingming/Ching Ming (solar term, not lunisolar) | — | — | **2026-04-05** | China-Briefing 2026 holiday schedule (Apr 4–6 span, center day Apr 5); Hong Kong sources confirm Apr 5, 2026 is a Sunday |
| Dragon Boat / Tuen Ng | (5, 5) | 5 | **2026-06-19** | China Briefing: "Dragon Boat Festival: June 19-21" |
| Mid-Autumn Festival | (8, 15) | 8 | **2026-09-25** | China Briefing: "Mid-Autumn Festival: September 25-27" |
| Mid-Autumn Festival, 2023 (leap 3rd month sanity check) | (8, 15) | 9 | **2023-09-29** | Well-documented real-world date; confirms the leap-month-shift helper |
| Buddha's Birthday | (4, 8) | 4 | **2026-05-24** | Hong Kong Labour Dept 2026 circular: actual date falls on a Sunday, substitute observed Monday May 25 — our computed May 24 is the true unshifted date |
| Chung Yeung Festival | (9, 9) | 9 | **2026-10-18** | Hong Kong Labour Dept 2026 circular: actual date falls on a Sunday, substitute observed Monday Oct 19 — our computed Oct 18 is the true unshifted date |
| Hùng Kings' Commemoration Day | (3, 10) | 3 | **2026-04-26** | Vietnam Briefing 2026 schedule: "Falls on a Sunday (April 26)" |
| Easter Sunday 2026 (for Hong Kong's Easter-adjacent holidays) | — | — | **2026-04-05** | Independently well-documented; Good Friday Apr 3, day following Good Friday Apr 4, Easter Monday Apr 6 |

---

## Reference: full holiday list per country

### China (no Easter, no leap check needed for month 1) — 7 holidays
| Date (2026) | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Feb 17 | `LunarNewYearsDay` |
| Apr 5 | `QingmingFestival` |
| May 1 | `LabourDay` |
| Jun 19 | `DragonBoatFestival` |
| Sep 25 | `MidAutumnFestival` |
| Oct 1 | `NationalDayOfChina` |

### Hong Kong (`EasterSundayCalculation.Christian`) — 17 holidays
| Date (2026) | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Feb 17 | `LunarNewYearsDay` |
| Feb 18 | `LunarNewYearsDay` |
| Feb 19 | `LunarNewYearsDay` |
| Apr 5 | `QingmingFestival` |
| easter - 2 (Apr 3) | `GoodFriday` |
| easter - 1 (Apr 4) | `HolySaturday` (day following Good Friday — same offset as Holy Saturday) |
| easter + 1 (Apr 6) | `EasterMonday` |
| May 1 | `LabourDay` |
| May 24 | `BuddhasBirthday` |
| Jun 19 | `DragonBoatFestival` |
| Jul 1 | `HKSAREstablishmentDay` |
| Sep 26 (Mid-Autumn + 1 day) | `MidAutumnFestival` |
| Oct 1 | `NationalDayOfChina` |
| Oct 18 | `ChungYeungFestival` |
| Dec 25 | `ChristmasDay` |
| Dec 26 | `BoxingDay` |

### Vietnam (no Easter) — 10 holidays
| Date (2026) | HolidayNames member |
|---|---|
| Jan 1 | `NewYearsDay` |
| Feb 16 (Tết Eve, day1 - 1) | `LunarNewYearsDay` |
| Feb 17 (Tết day 1) | `LunarNewYearsDay` |
| Feb 18 (Tết day 2) | `LunarNewYearsDay` |
| Feb 19 (Tết day 3) | `LunarNewYearsDay` |
| Feb 20 (Tết day 4) | `LunarNewYearsDay` |
| Apr 26 | `HungKingsCommemorationDay` |
| Apr 30 | `ReunificationDay` |
| May 1 | `LabourDay` |
| Sep 2 | `NationalDayOfVietnam` |

---

## Task 1: Add new HolidayNames enum members

**Files:**
- Modify: `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`

- [ ] **Step 1: Edit the enum**

Insert these new members in alphabetical position:

```
    BuddhasBirthday, // Hong Kong
    ChungYeungFestival, // Hong Kong
    DragonBoatFestival, // China, Hong Kong
    HKSAREstablishmentDay, // Hong Kong
    HungKingsCommemorationDay, // Vietnam
    LunarNewYearsDay, // China, Hong Kong, Vietnam
    MidAutumnFestival, // China, Hong Kong
    NationalDayOfChina, // China, Hong Kong
    NationalDayOfVietnam, // Vietnam
    QingmingFestival, // China, Hong Kong
    ReunificationDay, // Vietnam
```

Broaden the `//` comments on these existing members:

```
    BoxingDay, // UK, Canada, Australia, NZ, Estonia, Iceland, Cyprus, Hong Kong
    HolySaturday, // Bulgaria, Serbia, Hong Kong
```

- [ ] **Step 2: Build to verify the enum compiles**

Run: `cd TemporaLinq && dotnet build`
Expected: `Build succeeded.` with 0 errors.

- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for China, Hong Kong, and Vietnam"
```

---

## Task 2: Add China national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/China/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/ChinaTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `ChineseLunisolarCalendarCalculation.DateInGregorianYear`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test** at `TemporaLinq/TemporaLinq.Test/Holidays/Asia/ChinaTest.cs`:

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.China;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class ChinaTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(7);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 1) && h.Name == NationalDayOfChina);
    }

    [Fact]
    public void GetHolidays_ContainsLunisolarAndSolarTermHolidays()
    {
        // Reference dates independently verified 2026-08-26 against System.Globalization.ChineseLunisolarCalendar
        // and cross-checked against China-Briefing's published 2026 holiday schedule.
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 17) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 5) && h.Name == QingmingFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 19) && h.Name == DragonBoatFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 25) && h.Name == MidAutumnFestival);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter ChinaTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation** at `TemporaLinq/TemporaLinq.Holidays/Asia/China/NationalHolidays.cs`:

```csharp
using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.China;

/// <summary>
/// Provides Chinese national public holidays: Chinese New Year, Qingming Festival,
/// Dragon Boat Festival, and Mid-Autumn Festival, plus fixed civil holidays.
/// <para>
/// Only the core lunisolar-/solar-term-anchored date for each festival is modeled.
/// China's actual statutory calendar adds a multi-day "Golden Week" around several
/// of these (e.g. Spring Festival's eve + following days) plus government-announced
/// weekend "make-up workday" shifts published separately for each year — those are
/// non-formulaic, per-year administrative decisions, not calendar arithmetic, and
/// are out of scope here (see the worldwide-holidays design doc).
/// </para>
/// <para>
/// Qingming Festival is a solar term (not a lunisolar date), computed via a
/// well-documented arithmetic approximation for the 21st century (2001-2100):
/// <c>floor(Y * 0.2422 + 4.81) - floor(Y / 4)</c> gives the April day number, where
/// Y is the last two digits of the Gregorian year. Verified against known reference
/// dates (e.g. April 4, 2021; April 5, 2019 and 2026).
/// </para>
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var chineseNewYear = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 1);
        var qingming = QingmingFestivalDate(year);
        var dragonBoat = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 5), 5);
        var midAutumn = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 8), 15);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(chineseNewYear, LunarNewYearsDay),
                new(qingming, QingmingFestival),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(dragonBoat, DragonBoatFestival),
                new(midAutumn, MidAutumnFestival),
                new(new DateOnly(year, 10, 1), NationalDayOfChina),
            }
            .Order()
            .ToImmutableList();
    }

    private static DateOnly QingmingFestivalDate(int year)
    {
        var y = year % 100;
        var aprilDay = (int)Math.Floor(y * 0.2422 + 4.81) - y / 4;
        return new DateOnly(year, 4, aprilDay);
    }

    private static readonly ChineseLunisolarCalendar Calendar = new();

    private static int EffectiveMonth(int gregorianYear, int civilMonth)
    {
        var lunisolarYear = Calendar.GetYear(new DateTime(gregorianYear, 6, 1));
        var leapMonth = Calendar.GetLeapMonth(lunisolarYear);
        return leapMonth != 0 && leapMonth < civilMonth ? civilMonth + 1 : civilMonth;
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter ChinaTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/China/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/ChinaTest.cs
git commit -m "feat: add China national holidays"
```

---

## Task 3: Add Hong Kong national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/HongKong/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/HongKongTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `ChineseLunisolarCalendarCalculation.DateInGregorianYear`, `EasterSundayCalculation.Christian.ForYear`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**:

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.HongKong;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class HongKongTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(17);
    }

    [Fact]
    public void GetHolidays_ContainsFixedHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 1, 1) && h.Name == NewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 7, 1) && h.Name == HKSAREstablishmentDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 1) && h.Name == NationalDayOfChina);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 25) && h.Name == ChristmasDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 12, 26) && h.Name == BoxingDay);
    }

    [Fact]
    public void GetHolidays_ContainsLunarNewYearAndOtherLunisolarHolidays()
    {
        // Reference dates independently verified 2026-08-25/26 against
        // System.Globalization.ChineseLunisolarCalendar and cross-checked against the
        // Hong Kong Labour Department's published 2026 statutory holidays circular.
        // The true unshifted dates are used (no Sunday-substitution rule modeled) —
        // e.g. Buddha's Birthday and Chung Yeung both fall on a Sunday in 2026 and are
        // officially observed one day later, which this library does not model.
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 17) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 18) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 19) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 5) && h.Name == QingmingFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 24) && h.Name == BuddhasBirthday);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 19) && h.Name == DragonBoatFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 26) && h.Name == MidAutumnFestival);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 18) && h.Name == ChungYeungFestival);
    }

    [Fact]
    public void GetHolidays_ContainsMovableFeasts()
    {
        var easter2026 = EasterSundayCalculation.Christian.ForYear(2026);
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-2) && h.Name == GoodFriday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(-1) && h.Name == HolySaturday);
        holidays.Should().Contain(h => h.Date == easter2026.AddDays(1) && h.Name == EasterMonday);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter HongKongTest`
Expected: FAIL (compile error).

- [ ] **Step 3: Write the implementation**:

```csharp
using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.HongKong;

/// <summary>
/// Provides Hong Kong general (public) holidays.
/// <para>
/// Mid-Autumn Festival is observed the day *after* the lunisolar 8th-month-15th-day
/// date (most celebrations happen at night, so the following day is the rest day).
/// </para>
/// <para>
/// No Sunday-substitution rule is modeled: Hong Kong law moves a holiday to the next
/// weekday when it falls on a Sunday (occasionally shifting further still to avoid
/// colliding with an already-designated holiday, as with Ching Ming 2026, which
/// truly falls on Sunday April 5 but is officially observed April 7 to avoid Easter
/// Monday on April 6). This library always returns the true underlying calendar/
/// solar-term/lunisolar date, consistent with how movable feasts are treated
/// elsewhere in this library.
/// </para>
/// <para>
/// Qingming/Ching Ming Festival is a solar term, computed the same way as for China
/// (see <see cref="TemporaLinq.Holidays.Asia.China.NationalHolidays"/>).
/// </para>
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var easter = EasterSundayCalculation.Christian.ForYear(year);
        var lunarNewYear = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 1);
        var qingming = QingmingFestivalDate(year);
        var buddhasBirthday = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 4), 8);
        var dragonBoat = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 5), 5);
        var midAutumn = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 8), 15);
        var chungYeung = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 9), 9);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(lunarNewYear, LunarNewYearsDay),
                new(lunarNewYear.AddDays(1), LunarNewYearsDay),
                new(lunarNewYear.AddDays(2), LunarNewYearsDay),
                new(qingming, QingmingFestival),
                new(easter.AddDays(-2), GoodFriday),
                new(easter.AddDays(-1), HolySaturday),
                new(easter.AddDays(1), EasterMonday),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(buddhasBirthday, BuddhasBirthday),
                new(dragonBoat, DragonBoatFestival),
                new(new DateOnly(year, 7, 1), HKSAREstablishmentDay),
                new(midAutumn.AddDays(1), MidAutumnFestival),
                new(new DateOnly(year, 10, 1), NationalDayOfChina),
                new(chungYeung, ChungYeungFestival),
                new(new DateOnly(year, 12, 25), ChristmasDay),
                new(new DateOnly(year, 12, 26), BoxingDay),
            }
            .Order()
            .ToImmutableList();
    }

    private static DateOnly QingmingFestivalDate(int year)
    {
        var y = year % 100;
        var aprilDay = (int)Math.Floor(y * 0.2422 + 4.81) - y / 4;
        return new DateOnly(year, 4, aprilDay);
    }

    private static readonly ChineseLunisolarCalendar Calendar = new();

    private static int EffectiveMonth(int gregorianYear, int civilMonth)
    {
        var lunisolarYear = Calendar.GetYear(new DateTime(gregorianYear, 6, 1));
        var leapMonth = Calendar.GetLeapMonth(lunisolarYear);
        return leapMonth != 0 && leapMonth < civilMonth ? civilMonth + 1 : civilMonth;
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter HongKongTest`
Expected: PASS, 4 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/HongKong/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/HongKongTest.cs
git commit -m "feat: add Hong Kong national holidays"
```

---

## Task 4: Add Vietnam national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Vietnam/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/VietnamTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `ChineseLunisolarCalendarCalculation.DateInGregorianYear`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test**:

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Vietnam;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class VietnamTest
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
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 30) && h.Name == ReunificationDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 1) && h.Name == LabourDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 2) && h.Name == NationalDayOfVietnam);
    }

    [Fact]
    public void GetHolidays_ContainsLunisolarHolidays()
    {
        // Reference dates independently verified 2026-08-26 against
        // System.Globalization.ChineseLunisolarCalendar (used here as a documented
        // approximation for Vietnam's own lunisolar calendar) and cross-checked
        // against Vietnam-Briefing's published 2026 Tet and Hung Kings schedule.
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 16) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 17) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 18) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 19) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 20) && h.Name == LunarNewYearsDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 26) && h.Name == HungKingsCommemorationDay);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter VietnamTest`
Expected: FAIL (compile error).

- [ ] **Step 3: Write the implementation**:

```csharp
using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Vietnam;

/// <summary>
/// Provides Vietnamese national public holidays.
/// <para>
/// Tết (Lunar New Year) is modeled as the 5-day statutory span fixed by Vietnamese
/// labour law: New Year's Eve (the day before lunisolar month 1 day 1) plus the
/// first four days of the lunar new year. Government-announced weekend "bridge day"
/// extensions around this core span are a per-year administrative decision, not
/// calendar arithmetic, and are out of scope (see the worldwide-holidays design doc).
/// </para>
/// <para>
/// <b>Approximation caveat:</b> Tết and Hùng Kings' Commemoration Day are computed
/// via <see cref="ChineseLunisolarCalendarCalculation"/>, which models China's
/// lunisolar calendar (computed for UTC+8). Vietnam's own lunisolar calendar is
/// nominally computed for UTC+7; in rare years a new moon falling close to the day
/// boundary between the two time zones can cause Vietnam's calendar to land a full
/// lunar month off from China's. This is a documented, accepted approximation per
/// the calendar-calculation-mechanisms design.
/// </para>
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        var tetDay1 = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, 1, 1);
        var hungKings = ChineseLunisolarCalendarCalculation.DateInGregorianYear(year, EffectiveMonth(year, 3), 10);

        return new List<Holiday>
            {
                new(new DateOnly(year, 1, 1), NewYearsDay),
                new(tetDay1.AddDays(-1), LunarNewYearsDay),
                new(tetDay1, LunarNewYearsDay),
                new(tetDay1.AddDays(1), LunarNewYearsDay),
                new(tetDay1.AddDays(2), LunarNewYearsDay),
                new(tetDay1.AddDays(3), LunarNewYearsDay),
                new(hungKings, HungKingsCommemorationDay),
                new(new DateOnly(year, 4, 30), ReunificationDay),
                new(new DateOnly(year, 5, 1), LabourDay),
                new(new DateOnly(year, 9, 2), NationalDayOfVietnam),
            }
            .Order()
            .ToImmutableList();
    }

    private static readonly ChineseLunisolarCalendar Calendar = new();

    private static int EffectiveMonth(int gregorianYear, int civilMonth)
    {
        var lunisolarYear = Calendar.GetYear(new DateTime(gregorianYear, 6, 1));
        var leapMonth = Calendar.GetLeapMonth(lunisolarYear);
        return leapMonth != 0 && leapMonth < civilMonth ? civilMonth + 1 : civilMonth;
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter VietnamTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Vietnam/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/VietnamTest.cs
git commit -m "feat: add Vietnam national holidays"
```

---

## Task 5: Update the worldwide-holidays checklist and run the full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1: Update the checklist**

In the `### Asia` section, mark China and Hong Kong done within their existing tier lines, and move Vietnam out of Tier AS2's list into a "done" line, matching the style used elsewhere (e.g. Tier E-series "Done: ✅ ..." lines). Concretely, change:

```
- Tier AS1: India (...), Israel (...), Japan, China (Chinese-lunisolar-computable), South Korea (Korean-lunisolar-computable), Singapore (...), Turkey (...)
```
to note China done, e.g. add a line:
```
- Done: ✅ China (Tier AS1)
```
and remove "China (Chinese-lunisolar-computable)" from the Tier AS1 prose list (leaving the rest of that tier's entries as still-pending). Similarly for Tier AS2 (Vietnam) and Tier AS3 (Hong Kong):
```
- Done: ✅ Vietnam (Tier AS2)
- Done: ✅ Hong Kong (Tier AS3)
```
removing "Vietnam (Chinese-lunisolar-computable, approximate)" from AS2's prose list and "Hong Kong (Chinese-lunisolar-computable)" from AS3's prose list. Keep South Korea and Taiwan exactly where they are (untouched — different batch).

- [ ] **Step 2: Run the full test suite**

Run: `cd TemporaLinq && dotnet test --framework net10.0`
Expected: All tests pass, 0 failures. (Adds 3 (China) + 4 (Hong Kong) + 3 (Vietnam) = 10 new test methods to the prior total.)

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark China, Hong Kong, and Vietnam done in worldwide holidays checklist"
```
