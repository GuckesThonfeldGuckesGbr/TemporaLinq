using System.Collections;
using System.Reflection;
using FluentAssertions;
using Memoizer;
using TemporaLinq.Holidays;

namespace TemporaLinq.Test;

[Collection("Cache")]
public class CacheTest
{
    private const int YearsToGenerate = 10;
    private const BindingFlags CachedMethodFlags = BindingFlags.NonPublic | BindingFlags.Static;

    // 1902, not 1900: System.Globalization.ChineseLunisolarCalendar (used by
    // ChineseLunisolarCalendarCalculation, which China/Hong Kong/Vietnam's holidays are
    // built on) only supports Gregorian dates from 02/19/1901 onward, so the first
    // Gregorian year whose full Jan 1-Dec 31 span is in range is 1902; using 1900 or 1901
    // throws ArgumentOutOfRangeException.
    private const int StartYear = 1902;

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
        // Timing-based verification of caching is inherently flaky (JIT warm-up, GC, scheduler
        // noise). Instead, verify the invariant the timing was a proxy for: the holiday
        // computation is wired up as a static, [Cache]-decorated method, so it is memoized once
        // per class rather than recomputed per instance.
        var method = type.GetMethod("GetHolidaysFor", CachedMethodFlags, [typeof(int)]);

        method.Should().NotBeNull(
            $"{type.Name} should have a private static GetHolidaysFor(int) method backing its holiday computation");
        method.IsStatic.Should().BeTrue(
            "the cache must be shared across instances of the same class, not created per object");
        method.GetCustomAttribute<CacheAttribute>().Should().NotBeNull(
            $"{type.Name}.GetHolidaysFor should be decorated with [Cache] so it isn't recomputed on every call");

        var holidayEnumerable1 = SetUpInstance(type);
        var holidayEnumerable2 = SetUpInstance(type);

        holidayEnumerable1.ToList().Should().BeEquivalentTo(holidayEnumerable2.ToList());
    }

    private static IHolidayEnumerable SetUpInstance(Type type)
    {
        var holidayEnumerable = Activator.CreateInstance(type);
        type.BaseType
            .GetProperty(nameof(IHolidayEnumerable.StartDate))!
            .SetValue(holidayEnumerable, new DateOnly(StartYear, 1, 1));

        type.BaseType
            .GetProperty(nameof(IHolidayEnumerable.EndDate))!
            .SetValue(holidayEnumerable, new DateOnly(StartYear + YearsToGenerate, 1, 1));
        return (IHolidayEnumerable) holidayEnumerable!;
    }
}