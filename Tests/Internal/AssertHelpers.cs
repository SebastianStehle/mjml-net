using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
using AngleSharp.Diffing.Strategies.AttributeStrategies;
using AngleSharp.Diffing.Strategies.ElementStrategies;
using AngleSharp.Diffing.Strategies.TextNodeStrategies;
using AngleSharp.Dom;
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
                        options.AddElementComparer();
                        options.AddComparer(ElementClosingComparer.Compare);
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

            void AppendDiff(string message, string? actual = null, string? expected = null)
            {
                sb.AppendLine(message);

                if (actual != null)
                {
                    sb.AppendLine($" * Actual: '{actual}'.");
                }

                if (expected != null)
                {
                    sb.AppendLine($" * Should: '{expected}'.");
                }
            }

            var i = 1;
            foreach (var diff in diffs)
            {
                sb.Append(i);
                sb.Append(' ');

                FormatDiff(diff, AppendDiff);

                i++;
            }

            return sb.ToString();
        }

        private static void FormatDiff(IDiff diff, Action<string, string?, string?> append)
        {
            switch (diff)
            {
                case NodeDiff n:
                    FormatNodeDiff(n, append);
                    break;
                case AttrDiff a:
                    FormatAttrDiff(a, append);
                    break;
                case MissingNodeDiff m:
                    append($"The {Name(m.Control)} at {m.Control.Path} is missing.", null, null);
                    break;
                case MissingAttrDiff m:
                    append($"The attribute at {m.Control.Path} is missing.", null, null);
                    break;
                case UnexpectedNodeDiff u:
                    append($"The {Name(u.Test)} at {u.Test.Path} was not expected.", null, null);
                    break;
                case UnexpectedAttrDiff u:
                    append($"The attribute at {u.Test.Path} was not expected.", null, null);
                    break;
                default:
                    append("Other error", null, null);
                    break;
            }
        }

        private static void FormatNodeDiff(NodeDiff n, Action<string, string?, string?> append)
        {
            if (n.Target == DiffTarget.Text && n.Control.Path.Equals(n.Test.Path, StringComparison.Ordinal))
            {
                append($"The text in {n.Control.Path} is different.", n.Test.Node.Text(), n.Control.Node.Text());
            }
            else if (n.Target == DiffTarget.Text)
            {
                append($"The expected {Name(n.Control)} at {n.Control.Path} and the actual {Name(n.Test)} at {n.Test.Path} is different.", null, null);
            }
            else if (n.Control.Path.Equals(n.Test.Path, StringComparison.Ordinal))
            {
                append($"The {Name(n.Control)}s at {n.Control.Path} are different.", null, null);
            }
            else
            {
                append($"The expected {Name(n.Control)} at {n.Control.Path} and the actual {Name(n.Test)} at {n.Test.Path} are different.", null, null);
            }
        }

        private static void FormatAttrDiff(AttrDiff a, Action<string, string?, string?> append)
        {
            if (a.Control.Path.Equals(a.Test.Path, StringComparison.Ordinal))
            {
                append($"The values of the attributes at {a.Control.Path} are different.", a.Test.Attribute.Value, a.Control.Attribute.Value);
            }
            else
            {
                append($"The value of the attribute {a.Control.Path} and actual attribute {a.Test.Path} are different.", a.Test.Attribute.Value, a.Control.Attribute.Value);
            }
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
