# Holidays

## The model

A `Holiday` is a `Date` + a `Name` (a member of the shared `HolidayNames`
enum):

```csharp
public readonly record struct Holiday(DateOnly Date, HolidayNames Name);
```

`HolidayNames` is one big enum shared across every country, not a
per-country type. Holidays that are genuinely the same occasion across
countries (e.g. `NewYearsDay`, `GoodFriday`, `IndependenceDay`) reuse the same
member — check its `//` comment in `HolidayNames.cs` for which countries use
it. Holidays that only look similar but are locally distinct (e.g. Thailand's
`VisakhaBuchaDay` vs. Cambodia's `VisakBocheaDay` vs. Myanmar's
`KasonFullMoonDay` — all Vesak-adjacent Buddhist observances with their own
local name and, in some cases, slightly different calculation) get their own
enum member rather than being forced together.

## `HolidayEnumerable<T>`

Every country's holiday list is a class deriving from
`HolidayEnumerable<T>`, one per country (and, for a few countries, one per
subdivision — see below):

```csharp
using TemporaLinq.Holidays.Europe.Germany;

var holidays = NationalHolidays.Create()
    .From(new DateOnly(2026, 1, 1))
    .To(new DateOnly(2026, 12, 31));

foreach (var holiday in holidays)
    Console.WriteLine($"{holiday.Date}: {holiday.Name}");

holidays.IsHoliday(new DateOnly(2026, 12, 25));      // true
holidays.GetHoliday(new DateOnly(2026, 12, 25));     // Holiday? - the match, or null
holidays.TryGetHoliday(new DateOnly(2026, 12, 25), out var h);
```

- `.Create()` gives you the full supported date range by default (`DateOnly.MinValue`
  to `DateOnly.MaxValue`); `.From`/`.To` narrow it, same as `Dates`.
- Holiday dates are computed lazily, one Gregorian year at a time, and cached
  per year (via `Memoizer`'s `[Cache]` attribute) so repeated enumeration or
  lookups over the same year don't recompute movable-feast/lunar-calendar math.
- Because `Holiday` implements `IComparable<Holiday>` and the enumerator
  yields years in ascending order, a `HolidayEnumerable<T>` composes with the
  date-stream operators in [Date and time streams](date-streams.md) — e.g.
  excluding holidays from a business-day stream.

## National vs. regional holidays

Most countries expose a single `NationalHolidays` class. A handful of federal
countries additionally expose a `StateHolidays.cs` with one class per
subdivision, each following the same `HolidayEnumerable<T>` pattern and often
composing smaller building blocks with `Operations.Merge`:

```csharp
using TemporaLinq.Holidays.Europe.Germany;

var badenWuerttemberg = BadenWuerttemberg.Create()
    .From(new DateOnly(2026, 1, 1))
    .To(new DateOnly(2026, 12, 31));
```

Check the relevant country's folder for whether a `StateHolidays.cs` exists;
most countries only have national-level holidays implemented so far (see
[Country coverage](countries.md)).

## Where holiday dates come from

Fixed-date and Easter-relative holidays are plain arithmetic
(`EasterSundayCalculation.Christian`/`.ChristianOrthodox`). Holidays on a
non-Gregorian calendar (Hijri, Hebrew, Persian, Chinese lunisolar, Ethiopian,
various Buddhist calendars, Mongolian) go through a dedicated calculation
class — see [Calendar calculations](calendar-calculations.md) for what each
one does and, importantly, how accurate it is and where it can be off by a
day or more.
