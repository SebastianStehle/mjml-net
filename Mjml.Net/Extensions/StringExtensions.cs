using Mjml.Net.Internal;

namespace Mjml.Net.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] TrimChars = { ' ', '\n', '\r' };

        public static string? SuffixCssClasses(this string classAttributeValue, string? suffix)
        {
            if (classAttributeValue == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(classAttributeValue))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(suffix))
            {
                return classAttributeValue;
            }

            var trimmed = classAttributeValue.Trim();

            if (!trimmed.Contains(' ', StringComparison.OrdinalIgnoreCase))
            {
                return $"{trimmed}-{suffix}";
            }
            else
            {
                var classes = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var stringBuilder = ObjectPools.StringBuilder.Get();
                try
                {
                    foreach (var className in classes)
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(' ');
                        }

                        stringBuilder.Append(className);
                        stringBuilder.Append('-');
                        stringBuilder.Append(suffix);
                    }

                    return stringBuilder.ToString();
                }
                finally
                {
                    ObjectPools.StringBuilder.Return(stringBuilder);
                }
            }
        }

        public static ReadOnlySpan<char> TrimXmlStart(this ReadOnlySpan<char> source)
        {
            return source.TrimStart(TrimChars);
        }

        public static ReadOnlySpan<char> TrimXmlEnd(this ReadOnlySpan<char> source)
        {
            return source.TrimEnd(TrimChars);
        }
    }
}
