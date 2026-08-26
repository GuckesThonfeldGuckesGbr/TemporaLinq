# Hebrew/Persian Calendar Holidays: Israel and Iran Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add computable national public holidays for Israel (Hebrew calendar) and Iran (Persian civil calendar + Hijri Shia religious observances) to `TemporaLinq.Holidays`, following the exact pattern already used by every other country, and exercising the newly-merged `HebrewCalendarCalculation`, `PersianCalendarCalculation`, and `HijriCalendarCalculation` mechanisms for the first time.

**Architecture:** Each country gets a `NationalHolidays : HolidayEnumerable<NationalHolidays>` record at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs` (the `Asia` directory is new), computing a per-year `ImmutableList<Holiday>` (memoized via `[Cache]`). Israel uses `HebrewCalendarCalculation.DateInGregorianYear`; Iran uses `PersianCalendarCalculation.DateInGregorianYear` for civil-calendar dates and `HijriCalendarCalculation.DatesInGregorianYear` for Shia religious dates. Each country also gets a test file at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs`, targeting year 2026, following the existing per-country test pattern.

**Tech Stack:** C#/.NET (net8.0 + net10.0 multi-target), xUnit + FluentAssertions, `Memoizer`'s `[Cache]` attribute.

**Spec:** `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`, `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`

## Reference data verified for 2026

All values below were cross-checked against .NET's `HebrewCalendar`/`PersianCalendar`/`HijriCalendar` (via a throwaway console harness calling the three calculation types) and against independent web sources (Hebcal, Wikipedia, government-holiday aggregator sites). Sources are cited inline in code comments.

### Israel — Hebrew calendar (2026 = Hebrew year 5786, NOT a leap year; `HebrewCalendar().IsLeapYear(5786) == false`)

| Holiday | Hebrew month/day | 2026 Gregorian date | Verified against |
|---|---|---|---|
| Rosh Hashanah (day 1) | 1 Tishrei | 2026-09-12 (Sat) | Hebcal / search: Rosh Hashanah Fri-Sat 9/12-9/13 |
| Rosh Hashanah (day 2) | 2 Tishrei | 2026-09-13 (Sun) | as above |
| Yom Kippur | 10 Tishrei | 2026-09-21 (Mon) | search: Yom Kippur sunset 9/20 - nightfall 9/21 |
| Sukkot (first day) | 15 Tishrei | 2026-09-26 (Sat) | search: Sukkot Sat-Sun 9/26-9/27 |
| Shmini Atzeret / Simchat Torah (combined, Israeli practice) | 22 Tishrei | 2026-10-03 (Sat) | search: Simchat Torah Sat-Sun 10/3-10/4 |
| Passover (first day) | 15 Nisan | 2026-04-02 (Thu) | search: Passover Thu-Fri 4/2-4/3 |
| Passover (last/7th day) | 21 Nisan | 2026-04-08 (Wed) | search: Passover Wed-Thu 4/8-4/9 |
| Yom HaShoah (Holocaust Remembrance Day) | 27 Nisan | 2026-04-14 (Tue) | Hebcal/search: evening of Apr 13 to evening Apr 14 |
| Yom Ha'atzmaut (Independence Day) | 5 Iyar | 2026-04-22 (Wed) | Hebcal/search: begins sundown Apr 21, ends nightfall Apr 22 |
| Shavuot | 6 Sivan | 2026-05-22 (Fri) | search: Shavuot Fri-Sat 5/22-5/23 |

Hebrew month numbers used above (2026/5786, non-leap): Tishrei=1, Nisan=7, Iyar=8, Sivan=9. **Leap-year handling:** in a 13-month Hebrew leap year, Adar splits into Adar I (month 6) and Adar II (month 7), which shifts every month from Nisan onward up by one slot: Nisan=8, Iyar=9, Sivan=10. Tishrei-based holidays (Rosh Hashanah, Yom Kippur, Sukkot, Simchat Torah) are unaffected since Tishrei is always month 1 regardless of leap status (the leap month falls later in the Hebrew year, after Shevat and before Nisan). The implementation must compute `HebrewCalendar().IsLeapYear(...)` for the relevant Hebrew year every time `GetHolidaysFor(year)` runs — it must NOT hardcode month 7/8/9 as if always non-leap.

