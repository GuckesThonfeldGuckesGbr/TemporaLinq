using System.Collections;

namespace TemporaLinq.Times;

public record Times : ITimeEnumerable
{
    public TimeOnly StartTime { get; private init; }
    public TimeOnly EndTime { get; private init; }
    public TimeSpan Step { get; private init; }

    public static Times From(TimeOnly start) => new() { StartTime = start };

    /// <summary>
    /// The end time is exclusive
    /// </summary>
    /// <param name="end"></param>
    /// <returns></returns>
    public Times To(TimeOnly end) => this with { EndTime = end };

    public Times Every(TimeSpan step)
        => step > TimeSpan.Zero
            ? this with { Step = step }
            : throw new ArgumentOutOfRangeException(nameof(step), "Step must be positive.");

    public IEnumerator<TimeOnly> GetEnumerator()
    {
        for (var time = StartTime; time < EndTime; time = time.Add(Step))
        {
            yield return time;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
