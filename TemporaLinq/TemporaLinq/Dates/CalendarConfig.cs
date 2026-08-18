using System.Globalization;

namespace TemporaLinq.Dates;

public record CalendarConfig(Calendar Calendar, DayOfWeek[] WeekendDays)
{
    public static CalendarConfig Default { get; } = new(
        CultureInfo.InvariantCulture.Calendar,
        Weekends.Western
    );

    public CalendarConfig WithWeekend(params DayOfWeek[] weekendDays)
        => this with { WeekendDays = weekendDays };
}
