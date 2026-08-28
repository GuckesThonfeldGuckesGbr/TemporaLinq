# Calendar calculations

Every holiday that doesn't fall on a fixed Gregorian date, or a fixed offset
from Easter, is computed via one of the calculation classes below. All of
them are **closed-form calculations** (a formula, or a deterministic
framework-provided conversion) — this library deliberately avoids
hand-maintained per-year lookup tables (see [Known gaps](known-gaps.md) for
the handful of holidays where no such formula exists and a table would be the
only option).

Each section below states what the calculation covers, where it comes from,
and — this is the important part if you're relying on a specific date —
**how accurate it actually is**.

## Easter (`EasterSundayCalculation`)

```csharp
var easter = EasterSundayCalculation.Christian.ForYear(2026);
var orthodoxEaster = EasterSundayCalculation.ChristianOrthodox.ForYear(2026);
```

Computus arithmetic (Gregorian and Julian/revised-Julian variants). Exact —
no known-day uncertainty. Most of Europe/the Americas' movable feasts
(Good Friday, Easter Monday, Whit Monday, Carnival days, etc.) are expressed
as `easter.AddDays(n)`.

## Hijri (`HijriCalendarCalculation`)

Wraps `System.Globalization.HijriCalendar`'s tabular (arithmetic) Hijri
calendar.

> **Accuracy:** real-world governments/religious authorities often confirm
> Eid al-Fitr and Eid al-Adha only by literal moon-sighting the night before,
> which can differ from this tabular calculation by **±1, rarely ±2, days**.
> Treat Hijri-derived holiday dates as the tabular/most-likely date, not a
> guarantee of what any specific country announces that year.

A Hijri year (~354 days) is shorter than the Gregorian year, so a given
(month, day) can fall twice within one Gregorian year (never zero) —
`DatesInGregorianYear` returns an `IEnumerable<DateOnly>` for exactly this
reason.

## Hebrew (`HebrewCalendarCalculation`)

Wraps `System.Globalization.HebrewCalendar`. Deterministic and exact — the
Hebrew calendar's 19-year Metonic intercalation cycle (a 13th month, Adar II,
added 7 times per cycle) keeps it aligned with the solar year, so a given
(month, day) occurs exactly once per Gregorian year.

**Pitfall:** in a leap year, Adar splits into Adar I (month 6) and Adar II
(month 7), shifting Nisan and every later month up one slot. Check
`HebrewCalendar.IsLeapYear` before hard-coding a month number for a specific
year.

## Persian (`PersianCalendarCalculation`)

Wraps `System.Globalization.PersianCalendar` — Iran's solar civil calendar,
with its own leap-year rule. Deterministic and exact; a given (month, day)
occurs exactly once per Gregorian year, no leap-month shifting to worry about
(it's a solar calendar, not lunisolar).

## Chinese / Korean / Taiwanese lunisolar

`ChineseLunisolarCalendarCalculation`, `KoreanLunisolarCalendarCalculation`,
`TaiwanLunisolarCalendarCalculation` — thin wrappers around
`System.Globalization.{Chinese,Korean,Taiwan}LunisolarCalendar`. These are
framework-provided precomputed astronomical data, not a formula this project
maintains, but they behave identically to a closed-form calculation from a
caller's point of view: deterministic, zero maintenance burden.

> **Valid ranges:** Chinese/Korean 1901–2100 Gregorian; Taiwan 1912–2051
> Gregorian (Taiwan's calendar internally numbers years in the ROC/Minguo era
> — Gregorian year minus 1911 — always derive the native year via `GetYear`
> rather than assuming it equals the Gregorian year).

**Pitfall — leap months:** all three calendars insert a leap month in some
years. In a leap year, every month *after* the leap month is shifted up one
slot relative to an ordinary year. Always check `GetLeapMonth(year)` on the
target lunisolar year before hard-coding a raw month number — the same
(nominal) festival can land on a different `.NET` month number depending on
whether that year had a leap month before it.

## Ethiopian (`EthiopianCalendarCalculation`)

No `System.Globalization` support exists for this one, so it's a small custom
day-number-offset calculation (Ethiopian leap years, a multiple-of-4 rule,
add one intercalary day — the same shape as the Julian calendar's leap rule,
different epoch/phase). 13 months: twelve of 30 days, plus a 5-day (6 in a
leap year) 13th month, Pagume.

Verified against 36 independently-sourced reference (Ethiopian, Gregorian)
date pairs across 7 Gregorian years, covering both leap and non-leap
Ethiopian years and the Pagume-13 boundary — all 36 matched exactly.
Deterministic and exact.

## Southeast Asian Buddhist lunisolar (`TemporaLinq.Astronomy.SoutheastAsianBuddhistCalendar`)

Covers the Theravada Buddhist holy days shared (under various local names) by
Thailand, Myanmar, and Cambodia: Makha Bucha (full moon of lunar month 3),
Visakha Bucha/Vesak (month 6), Asalha Bucha (month 8).

This is a genuine astronomical calculation, not a framework wrapper: month 1
of each lunar year is re-anchored every year at the most recent new moon
on/before the preceding Gregorian year's December solstice (both computed via
Meeus formulas — see below). Re-anchoring annually means an earlier year's
leap-month insertion is automatically absorbed and never needs explicit
tracking for months 1–8.