**Known limitation / simplification:** Yom Ha'atzmaut's official shifting rule (to avoid the holiday or its preceding Yom HaZikaron falling on/adjacent to Shabbat) is implemented exactly as researched and confirmed: if 5 Iyar falls on Friday or Saturday, the holiday moves to the preceding Thursday; if 5 Iyar falls on Monday, it moves to Tuesday. All other weekdays are unshifted. This is a genuinely simple, well-documented rule (in effect since 1951, amended in 2004) so it IS implemented, not deferred. Yom HaShoah has an analogous (less commonly documented) shift rule that is **not** implemented here — it is left unshifted, which is an accepted simplification for this task since 27 Nisan for 2026 doesn't require it and the rule is less consistently documented than Yom Ha'atzmaut's.

Yom HaZikaron (Memorial Day, the day before Independence Day) is intentionally excluded — it is a solemn day of remembrance, not a non-working public holiday, consistent with the task's scope list.

### Iran — Persian civil calendar dates (verified via `PersianCalendarCalculation`, cross-checked against search results)

| Holiday | Persian month/day | 2026 Gregorian date | Verified against |
|---|---|---|---|
| Nowruz (day 1) | 1 Farvardin | 2026-03-21 (Sat) | search: Nowruz Mar 20-21 |
| Nowruz (day 2) | 2 Farvardin | 2026-03-22 (Sun) | — |
| Nowruz (day 3) | 3 Farvardin | 2026-03-23 (Mon) | — |
| Nowruz (day 4) | 4 Farvardin | 2026-03-24 (Tue) | — |
| Islamic Republic Day | 12 Farvardin | 2026-04-01 (Wed) | search: Islamic Republic Day is April 1 |
| Nature's Day (Sizdah Bedar) | 13 Farvardin | 2026-04-02 (Thu) | search: Sizdah Bedar Apr 2 |
| Death of Khomeini | 14 Khordad | 2026-06-04 (Thu) | — |
| 15 Khordad uprising | 15 Khordad | 2026-06-05 (Fri) | search: Khordad National Uprising observed June 5, Friday |
| Islamic Revolution victory day | 22 Bahman | 2026-02-11 (Wed) | well-known fixed anniversary date |

### Iran — Hijri (Shia) religious observances (via `HijriCalendarCalculation`, tabular/arithmetic approximation — see that type's XML doc for the +/-1-2 day moon-sighting caveat)

