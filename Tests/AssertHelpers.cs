using System;
using System.Linq;
using System.Text;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Strategies.AttributeStrategies;
using AngleSharp.Diffing.Strategies.TextNodeStrategies;
using Xunit;

namespace Tests
{
    public static class AssertHelpers
    {
        public static void TrimmedEqual(string expected, string actual)
        {
            var expTrimmed = Trim(expected);
            var actTrimmed = Trim(actual);

            Assert.Equal(expTrimmed, actTrimmed);
        }

        public static void TrimmedContains(string expected, string actual)
        {
            var expTrimmed = Trim(expected);
            var actTrimmed = Trim(actual);

            Assert.Contains(expTrimmed, actTrimmed, StringComparison.Ordinal);
        }

        public static void HtmlAssert(string expected, string actual)
        {
            var diffs =
                DiffBuilder
                    .Compare(actual)
                    .WithTest(expected)
                    .WithOptions(options => options
                        .AddAttributeComparer()
                        .AddAttributeNameMatcher()
                        .AddBooleanAttributeComparer(BooleanAttributeComparision.Strict)
                        .AddClassAttributeComparer()
                        .AddCssSelectorMatcher()
                        .AddElementComparer()
                        .AddIgnoreElementSupport()
                        .AddSearchingNodeMatcher()
                        .AddStyleAttributeComparer()
                        .AddStyleSheetComparer()
                        .AddTextComparer(WhitespaceOption.Normalize, ignoreCase: false)
                        .IgnoreDiffAttributes()
                    )
                    .Build();

            var sb = new StringBuilder();

            foreach (var diff in diffs)
            {
                sb.Append(" - ");
                sb.AppendLine(diff.ToString());
            }

            Assert.True(!diffs.Any(), sb.ToString());
        }

        private static string Trim(string value)
        {
            var lines = value.Split('\n');

            return string.Join(Environment.NewLine, lines.Select(x => x.Trim()).Where(x => x.Length > 0));
        }
    }
}
