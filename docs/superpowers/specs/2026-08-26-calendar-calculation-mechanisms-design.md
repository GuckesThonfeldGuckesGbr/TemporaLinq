# Calendar Calculation Mechanisms Design

## Context

The worldwide holidays checklist (`docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`)
flags a large number of countries 🔴 "needs `StaticHolidayEnumerable<T>`" — the assumption being
that any holiday whose date isn't Gregorian-fixed or Christian/Orthodox-Easter-based can only be
captured with a hand-maintained, per-year lookup table.

That assumption is wrong for most of the flagged countries. Several non-Gregorian calendar systems
are themselves deterministic, rule-based arithmetic (or framework-provided, table-backed but
zero-maintenance-for-us) calculations — the same kind of "formula, not a table" approach already
used for `EasterSundayCalculation`. This design adds the calculation building blocks for those
calendar systems and reclassifies the checklist accordingly. `StaticHolidayEnumerable<T>` is
**not** built by this design — seeoutcome below.

## Calendar systems surveyed

| Calendar | Nature | Source | Verdict |
|---|---|---|---|
| Hijri (Islamic lunar) | Deterministic tabular arithmetic (12 lunar months, 11-year leap cycle in 30 years) | `System.Globalization.HijriCalendar` | Computable |
| Hebrew (lunisolar) | Deterministic tabular arithmetic (19-year Metonic cycle, 7 leap months) | `System.Globalization.HebrewCalendar` | Computable |
| Persian solar (Iran's civil calendar) | Deterministic astronomical-approximation leap rule | `System.Globalization.PersianCalendar` | Computable |
| Chinese/Korean/Taiwanese lunisolar | Framework-embedded precomputed tables (accurate for a wide practical year range, e.g. ~1901–2100); not a closed-form formula, but zero maintenance burden for this codebase | `System.Globalization.ChineseLunisolarCalendar`, `KoreanLunisolarCalendar`, `TaiwanLunisolarCalendar` | Computable |
| Coptic Easter | Same Julian-calendar computus already used for Eastern Orthodox Easter | Existing `EasterSundayCalculation.ChristianOrthodox` | Already computable, no new code |
| Ethiopian calendar | Deterministic fixed-offset/leap-year arithmetic (structurally a shifted Julian calendar) | No .NET class — small custom formula | Computable, new code |
| Thai Buddhist lunar holy days, Balinese Saka (Nyepi), Nepali Bikram Sambat, Burmese/Khmer/Lao/Mongolian Buddhist calendars, Tamil/Hindu calendar (Deepavali), broader Hindu lunisolar calendar (Diwali, Holi, etc.) | Genuinely astronomical/regional-almanac-dependent; no accepted simple arithmetic formula, no .NET support | — | **Remains hard.** Deferred; a future `HinduCalendarCalculation` or similar is its own design when its tier comes up. `ThaiBuddhistCalendar`/similar .NET classes are civil year-offset calendars only (Gregorian + a constant), not the religious lunar calendar — do not use them for Buddhist holy days. |

**Note on accuracy:** Hijri, Hebrew, Persian, and the lunisolar calendars above are all
*approximations* actually used in civil software, but the real-world observed date some
governments announce (particularly Eid al-Fitr/al-Adha, which several countries confirm only via
moon-sighting the night before) can differ by ±1, rarely ±2, days from the tabular calculation.
Every country file built on these calculations documents this in its XML doc comment, the same
spirit as noting that `EasterSundayCalculation` is a formula, not a decree.

## API design

Each calculation lives in `TemporaLinq.Holidays` as a static class, mirroring
`EasterSundayCalculation`'s shape.

### HijriCalendarCalculation

Hijri years run ~354 days — shorter than the Gregorian year — so a fixed Hijri (month, day)
drifts backward through the Gregorian calendar over time and, roughly once every 33 years, falls
**twice** within the same Gregorian year (confirmed empirically: e.g. Gregorian 2008 contains two
occurrences of 1 Muharram, on 2008-01-09 and 2008-12-28). Because the Hijri year is shorter, not
longer, this drift only ever produces doubles, never a skipped (zero-occurrence) year — every
Gregorian year has at least one occurrence of any given Hijri (month, day). Because of this, the
API returns a sequence, not a single date:

```csharp
public static class HijriCalendarCalculation
{
    /// <summary>
    /// Returns the Gregorian date(s) on which the given Hijri month/day falls within the
    /// specified Gregorian year. Always at least one date; occasionally two, because a
    /// Hijri year (~354 days) is shorter than the Gregorian year and periodically drifts
    /// enough to repeat within one Gregorian year.
    /// </summary>
    public static IEnumerable<DateOnly> DatesInGregorianYear(int gregorianYear, int hijriMonth, int hijriDay);
}
```

Implementation approach: convert `DateOnly(gregorianYear, 1, 1)` and `DateOnly(gregorianYear, 12, 31)`
to Hijri year numbers via `HijriCalendar`, then for each Hijri year in that (inclusive) range,
convert `(hijriYear, hijriMonth, hijriDay)` back to Gregorian and keep the ones that fall within
`[gregorianYear-01-01, gregorianYear-12-31]`.

### HebrewCalendarCalculation, PersianCalendarCalculation

Both calendars are intercalated (leap months/days) specifically to stay aligned with the solar
year, so a given (month, day) occurs exactly once per Gregorian year in practice — the same
one-date-per-year shape as `EasterSundayCalculation`:

```csharp
public static class HebrewCalendarCalculation
{
    public static DateOnly DateInGregorianYear(int gregorianYear, int hebrewMonth, int hebrewDay);
}

public static class PersianCalendarCalculation
{
    public static DateOnly DateInGregorianYear(int gregorianYear, int persianMonth, int persianDay);
}
```

Implementation approach: find the relevant native-calendar year overlapping `gregorianYear` (via
the same "convert Jan 1 and Dec 31" technique) and convert forward; since these calendars don't
drift, exactly one converted date will land inside the target Gregorian year. Throw
`InvalidOperationException` if that invariant is ever violated (defensive; should not happen for
any real (month, day) combination in the supported year range).

### ChineseLunisolarCalendarCalculation (and Korean/Taiwanese siblings, added when their countries are implemented)

Same one-date-per-year shape as Hebrew/Persian, backed by `ChineseLunisolarCalendar`:

```csharp
public static class ChineseLunisolarCalendarCalculation
{
    public static DateOnly DateInGregorianYear(int gregorianYear, int lunisolarMonth, int lunisolarDay);
}
```

`KoreanLunisolarCalendarCalculation` and `TaiwanLunisolarCalendarCalculation` follow the identical
shape, backed by their respective .NET calendar classes. Only build the ones actually needed by a
country implementation task — don't pre-build Korean/Taiwanese ahead of the tiers that use them.

### EthiopianCalendarCalculation

No .NET class exists. The Ethiopian calendar is a fixed arithmetic offset from the Julian
calendar (13 months: 12 of 30 days plus a short 13th month of 5 or 6 days in a leap year, leap
year every 4th year with no century exception, similar to the Julian leap rule):

```csharp
public static class EthiopianCalendarCalculation
{
    public static DateOnly DateInGregorianYear(int gregorianYear, int ethiopianMonth, int ethiopianDay);
}
```

Implementation approach: compute via the well-documented Julian-day-number offset between the
Ethiopian and Gregorian calendars (Ethiopian New Year, 1 Meskerem, falls on September 11 in the
Gregorian calendar, or September 12 in the Gregorian year before a Gregorian leap year) — same
one-date-per-year shape.

## What this design does NOT do

- **No `StaticHolidayEnumerable<T>`.** Building a generic per-year-lookup-table mechanism
  speculatively, before a country that actually needs one is in scope, violates YAGNI — we don't
  yet know what shape such a table should take. When a tier is reached where a country's calendar
  is genuinely irreducible to formula (e.g. Thailand's Buddhist lunar holy days), design that
  mechanism then, informed by that country's actual requirements.
- **No Hindu/Buddhist/Tamil calendar calculation.** Out of scope; still 🔴, still deferred.
- **No re-litigating India as a whole.** India's central-government Gazetted holidays (3 fixed
  civil days + Hijri-based + Christian-Easter-based ones) become tractable with this design's
  building blocks and can ship as an ordinary country tier once reached. India's Hindu-calendar
  holidays and state-specific additions remain out of scope for this design.
- **No live moon-sighting or astronomical-observation lookups.** All calculations here are
  deterministic arithmetic/table-based approximations, not real-time astronomical data.

## Reclassification of the worldwide holidays checklist

The following countries move from 🔴 to plain (computable) status, to be implemented as ordinary
country tiers once this design's building blocks exist. `docs/superpowers/specs/2026-08-25-worldwide-holidays-design.md`
is updated in a companion commit to reflect this table directly in the checklist.

| Country | Calendar(s) needed | Notes |
|---|---|---|
| Albania | Hijri | Was Tier E5 deferred |
| Kosovo | Hijri | Was Tier E5 deferred |
| Bosnia and Herzegovina | Hijri | Entity-fragmentation (Federation vs. Republika Srpska holiday law) is a separate, non-calendar complexity that may still warrant deferral or a scoped-down "state-level only" treatment when its turn comes — was Tier E4 deferred |
| Turkey | Hijri | Tier AS1 |
| Israel | Hebrew | Tier AS1 |
| Iran | Persian (civil) + Hijri (Shia religious observances) | Tier AS3 |
| China | Chinese lunisolar | Tier AS1 |
| South Korea | Korean lunisolar | Not yet in a named tier |
| Taiwan | Taiwan lunisolar | Tier AS3 |
| Hong Kong | Chinese lunisolar | Tier AS3 |
| Vietnam | Chinese lunisolar (used as a close approximation; Vietnam's Tết can rarely fall a lunar month off from China's due to timezone-driven new-moon-adjacent differences — document as an approximation caveat) | Tier AS2 |
| Pakistan | Hijri | Tier AS2 |
| Bangladesh | Hijri (plus Hindu/Buddhist minority holidays, deferred) | Tier AS2 |
| Saudi Arabia | Hijri | Tier AS2 |
| UAE | Hijri | Tier AS2 |
| Qatar | Hijri | Tier AS3 |
| Kuwait | Hijri | Tier AS3 |
| Iraq | Hijri (Sunni/Shia moon-sighting authorities occasionally differ by a day — document as approximation caveat) | Tier AS3 |
| Uzbekistan and remaining Central Asia | Hijri | Tier AS4 |
| Nigeria | Hijri + existing Christian Easter calc | Tier AF1 |
| Egypt | Hijri + existing Coptic/Orthodox Easter calc | Tier AF1 |
| Morocco | Hijri | Tier AF1 |
| Ethiopia | Ethiopian calendar | Tier AF2 |
| India | Hijri + existing Christian Easter calc, for the **central Gazetted list only** | Tier AS1; Hindu-calendar holidays and state-specific days remain deferred |

**Remains 🔴, unchanged:** Haiti, Venezuela (unrelated — political/administrative reasons, not
calendar), Sri Lanka, Nepal, Myanmar, Cambodia, Laos, Mongolia, Indonesia (Nyepi/Vesak component —
its Hijri and Christian-Easter components are now computable, so Indonesia could ship a *partial*
holiday set the same way India does), Thailand, Malaysia and Singapore (their Hindu/Buddhist
calendar components remain hard, but their Hijri and Chinese-lunisolar components are now
computable — same partial-ship approach as India/Indonesia), remaining Tier AS4/AF2/OC2
low-priority entries not otherwise listed above.

## Testing

Each calculation class gets a test file at `TemporaLinq.Test/Holidays/<CalculationName>Test.cs`
asserting known reference conversions (e.g. a specific well-documented Hijri New Year → Gregorian
date pair) for a spread of years, plus — for `HijriCalendarCalculation` specifically — a test
covering a year where the same Hijri (month, day) occurs twice (e.g. Gregorian 2008 for 1
Muharram), to exercise the multi-occurrence branch.
