namespace Mjml.Net
{
    internal static class BindingHelper
    {
        public static (string?, string?, string?, string?) ParseShorthandValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return (null, null, null, null);
            }

            var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string? t = null;
            string? r = null;
            string? b = null;
            string? l = null;

            switch (parts.Length)
            {
                case 2:
                    t = parts[0];
                    r = parts[1];
                    b = parts[0];
                    l = parts[1];
                    break;
                case 3:
                    t = parts[0];
                    r = parts[1];
                    b = parts[2];
                    l = parts[1];
                    break;
                case 4:
                    t = parts[0];
                    r = parts[1];
                    b = parts[2];
                    l = parts[3];
                    break;
            }

            return (t, r, b, l);
        }

        public static string CoerceColor(string value)
        {
            var trimmed = value.AsSpan().Trim();

            if (trimmed.Length == 4 && trimmed[0] == '#')
            {
                return new string(new char[]
                {
                    trimmed[0],
                    trimmed[1],
                    trimmed[1],
                    trimmed[2],
                    trimmed[2],
                    trimmed[3],
                    trimmed[3]
                });
            }

            return value;
        }
    }
}
