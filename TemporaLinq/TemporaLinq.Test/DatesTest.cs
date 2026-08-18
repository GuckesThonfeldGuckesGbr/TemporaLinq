using System.Globalization;
using FluentAssertions;

namespace TemporaLinq.Test;

using TemporaLinq.Dates;
using Dates = TemporaLinq.Dates.Dates;

public class DatesTest
{
    [Fact]
    public void LocalCalendarHasOneWeek()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var oneWeek = Dates.Local().From(today)
            .TakeWhile(date => date == today || date.DayOfWeek != today.DayOfWeek)
            .ToList();
        oneWeek.Should().HaveCount(7);
    }

    [Fact]
    public void AllCalendarsHaveSevenDayWeeks()
    {
        var calendars = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Select(c => c.Calendar).Distinct();

        foreach (var calendar in calendars)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var oneWeek = Dates.OfCalendar(calendar).From(today)
                .TakeWhile(date => date == today || date.DayOfWeek != today.DayOfWeek).ToList();
            oneWeek.Should().HaveCount(7);
        }
    }
    
    
}