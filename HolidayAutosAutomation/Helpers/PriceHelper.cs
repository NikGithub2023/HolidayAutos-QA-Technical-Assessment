using System.Globalization;
using System.Text.RegularExpressions;

namespace HolidayAutosAutomation.Helpers;

public static class PriceHelper
{
    public static decimal ParsePrice(string priceText)
    {
        var cleaned = Regex.Replace(priceText, @"[^\d\.]", string.Empty);
        return decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0m;
    }
}
