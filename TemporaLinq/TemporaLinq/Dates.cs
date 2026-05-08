using System.Collections;
using System.Globalization;

namespace TemporaLinq;

public record Dates : IEnumerable<DateOnly>
{
    private readonly Calendar calendar;

    public DateOnly EndDate { get; private init; }

    public DateOnly StartDate { get; private init; }

    private Dates(Calendar calendar)
    {
        this.calendar = calendar;
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

    public IEnumerator<DateOnly> GetEnumerator()
    {
        for (var date = StartDate; date <= EndDate; date = NextDay(date))
        {
            yield return date;
        }
    }

    private DateOnly NextDay(DateOnly date) 
        => DateOnly.FromDateTime(calendar.AddDays(date.ToDateTime(TimeOnly.MinValue), 1));

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}