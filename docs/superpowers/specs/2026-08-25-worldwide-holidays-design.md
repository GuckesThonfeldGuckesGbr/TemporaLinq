# Worldwide Holiday Coverage — Design

## Goal

Extend `TemporaLinq.Holidays` from its current 5 countries (USA, Germany, France,
Italy, Spain) toward global coverage, prioritized by likely .NET/C# developer
population rather than raw population or GDP, and implemented in batches
(continent tiers) rather than as one unbounded effort.

## Ranking basis

There is no authoritative "C#/.NET usage by country" dataset. Ranking uses
proxies: Stack Overflow Developer Survey regional breakdowns, known
.NET/enterprise-outsourcing hub geography (notably Eastern Europe and India),
and general developer-population size. Treat tiers as a reasonable starting
order, adjustable as real usage/requests come in — not ground truth.

## Architecture

### Computable countries (existing pattern, unchanged)

Countries continue to live at
`TemporaLinq.Holidays/<Continent>/<Country>/{NationalHolidays,StateHolidays}.cs`,
subclassing `HolidayEnumerable<T>`, using the shared `HolidayNames` enum and
`EasterSundayCalculation` (or other movable-feast calculators) exactly as
Germany/France/Italy/Spain/USA do today. Tests live at
`TemporaLinq.Test/Holidays/<Continent>/<Country>Test.cs`.

### Non-Gregorian calendar countries (formula-based, not static tables)

**Superseded 2026-08-26** — see
`docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`.
The original plan below assumed non-Gregorian-calendar holidays are
unformulable and need a hand-maintained per-year lookup table. That assumption
was wrong for most cases: Hijri, Hebrew, Persian, and the Chinese/Korean/
Taiwanese lunisolar calendars are all deterministic, rule-based (or
framework-table-backed, zero-maintenance-for-us) calculations, the same kind
of "formula, not a table" building block as `EasterSundayCalculation`. The
companion design adds `HijriCalendarCalculation`, `HebrewCalendarCalculation`,
`PersianCalendarCalculation`, and lunisolar equivalents, and reclassifies the
checklist below accordingly. `StaticHolidayEnumerable<T>` (originally proposed
here) is **not built** — see that design's "What this design does NOT do"
section. A small number of calendars (Hindu lunisolar, Thai/Balinese/Burmese/
Khmer/Lao/Mongolian Buddhist calendars, Tamil calendar) remain genuinely
irreducible to formula and stay deferred; a static-table mechanism is
revisited only if a country actually requiring one is reached.

## Full checklist

Legend: ✅ done · 🔴 flagged hard (genuinely irreducible to a formula — see
`docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`
for the full reclassification) · plain = computable via the existing
formula-based pattern, including via the Hijri/Hebrew/Persian/lunisolar/
Ethiopian calendar calculations added by that design.

### Europe
- Done: ✅ Germany, ✅ France, ✅ Italy, ✅ Spain
- Done: ✅ United Kingdom, ✅ Poland, ✅ Netherlands, ✅ Ukraine, ✅ Sweden, ✅ Switzerland, ✅ Belgium, ✅ Austria (Tier E1)
- Done: ✅ Ireland, ✅ Denmark, ✅ Norway, ✅ Finland, ✅ Czech Republic, ✅ Romania, ✅ Portugal, ✅ Greece (Tier E2)
- Done: ✅ Hungary, ✅ Bulgaria, ✅ Serbia, ✅ Croatia, ✅ Slovakia, ✅ Slovenia, ✅ Lithuania, ✅ Latvia (Tier E3)
- Done: ✅ Estonia, ✅ Iceland, ✅ Luxembourg, ✅ Malta, ✅ Cyprus, ✅ Moldova (Tier E4)
- Tier E4 remaining: Belarus, Bosnia and Herzegovina (Hijri-computable; entity-fragmentation of holiday law across Federation/Republika Srpska is a separate non-calendar complexity — scope when implemented)
- Done: ✅ North Macedonia, ✅ Montenegro, ✅ Andorra, ✅ Monaco, ✅ San Marino, ✅ Liechtenstein, ✅ Vatican City (Tier E5)
- Tier E5 remaining: Albania, Kosovo (Hijri-computable)

