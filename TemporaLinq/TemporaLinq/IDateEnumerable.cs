namespace de.baggerbagger.TemporaLinq;

public interface IDateEnumerable : IEnumerable<DateOnly>
{
    CalendarConfig Config { get; }
}