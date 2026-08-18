namespace TemporaLinq;

public static class Weekends
{
    public static DayOfWeek[] Western => [DayOfWeek.Saturday, DayOfWeek.Sunday];
    public static DayOfWeek[] Arab => [DayOfWeek.Thursday, DayOfWeek.Friday];
    public static DayOfWeek[] Saudi => [DayOfWeek.Friday, DayOfWeek.Saturday];
    public static DayOfWeek[] Israel => [DayOfWeek.Friday, DayOfWeek.Saturday];
    public static DayOfWeek[] Iran => [DayOfWeek.Thursday, DayOfWeek.Friday];
}
