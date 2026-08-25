# TemporaLinq
A convenient library to handle series of dates as streams

## Usage

### Business days and weekends

```csharp
using TemporaLinq.Dates;

var january = Dates.Invariant()
    .From(new DateOnly(2026, 1, 1))
    .To(new DateOnly(2026, 1, 31));

var businessDays = january.BusinessDays(); // Mon-Fri
var weekends = january.Weekends();         // Sat-Sun

// Custom weekend, e.g. the Arab working week
var arabBusinessDays = january.WithWeekend(DayOfWeek.Thursday, DayOfWeek.Friday).BusinessDays();
```

### Holidays

```csharp
using TemporaLinq.Holidays.Europe.Germany;

var holidays = NationalHolidays.Create()
    .From(new DateOnly(2026, 1, 1))
    .To(new DateOnly(2026, 12, 31));

foreach (var holiday in holidays)
    Console.WriteLine($"{holiday.Date}: {holiday.Name}");

var isChristmas = holidays.IsHoliday(new DateOnly(2026, 12, 25)); // true
```
