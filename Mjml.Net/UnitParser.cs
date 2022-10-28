using System.Globalization;

namespace Mjml.Net
{
    public static class UnitParser
    {
        public static (double Value, Unit Unit) Parse(string? rawValue, Unit defaultUnit = Unit.None)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return (0, Unit.Unknown);
            }

            var span = rawValue.AsSpan().Trim();

            var hasSeparator = false;

            var i = 0;
            for (i = 0; i < span.Length; i++)
            {
                var c = span[i];

                if (c == '.' || c == ',')
                {
                    hasSeparator = true;
                    continue;
                }

                if (!char.IsNumber(c))
                {
                    break;
                }
            }

            var unitSpan = span[i..];
            var unitType = Unit.Unknown;

            var f = new string(unitSpan);

            if (unitSpan.StartsWith("px", StringComparison.OrdinalIgnoreCase))
            {
                unitType = Unit.Pixels;
            }
            else if (unitSpan.StartsWith("%", StringComparison.OrdinalIgnoreCase))
            {
                unitType = Unit.Percent;
            }
            else if (unitSpan.StartsWith("em", StringComparison.OrdinalIgnoreCase))
            {
                unitType = Unit.Em;
            }
            else if (unitSpan.StartsWith("rem", StringComparison.OrdinalIgnoreCase))
            {
                unitType = Unit.Rem;
            }
            else if (unitSpan.Length == 0)
            {
                unitType = defaultUnit;
            }

            var valueSpan = span[..i];

            if (valueSpan.Length == 0)
            {
                return (0, unitType);
            }

            if (hasSeparator)
            {
                double.TryParse(valueSpan, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp);

                if (unitType == Unit.Pixels)
                {
                    return ((int)temp, unitType);
                }

                return (temp, unitType);
            }
            else
            {
                int.TryParse(valueSpan, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp);

                return (temp, unitType);
            }
        }
    }
}
