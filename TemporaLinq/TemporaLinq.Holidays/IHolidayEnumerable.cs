namespace TemporaLinq.Holidays;

public interface IHolidayEnumerable : IMonotonicallyAscendingEnumerable<Holiday>
{
    DateOnly StartDate { get; }
    DateOnly EndDate { get; }
}