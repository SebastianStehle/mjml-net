using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
using AngleSharp.Diffing.Strategies.AttributeStrategies;
using AngleSharp.Diffing.Strategies.TextNodeStrategies;
using Mjml.Net;
using Xunit;

#pragma warning disable MA0011 // IFormatProvider is missing

namespace Tests.Internal
{
    public static partial class AssertHelpers
    {
        public static void MultilineText(MjmlRenderContext sut, params string[] lines)
        {
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                sb.AppendLine(line.Replace('\'', '"'));
            }

            var actual = sut.EndBuffer()!.ToString();

            Assert.Equal(sb.ToString(), actual);
        }

        public static void HtmlFileAssert(string name, string actual, bool ignoreIds = false)
        {
            var expected = TestHelper.GetContent(name);

            HtmlAssert(name, actual, expected, ignoreIds);
        }

        public static void HtmlAssert(string fileName, string actual, string expected, bool ignoreIds = false)
        {
            HtmlAssertCore(expected, actual, ignoreIds, fileName);
        }

        public static void HtmlAssert(string expected, string actual, bool ignoreIds = false)
        {
            HtmlAssertCore(expected, actual, ignoreIds, null);
        }

        private static void HtmlAssertCore(string expected, string actual, bool ignoreIds, string? fileName)
        {
            expected =
                expected
                    .ConvertConditionalComment()
                    .ConvertNegatedConditionalComment();

            actual =
                actual
                    .ConvertConditionalComment()
                    .ConvertNegatedConditionalComment();

            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    File.WriteAllText($"{fileName}.expected.html", expected);
                    File.WriteAllText($"{fileName}.actual.html", actual);
                }
                catch (IOException)
                {
                }
            }

            var diffs =
                DiffBuilder
                    .Compare(actual)
                    .WithTest(expected)
                    .WithOptions(options =>
                    {
                        options.AddAttributeComparer();
                        options.AddAttributeNameMatcher();
                        options.AddBooleanAttributeComparer(BooleanAttributeComparision.Strict);
                        options.AddClassAttributeComparer();
                        options.AddCssSelectorMatcher();
                        options.AddElementComparer(true);
                        options.AddIgnoreElementSupport();
                        options.AddSearchingNodeMatcher();
                        options.AddStyleAttributeComparer(ignoreOrder: true);
                        options.AddStyleSheetComparer();
                        options.AddTextComparer(WhitespaceOption.Normalize, ignoreCase: false);
                        options.IgnoreDiffAttributes();
                        options.IgnoreCommentContent();
                        options.IgnoreEmptyAttributes();
                        options.IgnoreElement("br");

                        if (ignoreIds)
                        {
                            options.IgnoreAttribute("for");
                            options.IgnoreAttribute("id");
                        }
                    })
                    .Build().ToList();

            Assert.True(!diffs.Any(), FormatDiffs(diffs));
        }

        private static string FormatDiffs(IEnumerable<IDiff> diffs)
        {
            var sb = new StringBuilder();

            foreach (var diff in diffs)
            {
                sb.Append(" - ");
                sb.AppendLine(diff.ToString()!);
            }

            return sb.ToString();
        }

        private static string Name(this ComparisonSource source)
        {
            return source.Node.NodeType.ToString().ToLowerInvariant();
        }

        private static string ConvertNegatedConditionalComment(this string source)
        {
            source = source.Replace("<!--<![endif]-->", "<!-- [endif] -->", StringComparison.OrdinalIgnoreCase);

            source = NegatedConditionalCommentStart().Replace(source, x =>
            {
                var text = x.Groups[1].Value.Trim('-', '<', '>', '!');

                return $"<!-- [${text}] -->";
            });

            return source;
        }

        private static string ConvertConditionalComment(this string source)
        {
            source = source.Replace("<![endif]-->", "<!-- [endif] -->", StringComparison.OrdinalIgnoreCase);

            source = ConditionalCommentStart().Replace(source, x =>
            {
                var text = x.Groups[1].Value.Trim('-', '<', '>', '!');

                return $"<!-- [${text}] -->";
            });

            return source;
        }

        [GeneratedRegex(@"<!--\d{0,}\[(.*)\]\d{0,}>")]
        private static partial Regex ConditionalCommentStart();

        [GeneratedRegex(@"<!--\d{0,}\[(.*)\]\d{0,}><!-->")]
        private static partial Regex NegatedConditionalCommentStart();
    }
}
