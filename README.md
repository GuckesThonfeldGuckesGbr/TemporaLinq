# TemporaLinq
A convenient library to handle series of dates as streams, plus a growing set
of national/regional holiday calendars for ~75 countries, computed with real
calendar math rather than hand-maintained per-year tables.

## Installation

Not yet published on NuGet — reference the projects directly for now (see
[docs/package-structure.md](docs/package-structure.md) for the planned
granular per-continent/per-country NuGet split):

```bash
dotnet add reference path/to/TemporaLinq/TemporaLinq.csproj           # date/time streams
dotnet add reference path/to/TemporaLinq.Holidays/TemporaLinq.Holidays.csproj  # holidays, all countries
```

## Usage

### Business days and weekends

```csharp
using TemporaLinq.Dates;

var january = Dates.Invariant()
    .From(new DateOnly(2026, 1, 1))
    .To(new DateOnly(2026, 1, 31));

var businessDays = january.OnlyBusinessDays(); // Mon-Fri
var weekends = january.OnlyWeekends();         // Sat-Sun

// Custom weekend, e.g. the Arab working week
var arabBusinessDays = january.WithWeekend(DayOfWeek.Thursday, DayOfWeek.Friday).OnlyBusinessDays();
```

### Holidays

```csharp
using TemporaLinq.Holidays.Europe.Germany;

var holidays = NationalHolidays.Create()
    .From(new DateOnly(2026, 1, 1))
    .To(new DateOnly(2026, 12, 31));

foreach (var holiday in holidays)
    Console.WriteLine($"{holiday.Date}: {holiday.Name}");

holidays.IsHoliday(new DateOnly(2026, 12, 25)); // true
```

### Combining the two: working days excluding holidays

```csharp
using TemporaLinq;
using TemporaLinq.Dates;
using TemporaLinq.Holidays.Europe.Germany;

var year = Dates.Invariant().From(new DateOnly(2026, 1, 1)).To(new DateOnly(2026, 12, 31));
var holidayDates = NationalHolidays.Create().From(year.StartDate).To(year.EndDate)
    .Select(h => (DateOnly) h);

var workingDays = year.OnlyBusinessDays().Except(holidayDates.AsMonotonicallyAscendingEnumerable());
```

## Learn more

- [docs/date-streams.md](docs/date-streams.md) — the full `Dates`/`Times` API
- [docs/holidays.md](docs/holidays.md) — the holiday model, state-level holidays
- [docs/calendar-calculations.md](docs/calendar-calculations.md) — how each
  non-Gregorian calendar (Hijri, Hebrew, Chinese lunisolar, Mongolian, ...) is
  computed, and its accuracy limits
- [docs/countries.md](docs/countries.md) — every supported country and which
  calendar mechanism it uses
- [docs/known-gaps.md](docs/known-gaps.md) — holidays deliberately left
  unimplemented, and why
- [docs/package-structure.md](docs/package-structure.md) — current vs.
  planned packaging
