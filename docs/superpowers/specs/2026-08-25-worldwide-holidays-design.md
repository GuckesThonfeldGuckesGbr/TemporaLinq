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

### Non-computable countries (new: static tables)

Some countries' holidays cannot be derived from a formula — Hijri
(moon-sighting) dates, lunisolar calendars, or dates set by annual government
decree/proclamation. For these, add a new base class:

```csharp
public abstract record StaticHolidayEnumerable<T> : IHolidayEnumerable
    where T : StaticHolidayEnumerable<T>, new()
```

living beside `HolidayEnumerable<T>` in `TemporaLinq.Holidays`. It is backed by
a hardcoded `IReadOnlyDictionary<int, ImmutableList<Holiday>>` (per-year
tables sourced from published government/official calendars). Years outside
the populated range yield no holidays — no exception — consistent with how
`Merge()`-based composition already treats an empty source. It implements
`IHolidayEnumerable` directly so it composes with the existing `Merge()`/`On()`
combinators unchanged, and countries needing it subclass it directly instead
of `HolidayEnumerable<T>`.

This table data will need periodic manual refresh as new years are published.
That is an accepted, known maintenance cost of this approach — not something
this design solves.

## Full checklist

Legend: ✅ done · 🔴 flagged hard (needs `StaticHolidayEnumerable<T>`, not a pure
formula) · plain = computable via the existing formula-based pattern.

### Europe
- Done: ✅ Germany, ✅ France, ✅ Italy, ✅ Spain
- Done: ✅ United Kingdom, ✅ Poland, ✅ Netherlands, ✅ Ukraine, ✅ Sweden, ✅ Switzerland, ✅ Belgium, ✅ Austria (Tier E1)
- Done: ✅ Ireland, ✅ Denmark, ✅ Norway, ✅ Finland, ✅ Czech Republic, ✅ Romania, ✅ Portugal, ✅ Greece (Tier E2)
- Done: ✅ Hungary, ✅ Bulgaria, ✅ Serbia, ✅ Croatia, ✅ Slovakia, ✅ Slovenia, ✅ Lithuania, ✅ Latvia (Tier E3)
- Done: ✅ Estonia, ✅ Iceland, ✅ Luxembourg, ✅ Malta, ✅ Cyprus, ✅ Moldova (Tier E4)
- Tier E4 deferred: 🔴 Belarus, 🔴 Bosnia and Herzegovina (entity-fragmented calendar with Islamic lunar-calendar holidays)
- Tier E5: North Macedonia, Albania, Montenegro, Andorra, Monaco, San Marino, Liechtenstein, Vatican City, Kosovo

### North America
- Done: ✅ USA
- Tier NA1: Canada, Mexico
- Tier NA2: Costa Rica, Panama, Guatemala, Dominican Republic, Jamaica, Cuba, Honduras, El Salvador
- Tier NA3: Nicaragua, Belize, Bahamas, Trinidad and Tobago, Barbados, 🔴 Haiti, remaining Caribbean micro-states

### South America
- Tier SA1: Brazil, Argentina, Chile, Colombia
- Tier SA2: Peru, Uruguay, Ecuador, Paraguay, Bolivia, 🔴 Venezuela, Guyana, Suriname

### Asia
- Tier AS1: 🔴 India, 🔴 Israel, Japan, 🔴 China, 🔴 South Korea, 🔴 Singapore, 🔴 Turkey
- Tier AS2: 🔴 Vietnam, 🔴 Philippines, 🔴 Indonesia, 🔴 Malaysia, 🔴 Pakistan, 🔴 Bangladesh, 🔴 Saudi Arabia, 🔴 UAE
- Tier AS3: 🔴 Thailand, 🔴 Taiwan, 🔴 Hong Kong, Kazakhstan, 🔴 Qatar, 🔴 Kuwait, 🔴 Iraq, 🔴 Iran
- Tier AS4 (low priority, mostly 🔴): Sri Lanka, Nepal, Myanmar, Cambodia, Laos, Mongolia, Uzbekistan, remaining Central Asia

### Africa
- Tier AF1: South Africa, 🔴 Nigeria, 🔴 Egypt, Kenya, 🔴 Morocco, Ghana
- Tier AF2 (low priority, mostly 🔴): remaining African nations, incl. 🔴 Ethiopia (own calendar)

### Oceania
- Tier OC1: Australia, New Zealand
- Tier OC2 (low priority, mostly 🔴): Fiji, Papua New Guinea, remaining Pacific micro-states

## Batching / rollout

One `writing-plans` implementation plan (and PR) per tier listed above,
following the continent groupings and priority order shown. Tiers containing
only or mostly 🔴 countries are deferred until the `StaticHolidayEnumerable<T>`
mechanism exists and has been proven on at least one 🔴 country.

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

- Implementing any 🔴 (static-table) country — deferred to a follow-up design
  once `StaticHolidayEnumerable<T>` exists and Tier E1 has shipped.
- Non-Europe tiers — each gets planned via `writing-plans` when its turn comes.
