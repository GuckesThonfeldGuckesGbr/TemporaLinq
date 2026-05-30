namespace TemporaLinq.Holidays;

/// <summary>
/// Represents a holiday with its date and name.
/// </summary>
public readonly record struct Holiday(DateOnly Date, HolidayNames Name) : IComparable<Holiday>
{
    public static implicit operator DateOnly(Holiday holiday) => holiday.Date;
    public static implicit operator DateOnly?(Holiday? holiday) => holiday?.Date;

    public int CompareTo(Holiday other) 
        => Date.CompareTo(other.Date);
}