> **Accuracy:** inherits the lunar-phase calculation's real-world accuracy
> (well under a minute for centuries around the present — see below). The
> month-numbering/anchoring scheme itself is this project's own synthesis
> (not a third-party-verified port), checked during design against
> independently-published reference dates for all three holidays across
> three separate years. As with Hijri, some countries' own civil/religious
> authorities may publish a date that diverges by around ±1 day from this
> calculation.

Laos, though it shares this Buddhist calendar culturally, has **no
statutory** Buddhist-calendar public holiday (per its 2013 Labour Law) — its
holidays are all fixed civil dates, so it doesn't use this calculation at
all. See [Country coverage](countries.md).

## Lunar phase primitives (`TemporaLinq.Astronomy.LunarPhaseCalculation`, `DecemberSolsticeCalculation`)

The building blocks behind the calendar above, and directly usable for
Sri Lanka's monthly full-moon Poya holidays:

```csharp
var fullMoons = LunarPhaseCalculation.FullMoonsInGregorianYear(2026); // ~12-13 per year
```

Meeus' truncated periodic-term series (*Astronomical Algorithms*, 2nd ed.,
ch. 49 for phases, ch. 27 for solstices/equinoxes) — a real orbital-mechanics
series, not a linear approximation.

> **Accuracy:** well under a minute of the true moment, for roughly ±200–300
> years around the present. Accuracy degrades slowly beyond that window
> because of growing uncertainty in ΔT (the gap between Terrestrial Time and
> Earth's actual, slightly irregular rotation) — this implementation doesn't
> apply a ΔT correction, since within the practical range above it's at most
> a few minutes, far smaller than the day-level granularity being computed.
> This is a genuine physical model that tracks the real orbit, not a curve
> fit that slips further away from reality every year — the error stays
> roughly constant (and small) across the supported range rather than
> accumulating.

## Mongolian / Tibetan (`MongolianCalendarCalculation`)

Computes Tsagaan Sar (Mongolian Lunar New Year, month 1) and Ikh Duichen
(Buddha Day, month 4 day 15) via a direct port of the Phugpa calendar
mathematics (Svante Janson, "Tibetan calendar mathematics," 2007/2014) using
Mongolia's own epoch/reference-time constants (the Tegus Buyantu system,
devised by the monk Ishbaljir in 1747 for Ulaanbaatar) — ported from the
MIT-licensed [`@hnw/date-tibetan`](https://github.com/hnw/date-tibetan)
library, which implements the same paper.

> **Accuracy:** verified against 9 independently-sourced real Tsagaan Sar
> dates spanning 2020–2027 (6 matched exactly, 3 were off by one day) and 3
> real Ikh Duichen dates spanning 2024–2026 (all 3 exact). **Treat the result
> as accurate to within ±1 day of the locally observed date** — the same
> caveat this library gives Hijri-derived dates, and for a similar reason:
> the underlying mean-longitude model is a faithful astronomical
> approximation, but the exact moment of a lunar event can fall right at a
> local calendar-day boundary, and which side of that boundary the
> observed civil holiday lands on isn't always perfectly predictable from
> the model alone.

This is a different (and, for Mongolia specifically, more accurate) system
than the Tibetan/Chinese calendars — Mongolian and Tibetan Losar dates
genuinely diverge in some years (e.g. 2025: Tsagaan Sar March 1 vs. Tibetan
Losar February 28), and Mongolian and Chinese Lunar New Year diverge in
others (e.g. 2026: February 18 vs. February 17) — so don't substitute one
for another.
