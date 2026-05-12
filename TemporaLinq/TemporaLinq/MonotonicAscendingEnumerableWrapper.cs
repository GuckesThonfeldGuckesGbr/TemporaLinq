using System.Collections;

namespace TemporaLinq;

/// <summary>
/// Wraps an IEnumerable and enforces monotonic ascending order.
/// Throws a InvalidOperationException when a value is encountered that is less than the previous value.
/// </summary>
internal record MonotonicAscendingEnumerableWrapper<T>(IEnumerable<T> source) : IMonotonicallyAscendingEnumerable<T>
    where T : IComparable<T>
{
    private readonly IEnumerable<T> _source = source ?? throw new ArgumentNullException(nameof(source));

    public IEnumerator<T> GetEnumerator() => new MonotonicAscendingEnumerator<T>(_source.GetEnumerator());

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Enumerator that validates monotonic ascending order during enumeration.
/// </summary>
internal class MonotonicAscendingEnumerator<T>(IEnumerator<T> source) : IEnumerator<T>
    where T : IComparable<T>
{
    private readonly IEnumerator<T> _source = source ?? throw new ArgumentNullException(nameof(source));
    private T? _current;
    private bool _hasPrevious;
    private T _previous = default!;

    public T Current => _current ?? throw new InvalidOperationException("Enumeration has not started.");

    object? IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (!_source.MoveNext())
            return false;

        _current = _source.Current;

        if (_hasPrevious)
        {
            if (_previous!.CompareTo(_current) == 0)
            {
                throw new InvalidOperationException(
                    $"Sequence is not monotonically ascending. Previous value: {_previous}, Current value: {_current}");
            }
        }

        _previous = _current;
        _hasPrevious = true;

        return true;
    }

    public void Reset()
    {
        _source.Reset();
        _current = default;
        _hasPrevious = false;
    }

    public void Dispose() => _source.Dispose();
}