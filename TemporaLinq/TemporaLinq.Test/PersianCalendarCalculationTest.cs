using FluentAssertions;
using TemporaLinq.Holidays;

namespace TemporaLinq.Test;

public class PersianCalendarCalculationTest
{
    [Fact]
    public void DateInGregorianYear_ReturnsNowruz()
    {
        // Nowruz (1 Farvardin 1403) - the Persian New Year - fell on 2024-03-20.
        var date = PersianCalendarCalculation.DateInGregorianYear(2024, 1, 1);

        date.Should().Be(new DateOnly(2024, 3, 20));
    }

    [Fact]
    public void DateInGregorianYear_ReturnsDifferentNowruzInAnotherYear()
    {
        // Nowruz 1404 fell on 2025-03-21.
        var date = PersianCalendarCalculation.DateInGregorianYear(2025, 1, 1);

        date.Should().Be(new DateOnly(2025, 3, 21));
    }
}
