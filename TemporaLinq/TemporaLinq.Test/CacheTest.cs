using System.Collections;
using System.Diagnostics;
using FluentAssertions;
using TemporaLinq.Holidays;

namespace TemporaLinq.Test;

[Collection("Cache")]
public class CacheTest
{
    private const int ExpectedSpeedup = 5;
    private const int YearsToGenerate = 10;

    private class AllHolidayEnumerables : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var holidayAssemblyTypes = typeof(Holiday).Assembly
                .GetTypes()
                .Where(t => typeof(IHolidayEnumerable).IsAssignableFrom(t)
                            && t is { IsAbstract: false, IsInterface: false })
                .Select(t => new object[] { t });

            return holidayAssemblyTypes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(AllHolidayEnumerables))]
    public void CacheIsOnlyCreatedOncePerClassNotPerObject(Type type)
    {
        var stopwatch = new Stopwatch();
        var holidayEnumerable1 = SetUpInstance(type);
        var holidayEnumerable2 = SetUpInstance(type);

        stopwatch.Start();
        var dates1 = holidayEnumerable1.ToList();
        stopwatch.Stop();
        var time1 = stopwatch.ElapsedTicks;
        stopwatch.Reset();

        stopwatch.Start();
        var dates2 = holidayEnumerable2.ToList();
        stopwatch.Stop();
        var time2 = stopwatch.ElapsedTicks;
        stopwatch.Reset();

        time2.Should().BeLessThan(time1 / ExpectedSpeedup);

        dates1.Should().BeEquivalentTo(dates2);
    }

    private static IHolidayEnumerable SetUpInstance(Type type)
    {
        var holidayEnumerable = Activator.CreateInstance(type);
        type.BaseType
            .GetProperty(nameof(IHolidayEnumerable.StartDate))!
            .SetValue(holidayEnumerable, new DateOnly(1900, 1, 1));
        
        type.BaseType
            .GetProperty(nameof(IHolidayEnumerable.EndDate))!
            .SetValue(holidayEnumerable, new DateOnly(1900 + YearsToGenerate, 1, 1));
        return (IHolidayEnumerable) holidayEnumerable!;
    }
}