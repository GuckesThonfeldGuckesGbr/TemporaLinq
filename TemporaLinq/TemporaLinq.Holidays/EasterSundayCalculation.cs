using Memoizer;

namespace TemporaLinq.Holidays;

public abstract class EasterSundayCalculation
{
    public abstract DateOnly ForYear(int year);

    public static readonly EasterSundayCalculation Christian = new ChristianEasterCalculation();
    public static readonly EasterSundayCalculation ChristianOrthodox = new ChristianOrthodoxEasterCalculation();

    private class ChristianEasterCalculation : EasterSundayCalculation
    {
        [Cache]
        public override DateOnly ForYear(int year)
        {
            var a = year % 19;
            var b = year / 100;
            var c = year % 100;
            var d = b / 4;
            var e = b % 4;
            var f = (b + 8) / 25;
            var g = (b - f + 1) / 3;
            var h = (19 * a + b - d - g + 15) % 30;
            var i = c / 4;
            var k = c % 4;
            var l = (32 + 2 * e + 2 * i - h - k) % 7;
            var m = (a + 11 * h + 22 * l) / 451;

            var month = (h + l - 7 * m + 114) / 31;
            var day = (h + l - 7 * m + 114) % 31 + 1;

            return new DateOnly(year, month, day);
        }
    }

    private class ChristianOrthodoxEasterCalculation : EasterSundayCalculation
    {
        [Cache]
        public override DateOnly ForYear(int year)
        {
            var a = year % 19;
            var b = year % 7;
            var c = year % 4;

            var d = (19 * a + 16) % 30;
            var e = (2 * c + 4 * b + 6 * d) % 7;
            var f = (19 * a + 16) % 30;

            var key = f + e + 3;
            var month = key > 30 ? 5 : 4;
            var day = key > 30 ? key - 30 : key;

            return new DateOnly(year, month, day);
        }
    }
}