### North America
- Done: ✅ USA
- Tier NA1: Canada, Mexico
- Tier NA2: Costa Rica, Panama, Guatemala, Dominican Republic, Jamaica, Cuba, Honduras, El Salvador
- Tier NA3: Nicaragua, Belize, Bahamas, Trinidad and Tobago, Barbados, 🔴 Haiti, remaining Caribbean micro-states

### South America
- Tier SA1: Brazil, Argentina, Chile, Colombia
- Tier SA2: Peru, Uruguay, Ecuador, Paraguay, Bolivia, 🔴 Venezuela, Guyana, Suriname

### Asia
- Done: ✅ Israel (Hebrew-computable; Tier AS1), ✅ Iran (Persian- and Hijri-computable; Tier AS3)
- Tier AS1: India (Hijri- and Easter-computable; central Gazetted list only — Hindu-calendar holidays and state-specific days deferred), Japan, China (Chinese-lunisolar-computable), South Korea (Korean-lunisolar-computable), Singapore (Hijri- and Chinese-lunisolar-computable components only — Hindu/Buddhist components deferred), Turkey (Hijri-computable)
- Tier AS2: Vietnam (Chinese-lunisolar-computable, approximate), Philippines, Indonesia (Hijri- and Easter-computable components only — Nyepi/Vesak deferred), Malaysia (Hijri- and Chinese-lunisolar-computable components only — Hindu/Buddhist components deferred), Pakistan (Hijri-computable), Bangladesh (Hijri-computable component only — Hindu/Buddhist minority holidays deferred), Saudi Arabia (Hijri-computable), UAE (Hijri-computable)
- Tier AS3: 🔴 Thailand (Buddhist lunar calendar, still hard), Taiwan (Taiwan-lunisolar-computable), Hong Kong (Chinese-lunisolar-computable), Kazakhstan, Qatar (Hijri-computable), Kuwait (Hijri-computable), Iraq (Hijri-computable, approximate — Sunni/Shia moon-sighting authorities occasionally differ by a day)
- Tier AS4 (low priority): 🔴 Sri Lanka, 🔴 Nepal, 🔴 Myanmar, 🔴 Cambodia, 🔴 Laos, 🔴 Mongolia, Uzbekistan (Hijri-computable), remaining Central Asia (Hijri-computable)

### Africa
- Tier AF1: South Africa, Nigeria (Hijri- and Easter-computable), Egypt (Hijri- and Coptic-Easter-computable), Kenya, Morocco (Hijri-computable), Ghana
- Tier AF2 (low priority): remaining African nations, Ethiopia (Ethiopian-calendar-computable)

### Oceania
- Tier OC1: Australia, New Zealand
- Tier OC2 (low priority, mostly 🔴): Fiji, Papua New Guinea, remaining Pacific micro-states

## Batching / rollout

One `writing-plans` implementation plan (and PR) per tier listed above,
following the continent groupings and priority order shown. As of
2026-08-26, the calendar-calculation mechanisms design has unblocked most
previously-🔴 countries (see that design's reclassification table) — tiers
are no longer gated on it except for the small residual list of countries
whose calendars remain genuinely irreducible to formula (Thailand, Sri
Lanka, Nepal, Myanmar, Cambodia, Laos, Mongolia, Haiti, Venezuela, and the
Hindu/Buddhist-calendar components of India/Indonesia/Malaysia/Singapore/
Bangladesh).

**First batch: Tier E1** — United Kingdom, Poland, Netherlands, Ukraine,
Sweden, Switzerland, Belgium, Austria. All computable via the existing
formula-based pattern (no 🔴 countries), directly extending the proven
Europe implementation.

## Testing

Each country's `NationalHolidays`/`StateHolidays` gets a test file at
`TemporaLinq.Test/Holidays/<Continent>/<Country>Test.cs`, following the
existing FranceTest/GermanyTest/ItalyTest pattern — asserting known holiday
dates for a spread of years, including movable-feast years.

## Out of scope for this design

- Implementing any remaining 🔴 country — see
  `docs/superpowers/specs/2026-08-26-calendar-calculation-mechanisms-design.md`
  for what's still genuinely hard and why.
- Non-Europe tiers — each gets planned via `writing-plans` when its turn comes.