| Holiday | Hijri month/day | 2026 Gregorian date | Verified against |
|---|---|---|---|
| Tasua | 9 Muharram | 2026-06-24 (Wed) | derived, one day before Ashura |
| Ashura | 10 Muharram | 2026-06-25 (Thu) | search: Day of Ashura is June 25, 2026 |
| Arba'een | 20 Safar | 2026-08-04 (Tue) | 40 days after Ashura |
| Mawlid al-Nabi (Shia practice, 17 Rabi' al-awwal — NOT the Sunni 12th) | 17 Rabi' al-awwal | 2026-08-30 (Sun) | Iran official calendar lists Rabi-ol-Aval 17 |
| Eid al-Fitr (day 1) | 1 Shawwal | 2026-03-19 (Thu) | search: expected Mar 19 or 20, moon-sighting dependent |
| Eid al-Fitr (day 2) | 2 Shawwal | 2026-03-20 (Fri) | Iran's official calendar treats Eid al-Fitr as a 2-day holiday |
| Eid al-Adha | 10 Dhu al-Hijjah | 2026-05-26 (Tue) | standard Hijri calc |
| Eid al-Ghadir | 18 Dhu al-Hijjah | 2026-06-03 (Wed) | standard Hijri calc |

Scope is deliberately limited to the holidays explicitly listed in the task (Eid al-Fitr, Eid al-Adha, Eid al-Ghadir, Tasua/Ashura, Arba'een, Mawlid) even though Iran's full official calendar includes several more Shia observance days (e.g. martyrdom of Imam Ali, birth of Imam Mahdi, etc.) — those are out of scope for this pass.

## Global Constraints

- Countries live at `TemporaLinq.Holidays/Asia/<Country>/NationalHolidays.cs`; tests at `TemporaLinq.Test/Holidays/Asia/<Country>Test.cs`. The `Asia` directory does not exist yet in either project and must be created.
- `dotnet build` and `dotnet test --framework net10.0` must pass after every task (net8.0 testhost is unavailable in this sandbox).
- New `HolidayNames` enum members are added once, up front (Task 1), then reused by both country tasks.
- After both countries are done, update the checklist in `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`'s Asia section to mark Israel (Tier AS1) and Iran (Tier AS3) done (✅), matching the exact style already used for other tiers.

---

## Task 1: Add new HolidayNames enum members

**Status: DONE.** (Completed and verified — `dotnet build` succeeds with the enum additions below, alphabetically inserted, following the exact convention of every prior tier.)

New members added: `Arbaeen`, `Ashura`, `DeathOfKhomeini`, `EidAlAdha`, `EidAlFitr`, `EidAlGhadir`, `IslamicRepublicDayOfIran`, `IslamicRevolutionDayOfIran`, `KhordadNationalUprisingDay`, `MawlidAlNabi`, `NaturesDayOfIran`, `Nowruz`, `Passover`, `RoshHashanah`, `Shavuot`, `SimchatTorah`, `Sukkot`, `Tasua`, `YomHaAtzmaut`, `YomHaShoah`, `YomKippur`.

- [x] **Step 1:** Insert all new members alphabetically into `TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs`, each with a `// Country` comment.
- [x] **Step 2:** `dotnet build` succeeds with 0 errors.
- [ ] **Step 3: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/HolidayNames.cs
git commit -m "feat: add HolidayNames values for Israel and Iran"
```

---

## Task 2: Add Israel national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Israel/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/IsraelTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>` base, `HebrewCalendarCalculation.DateInGregorianYear(int, int, int) -> DateOnly`, `System.Globalization.HebrewCalendar` (for `IsLeapYear`/`GetYear`, to pick the correct Nisan/Iyar/Sivan month numbers), `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test** at `TemporaLinq/TemporaLinq.Test/Holidays/Asia/IsraelTest.cs`:

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Israel;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class IsraelTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(10);
    }

    [Fact]
    public void GetHolidays_ContainsHebrewCalendarHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 12) && h.Name == RoshHashanah);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 13) && h.Name == RoshHashanah);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 21) && h.Name == YomKippur);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 9, 26) && h.Name == Sukkot);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 10, 3) && h.Name == SimchatTorah);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 2) && h.Name == Passover);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 8) && h.Name == Passover);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 14) && h.Name == YomHaShoah);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 22) && h.Name == YomHaAtzmaut);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 22) && h.Name == Shavuot);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter IsraelTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation** at `TemporaLinq/TemporaLinq.Holidays/Asia/Israel/NationalHolidays.cs`:

```csharp
using System.Collections.Immutable;
using System.Globalization;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Israel;

/// <summary>
/// Provides Israeli national public holidays, computed from the Hebrew lunisolar calendar
/// via <see cref="HebrewCalendarCalculation"/>. Tishrei-based holidays (Rosh Hashanah, Yom
/// Kippur, Sukkot, Simchat Torah) always use Hebrew month 1 regardless of leap status. Nisan,
/// Iyar, and Sivan-based holidays (Passover, Yom HaShoah, Yom Ha'atzmaut, Shavuot) shift up by
/// one month slot in a 13-month Hebrew leap year, because Adar splits into Adar I (month 6)
/// and Adar II (month 7) earlier in that same Hebrew year - so this type re-derives the
/// correct month numbers per year rather than hardcoding them.
///
/// Known simplification: Yom Ha'atzmaut's real-world shifting rule (5 Iyar moves to the
/// preceding Thursday if it falls on Friday or Saturday, or to the following Tuesday if it
/// falls on a Monday - in force since 1951/2004 to avoid Sabbath conflicts around Yom
/// HaZikaron/Yom Ha'atzmaut) IS implemented below. Yom HaShoah has an analogous but less
/// consistently documented shift rule that is NOT implemented - its date here is always the
/// unshifted 27 Nisan. Yom HaZikaron itself (a solemn memorial day, not a non-working public
/// holiday) is intentionally out of scope.
/// </summary>
public record NationalHolidays : HolidayEnumerable<NationalHolidays>
{
    private static readonly HebrewCalendar HebrewCal = new();

    protected override IEnumerable<Holiday> GetHolidaysForYear(int year)
        => GetHolidaysFor(year);

    [Cache]
    private static ImmutableList<Holiday> GetHolidaysFor(int year)
    {
        // The Hebrew year covering Nisan/Iyar/Sivan in this Gregorian year - use a date well
        // inside that stretch (April 1) to look up the relevant Hebrew year and its leap status.
        var isLeapYear = HebrewCal.IsLeapYear(HebrewCal.GetYear(new DateTime(year, 4, 1)));
        var nisan = isLeapYear ? 8 : 7;
        var iyar = isLeapYear ? 9 : 8;
        var sivan = isLeapYear ? 10 : 9;

        var yomHaAtzmaut = ShiftYomHaAtzmaut(HebrewCalendarCalculation.DateInGregorianYear(year, iyar, 5));

        return new List<Holiday>
            {
                new(HebrewCalendarCalculation.DateInGregorianYear(year, 1, 1), RoshHashanah),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, 1, 2), RoshHashanah),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, 1, 10), YomKippur),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, 1, 15), Sukkot),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, 1, 22), SimchatTorah),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, nisan, 15), Passover),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, nisan, 21), Passover),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, nisan, 27), YomHaShoah),
                new(yomHaAtzmaut, YomHaAtzmaut),
                new(HebrewCalendarCalculation.DateInGregorianYear(year, sivan, 6), Shavuot),
            }
            .Order()
            .ToImmutableList();
    }

    /// <summary>
    /// Applies Israel's Independence Day shifting rule: if 5 Iyar falls on Friday or
    /// Saturday, the holiday moves to the preceding Thursday; if it falls on a Monday, it
    /// moves to the following Tuesday. All other weekdays are unshifted.
    /// </summary>
    private static DateOnly ShiftYomHaAtzmaut(DateOnly fifthOfIyar)
        => fifthOfIyar.DayOfWeek switch
        {
            DayOfWeek.Friday => fifthOfIyar.AddDays(-1),
            DayOfWeek.Saturday => fifthOfIyar.AddDays(-2),
            DayOfWeek.Monday => fifthOfIyar.AddDays(1),
            _ => fifthOfIyar,
        };
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter IsraelTest`
Expected: PASS, 2 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Israel/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/IsraelTest.cs
git commit -m "feat: add Israel national holidays"
```

