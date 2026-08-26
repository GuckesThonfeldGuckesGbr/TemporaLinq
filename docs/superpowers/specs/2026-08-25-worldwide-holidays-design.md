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
- Done: ✅ Bosnia and Herzegovina (Tier E4 — state-level/BiH-wide holidays only: New Year and Labour Day; entity-specific and community-religious holidays, including the Eids, are out of scope, see `TemporaLinq.Holidays/Europe/BosniaAndHerzegovina/NationalHolidays.cs`)
- Tier E4 remaining: Belarus
- Done: ✅ North Macedonia, ✅ Montenegro, ✅ Andorra, ✅ Monaco, ✅ San Marino, ✅ Liechtenstein, ✅ Vatican City, ✅ Albania, ✅ Kosovo (Tier E5)

### North America
- Done: ✅ USA
- Tier NA1: Canada, Mexico
- Tier NA2: Costa Rica, Panama, Guatemala, Dominican Republic, Jamaica, Cuba, Honduras, El Salvador
- Done: ✅ Haiti (fixed civil/religious days plus Easter-relative Carnival Monday/Tuesday and Good Friday; government-decreed holiday shifts and one-off commemorative days are out of scope, see `TemporaLinq.Holidays/NorthAmerica/Haiti/NationalHolidays.cs`) (Tier NA3)
- Tier NA3 remaining: Nicaragua, Belize, Bahamas, Trinidad and Tobago, Barbados, remaining Caribbean micro-states

### South America
- Tier SA1: Brazil, Argentina, Chile, Colombia
- Done: ✅ Venezuela (fixed civil/religious days plus Easter-relative Carnival Monday/Tuesday, Maundy Thursday, and Good Friday; "puente" decree-based holiday shifts are out of scope, see `TemporaLinq.Holidays/SouthAmerica/Venezuela/NationalHolidays.cs`) (Tier SA2)
- Tier SA2 remaining: Peru, Uruguay, Ecuador, Paraguay, Bolivia, Guyana, Suriname

