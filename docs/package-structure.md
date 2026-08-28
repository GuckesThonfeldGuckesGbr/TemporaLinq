# Package structure

## Today

TemporaLinq isn't published on NuGet yet, and today it's three projects, not
one-per-country:

- `TemporaLinq` — date/time streams (`Dates`, `Times`, the LINQ-style
  operators). No dependencies.
- `TemporaLinq.Astronomy` — the Meeus-based astronomical primitives (lunar
  phases, solstices) used by a few holiday calculations. No dependencies.
- `TemporaLinq.Holidays` — every country's `NationalHolidays`/`StateHolidays`,
  plus the calendar-calculation classes (Hijri, Hebrew, Chinese lunisolar,
  Mongolian, etc.). Depends on both of the above.

Until this is published, reference the projects directly:

```bash
dotnet add reference path/to/TemporaLinq/TemporaLinq.csproj
dotnet add reference path/to/TemporaLinq.Holidays/TemporaLinq.Holidays.csproj
```

## The plan

`TemporaLinq.Holidays` bundles all ~75 countries today, which is more than
most consumers need. The intent is to publish it (and grow it) as a family of
granular NuGet packages instead, so you only pull in what you actually use:

- `TemporaLinq` — the core date/time streams, always a dependency of any
  holidays package.
- `TemporaLinq.Holidays.Europe.Germany` — a single country. Depends only on
  `TemporaLinq` (and `TemporaLinq.Astronomy`/calendar-calculation code, for
  the countries that need it).
- `TemporaLinq.Holidays.Europe` — every European country, as a convenience
  bundle, for consumers who'd rather not list them individually.
- `TemporaLinq.Holidays` — everything, kept as the "just give me all of it"
  option.

This mirrors the existing folder layout
(`TemporaLinq.Holidays/<Continent>/<Country>/`), so the split is expected to
be mostly a packaging exercise rather than a code reorganization. **This
isn't built yet** — until it is, `TemporaLinq.Holidays` is the only way to
get any country's holidays, and it pulls in all of them regardless of which
one you need.