---

## Task 3: Add Iran national holidays

**Files:**
- Create: `TemporaLinq/TemporaLinq.Holidays/Asia/Iran/NationalHolidays.cs`
- Test: `TemporaLinq/TemporaLinq.Test/Holidays/Asia/IranTest.cs`

**Interfaces:**
- Consumes: `HolidayEnumerable<T>`, `PersianCalendarCalculation.DateInGregorianYear(int, int, int) -> DateOnly`, `HijriCalendarCalculation.DatesInGregorianYear(int, int, int) -> IEnumerable<DateOnly>`, `HolidayNames` members from Task 1.

- [ ] **Step 1: Write the failing test** at `TemporaLinq/TemporaLinq.Test/Holidays/Asia/IranTest.cs`:

```csharp
using FluentAssertions;
using TemporaLinq.Holidays;
using TemporaLinq.Holidays.Asia.Iran;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Test.Holidays.Asia;

public class IranTest
{
    [Fact]
    public void GetHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().HaveCount(17);
    }

    [Fact]
    public void GetHolidays_ContainsPersianCalendarHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 21) && h.Name == Nowruz);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 22) && h.Name == Nowruz);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 23) && h.Name == Nowruz);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 24) && h.Name == Nowruz);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 1) && h.Name == IslamicRepublicDayOfIran);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 4, 2) && h.Name == NaturesDayOfIran);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 4) && h.Name == DeathOfKhomeini);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 5) && h.Name == KhordadNationalUprisingDay);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 2, 11) && h.Name == IslamicRevolutionDayOfIran);
    }

    [Fact]
    public void GetHolidays_ContainsHijriShiaHolidays()
    {
        var holidays = NationalHolidays.Create().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));

        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 24) && h.Name == Tasua);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 25) && h.Name == Ashura);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 4) && h.Name == Arbaeen);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 8, 30) && h.Name == MawlidAlNabi);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 19) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 3, 20) && h.Name == EidAlFitr);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 5, 26) && h.Name == EidAlAdha);
        holidays.Should().Contain(h => h.Date == new DateOnly(2026, 6, 3) && h.Name == EidAlGhadir);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter IranTest`