### Asia
- Done: ✅ India (central Gazetted list only — Republic Day, Independence Day, Gandhi Jayanti, Good Friday, Christmas Day, and Hijri-computable Eid al-Fitr, Eid al-Adha/Bakrid, Muharram, and Milad-un-Nabi; Hindu-calendar holidays and state-specific days remain deferred pending a future Hindu/Buddhist calendar calculation mechanism), ✅ Turkey (`TemporaLinq.Holidays/Asia/Turkey/NationalHolidays.cs`, establishes the `Asia` folder convention), ✅ Israel (Hebrew-computable), ✅ China (Chinese-lunisolar-computable), ✅ South Korea (Korean-lunisolar-computable)
- Tier AS1 remaining: Japan
- Done: ✅ Pakistan, ✅ Bangladesh (Hijri-based and fixed civil holidays only — Hindu/Buddhist minority holidays (Durga Puja, Buddha Purnima) remain deferred pending a future Hindu/Buddhist calendar calculation mechanism), ✅ Saudi Arabia, ✅ UAE, ✅ Vietnam (Chinese-lunisolar-computable, approximate), ✅ Singapore (Hijri-, Chinese-lunisolar-, Easter-, and Buddhist-Vesak-computable — Deepavali (Hindu) deferred) (Tier AS2)
- Tier AS2 remaining: Philippines
- Done: ✅ Indonesia (Hijri-, Easter-, Chinese-lunisolar-, and Buddhist-Vesak/Waisak-computable components — Nyepi (Balinese Saka calendar) and Hindu Deepavali deferred), ✅ Malaysia (federal/national-level only: Hijri-, Chinese-lunisolar-, and Buddhist-Vesak-computable components plus the Agong's Birthday (first Monday of June) — Hindu Deepavali and state-specific holidays deferred) (Tier AS2)
- Done: ✅ Qatar, ✅ Kuwait, ✅ Iraq (Hijri-computable, approximate — Sunni/Shia moon-sighting authorities occasionally differ by a day), ✅ Iran (Persian- and Hijri-computable), ✅ Hong Kong (Chinese-lunisolar-computable), ✅ Taiwan (Taiwan-lunisolar-computable) (Tier AS3)
- Done: ✅ Thailand (Buddhist-lunisolar-computable via new `TemporaLinq.Astronomy.SoutheastAsianBuddhistCalendar` for Makha/Visakha/Asalha Bucha) (Tier AS3)
- Tier AS3 remaining: Kazakhstan
- Tier AS4 (low priority): 🔴 Nepal (civil Bikram Sambat calendar has no closed-form formula — every known implementation, including professional ones, relies on a pre-computed per-year month-length lookup table published by Nepal's own calendar authority; deferred as a formula-vs-table tradeoff pending explicit sign-off on embedding a static table, an exception to this project's formula-first approach — Nepal's Hindu-calendar holidays, e.g. Dashain/Tihar, may still become computable via a future Hindu lunisolar calendar mechanism independent of this civil-calendar blocker), ✅ Myanmar (Buddhist-lunisolar-computable, same mechanism as Thailand), 🔴 Mongolia, remaining Central Asia (Hijri-computable)
- Done: ✅ Uzbekistan, ✅ Sri Lanka (full-moon-computable via new `TemporaLinq.Astronomy.LunarPhaseCalculation`; Maha Sivarathri, a Hindu lunar holiday, remains deferred), ✅ Cambodia (Buddhist-lunisolar-computable via new `TemporaLinq.Astronomy.SoutheastAsianBuddhistCalendar` for Visak Bochea Day; Meak Bochea (removed from Cambodia's statutory list since 2020) and Asalha Bucha (never statutory there) are out of scope, as are Pchum Ben, the Water Festival, and the Royal Ploughing Ceremony — see `TemporaLinq.Holidays/Asia/Cambodia/NationalHolidays.cs`), ✅ Laos (fixed civil holidays only — per Laos's Labour Law (2013, Art. 55), it has no statutory Buddhist-calendar holiday at all; Visakha Bousa/Boun Khao Phansa/Boun Ok Phansa are widely observed culturally but not statutory, and National Teachers' Day is restricted to teachers/education staff — see `TemporaLinq.Holidays/Asia/Laos/NationalHolidays.cs`) (Tier AS4)

### Africa
- Done: ✅ Nigeria, ✅ Egypt, ✅ Morocco (Tier AF1, partial)
- Tier AF1 remaining: South Africa, Kenya, Ghana
- Done: ✅ Ethiopia (Ethiopian-calendar-computable; moved up from Tier AF2 once `EthiopianCalendarCalculation` was built and verified)
- Tier AF2 (low priority): remaining African nations

### Oceania
- Tier OC1: Australia, New Zealand
- Tier OC2 (low priority, mostly 🔴): Fiji, Papua New Guinea, remaining Pacific micro-states

## Batching / rollout

One `writing-plans` implementation plan (and PR) per tier listed above,
following the continent groupings and priority order shown. As of
2026-08-26, the calendar-calculation mechanisms design has unblocked most
previously-🔴 countries (see that design's reclassification table) — tiers
are no longer gated on it except for the small residual list of countries
whose calendars remain genuinely irreducible to formula (Nepal, Mongolia,
and the Hindu-calendar components of India/Indonesia/Malaysia/Singapore/
Bangladesh, plus Indonesia's Balinese-Saka-calendar Nyepi). Sri Lanka,
Thailand, Myanmar, Cambodia, and Laos, all previously flagged 🔴 here, are
now done: Sri Lanka via `LunarPhaseCalculation` (2026-08-26), and
Thailand/Myanmar/Cambodia plus the Buddhist-calendar Vesak component of
Indonesia/Malaysia/Singapore via the follow-up
`SoutheastAsianBuddhistCalendar` mechanism (2026-08-26) — Laos has no
statutory Buddhist-calendar holiday at all, so it needed only the fixed
civil-holiday pattern. Haiti and Venezuela, also previously flagged 🔴, are
now done as well (2026-08-27) — both run on the standard Gregorian/
Christian-Easter calendar; they were flagged for decree-based political
volatility, not calendar complexity, so this ships the stable annual
subset with that caveat documented.

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
