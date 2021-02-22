using System;
using System.Globalization;

namespace CurrenciesRatesParser.Mappers
{
    public static class ParseMapper
    {
        public static double ParseToDoubleFormat(this string str)
        {
            return double
                .Parse(string.Join(
                    string.Empty,
                    str.Split(default(string[]),
                    StringSplitOptions.RemoveEmptyEntries))
                .Replace(',', '.'), CultureInfo.InvariantCulture);
        }
    }
}
