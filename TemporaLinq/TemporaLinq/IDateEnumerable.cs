namespace TemporaLinq;

public interface IDateEnumerable : IMonotonicallyAscendingEnumerable<DateOnly>
{
    CalendarConfig Config { get; }
}