namespace TemporaLinq.Dates;

public interface IDateEnumerable : IMonotonicallyAscendingEnumerable<DateOnly>
{
    CalendarConfig Config { get; }
}