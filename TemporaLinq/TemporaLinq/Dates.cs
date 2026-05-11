using System.Collections;
using System.Globalization;

namespace de.baggerbagger.TemporaLinq;

public record Dates : IDateEnumerable
{
    public DateOnly EndDate { get; private init; }

    public DateOnly StartDate { get; private init; }

    public CalendarConfig Config { get; private init; } = CalendarConfig.Default;
    private Dates(Calendar calendar)
    {
        Config = new CalendarConfig(calendar, Weekends.Western);
        StartDate = DateOnly.FromDateTime(calendar.MinSupportedDateTime);
        EndDate = DateOnly.FromDateTime(calendar.MaxSupportedDateTime);
    }

    public static Dates OfCalendar(Calendar calendar)
        => new(calendar);

    public static Dates Local()
        => OfCalendar(CultureInfo.CurrentCulture.Calendar);

    public static Dates Invariant()
        => OfCalendar(CultureInfo.InvariantCulture.Calendar);

    public Dates From(DateOnly date) => this with { StartDate = date };
    public Dates To(DateOnly date) => this with { EndDate = date };
    public Dates WithWeekend(params DayOfWeek[] days) => this with { Config = Config.WithWeekend(days) };

    public IEnumerator<DateOnly> GetEnumerator()
    {
        for (var date = StartDate; date <= EndDate; date = NextDay(date))
        {
            yield return date;
        }
    }

    private DateOnly NextDay(DateOnly date) 
        => DateOnly.FromDateTime(Config.Calendar.AddDays(date.ToDateTime(TimeOnly.MinValue), 1));

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}