Expected: FAIL (compile error — namespace does not exist yet).

- [ ] **Step 3: Write the implementation** at `TemporaLinq/TemporaLinq.Holidays/Asia/Iran/NationalHolidays.cs`:

```csharp
using System.Collections.Immutable;
using Memoizer;
using static TemporaLinq.Holidays.HolidayNames;

namespace TemporaLinq.Holidays.Asia.Iran;

/// <summary>
/// Provides Iranian national public holidays: Persian solar civil-calendar holidays (via
/// <see cref="PersianCalendarCalculation"/>) plus Hijri lunar-calendar Shia religious
/// observances (via <see cref="HijriCalendarCalculation"/>). Hijri dates use a tabular
/// (arithmetic) approximation that can differ from real-world moon-sighting-confirmed dates
/// by up to a day or two - see that type's XML doc. This is most relevant for Eid al-Fitr and
/// Eid al-Adha. Scope is limited to the holidays explicitly in scope for this implementation
/// pass; Iran's full official calendar includes several additional Shia observance days
/// (e.g. martyrdom of Imam Ali, birth of Imam Mahdi) that are not included here.
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
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 1), Nowruz),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 2), Nowruz),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 3), Nowruz),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 4), Nowruz),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 12), IslamicRepublicDayOfIran),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 1, 13), NaturesDayOfIran),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 3, 14), DeathOfKhomeini),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 3, 15), KhordadNationalUprisingDay),
            new(PersianCalendarCalculation.DateInGregorianYear(year, 11, 22), IslamicRevolutionDayOfIran),
        };

        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 9))
            holidays.Add(new(date, Tasua));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 1, 10))
            holidays.Add(new(date, Ashura));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 2, 20))
            holidays.Add(new(date, Arbaeen));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 3, 17))
            holidays.Add(new(date, MawlidAlNabi));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 1))
            holidays.Add(new(date, EidAlFitr));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 10, 2))
            holidays.Add(new(date, EidAlFitr));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 10))
            holidays.Add(new(date, EidAlAdha));
        foreach (var date in HijriCalendarCalculation.DatesInGregorianYear(year, 12, 18))
            holidays.Add(new(date, EidAlGhadir));

        return holidays.Order().ToImmutableList();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd TemporaLinq && dotnet test --framework net10.0 --filter IranTest`
Expected: PASS, 3 tests.

- [ ] **Step 5: Commit**

```bash
git add TemporaLinq/TemporaLinq.Holidays/Asia/Iran/NationalHolidays.cs TemporaLinq/TemporaLinq.Test/Holidays/Asia/IranTest.cs
git commit -m "feat: add Iran national holidays"
```

---

## Task 4: Mark Israel and Iran done in the spec checklist, and run the full suite

**Files:**
- Modify: `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`

- [ ] **Step 1: Update the checklist**

In the Asia section, mark Israel (Tier AS1) and Iran (Tier AS3) done with ✅, matching the exact style used elsewhere in the doc (e.g. the Europe tiers' "Done: ✅ ..." lines), leaving all other still-pending countries in that tier's line as-is.

- [ ] **Step 2: Run the full test suite**

Run: `cd TemporaLinq && dotnet test --framework net10.0`
Expected: All tests pass, 0 failing.

- [ ] **Step 3: Commit**

```bash
git add docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md
git commit -m "docs: mark Israel and Iran done in worldwide holidays checklist"
```
