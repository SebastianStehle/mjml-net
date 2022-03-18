using System.Globalization;

namespace Mjml.Net.Components
{
    public static class UnitParser
    {
        public static (double Value, Unit Unit) Parse(string? rawValue)
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

            var unitSpan = span[i..].Trim();
            var unitType = Unit.Unknown;

            var f = new string(unitSpan);

            if (unitSpan.SequenceEqual("px"))
            {
                unitType = Unit.Pixels;
            }
            else if (unitSpan.SequenceEqual("%"))
            {
                unitType = Unit.Percent;
            }
            else if (unitSpan.Length == 0)
            {
                unitType = Unit.None;
            }

            var valueSpan = span[..i];

            if (valueSpan.Length == 0)
            {
                return (0, unitType);
            }

            if (hasSeparator)
            {
                double.TryParse(valueSpan, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp);

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
