namespace Mjml.Net.Extensions
{
    public static class StringExtensions
    {
        public static string? SuffixCssClasses(this string classAttributeValue, string? suffix)
        {
            if (string.IsNullOrEmpty(classAttributeValue))
            {
                return null;
            }

            if (string.IsNullOrEmpty(suffix))
            {
                return classAttributeValue;
            }

            var suffixedClasses = classAttributeValue
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => $"{c}-{suffix}");

            return string.Join(" ", suffixedClasses);
        }
    }
}
