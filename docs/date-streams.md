# Date and time streams

`TemporaLinq` treats a range of dates (or times) as a plain `IEnumerable<DateOnly>`
(or `IEnumerable<TimeOnly>`), so ordinary LINQ works on it. The library adds a
thin set of extension methods on top for the operations that come up
constantly when working with calendars: weekends, business days, and
set-like combination of several streams.

## `Dates`

```csharp
using TemporaLinq.Dates;

var january = Dates.Invariant()
    .From(new DateOnly(2026, 1, 1))
    .To(new DateOnly(2026, 1, 31)); // inclusive
```

- `Dates.Invariant()` uses `CultureInfo.InvariantCulture`'s calendar (almost
  always what you want). `Dates.Local()` uses the current culture's calendar;
  `Dates.OfCalendar(calendar)` accepts any `System.Globalization.Calendar`
  (e.g. `HijriCalendar`) if you need dates iterated in a non-Gregorian system.
- `.From`/`.To` are both inclusive and return a new `Dates` value (it's an
  immutable record) rather than mutating in place.

### Weekends and business days

```csharp
var businessDays = january.OnlyBusinessDays(); // Mon-Fri
var weekends = january.OnlyWeekends();         // Sat-Sun

// Custom weekend, e.g. the Arab working week
var arabBusinessDays = january.WithWeekend(DayOfWeek.Thursday, DayOfWeek.Friday).OnlyBusinessDays();
```

`Weekends` has a few common presets (`Weekends.Western`, `.Arab`, `.Saudi`,
`.Israel`, `.Iran`) usable with `.WithWeekend(Weekends.Saudi)`.

### LINQ-style operators

Beyond standard LINQ (`Where`, `Select`, `Take`, `Skip`, ...), which are
overridden to keep returning an `IDateEnumerable` (so you can keep chaining
date-specific operators), a few date-specific ones are provided:

- `.Only(params DayOfWeek[] days)` / `.Except(params DayOfWeek[] days)` —
  filter to (or exclude) specific weekdays.
- `.EveryNth(n)` — every n-th date in the stream.
- `.First(DayOfWeek)` / `.FirstOrDefault(DayOfWeek)` — first occurrence of a
  weekday in the stream (throws, or returns `default`, if none is found).

### Combining streams

`Merge`, `Union`, `Intersect`, `Except`, and `Distinct` all work on sorted
(ascending) date streams, in O(n+m) rather than materializing sets:

```csharp
using TemporaLinq; // for AsMonotonicallyAscendingEnumerable

// Business days that are also not a holiday
var holidayDates = germanHolidays.Select(h => (DateOnly) h); // Holiday -> DateOnly
var workingDays = businessDays.Except(holidayDates.AsMonotonicallyAscendingEnumerable());
```

`Holiday` converts implicitly to `DateOnly`, so a `HolidayEnumerable<T>` (see
[Holidays](holidays.md)) can be turned into a plain date stream with a
`Select` and combined with any of the above. Note that `.Except`/`.Union`/
`.Intersect` require *both* sides to already be sorted ascending — every
stream this library produces already is, so this only matters if you build a
custom `IEnumerable<DateOnly>` by hand.

## `Times`

The same idea for times of day, e.g. generating appointment slots:

```csharp
using TemporaLinq.Times;

var slots = Times.From(new TimeOnly(9, 0))
    .To(new TimeOnly(17, 0))   // exclusive
    .Every(TimeSpan.FromMinutes(30));
```
