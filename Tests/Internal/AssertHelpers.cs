using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
using AngleSharp.Diffing.Strategies.AttributeStrategies;
using AngleSharp.Diffing.Strategies.TextNodeStrategies;
using AngleSharp.Dom;
using Xunit;

#pragma warning disable MA0011 // IFormatProvider is missing

namespace Tests.Internal
{
    public static class AssertHelpers
    {
        public static void TrimmedEqual(string expected, string actual)
        {
            var lhs = Trim(expected);
            var rhs = Trim(actual);

            Assert.Equal(lhs, rhs);
        }

        public static void TrimmedContains(string expected, string actual)
        {
            var lhs = Trim(expected);
            var rhs = Trim(actual);

            Assert.Contains(lhs, rhs, StringComparison.Ordinal);
        }

        private static string Trim(string value)
        {
            var lines = value.Split('\n');

            return string.Join(Environment.NewLine, lines.Select(x => x.Trim()).Where(x => x.Length > 0));
        }

        public static void HtmlFileAssert(string name, string actual, bool ignoreIds = false)
        {
            var expected = TestHelper.GetContent(name);

            HtmlAssert(name, actual, expected, ignoreIds);
        }

        public static void HtmlAssert(string name, string actual, string expected, bool ignoreIds = false)
        {
            var lhs = Cleanup(expected);
            var rhs = Cleanup(actual);

            try
            {
                File.WriteAllText($"{name}.expected.html", lhs);
                File.WriteAllText($"{name}.actual.html", rhs);
            }
            catch (IOException)
            {
            }

            HtmlAssertCore(lhs, rhs, ignoreIds);
        }

        public static void HtmlAssert(string expected, string actual, bool ignoreIds = false)
        {
            var lhs = Cleanup(expected);
            var rhs = Cleanup(actual);

            HtmlAssertCore(lhs, rhs, ignoreIds);
        }

        private static void HtmlAssertCore(string expected, string actual, bool ignoreIds)
        {
            var diffs =
                DiffBuilder
                    .Compare(expected)
                    .WithTest(actual)
                    .WithOptions(options =>
                    {
                        options.AddAttributeComparer();
                        options.AddAttributeNameMatcher();
                        options.AddBooleanAttributeComparer(BooleanAttributeComparision.Strict);
                        options.AddClassAttributeComparer();
                        options.AddCssSelectorMatcher();
                        options.AddElementComparer();
                        options.AddIgnoreElementSupport();
                        options.AddSearchingNodeMatcher();
                        options.AddStyleAttributeComparer(ignoreOrder: true);
                        options.AddStyleSheetComparer();
                        options.AddTextComparer(WhitespaceOption.Normalize, ignoreCase: false);
                        options.IgnoreDiffAttributes();
                        options.IgnoreCommentContent();
                        options.IgnoreEmptyAttributes();

                        if (ignoreIds)
                        {
                            options.IgnoreAttribute("for");
                            options.IgnoreAttribute("id");
                        }
                    })
                    .Build();

            Assert.True(!diffs.Any(), FormatDiffs(diffs));
        }

        private static string Cleanup(string source)
        {
            // Replace ending negated conditional comment to normal comment.
            source = source.Replace("<!--<![endif]-->", "<!-- [endif] -->", StringComparison.OrdinalIgnoreCase);

            // Replace ending conditional comment to normal comment.
            source = source.Replace("<![endif]-->", "<!-- [endif] -->", StringComparison.OrdinalIgnoreCase);

            // Replace starting negated condition comment to normal comment
            source = Regex.Replace(source, @"<!--\d{0,}\[(.*)\]\d{0,}><!-->", x =>
            {
                var text = x.Groups[1].Value.Trim('-', '<', '>', '!');

                return $"<!-- [${text}] -->";
            });

            // Replace starting condition comment to normal comment
            source = Regex.Replace(source, @"<!--\d{0,}\[(.*)\]\d{0,}>", x =>
            {
                var text = x.Groups[1].Value.Trim('-', '<', '>', '!');

                return $"<!-- [${text}] -->";
            });

            return source;
        }

        private static string FormatDiffs(IEnumerable<IDiff> diffs)
        {
            var sb = new StringBuilder();

            var i = 1;
            foreach (var diff in diffs)
            {
                sb.Append(' ');
                sb.Append(i);
                sb.Append(' ');

                FormatDiff(diff, sb);

                i++;
            }

            return sb.ToString();
        }

        private static void FormatDiff(IDiff diff, StringBuilder sb)
        {
            switch (diff)
            {
                case NodeDiff n:
                    FormatNodeDiff(n, sb);
                    break;
                case AttrDiff a:
                    FormatAttrDiff(a, sb);
                    break;
                case MissingNodeDiff m:
                    sb.AppendDiff($"The {NodeName(m.Control)} at {m.Control.Path} is missing.");
                    break;
                case MissingAttrDiff m:
                    sb.AppendDiff($"The attribute at {m.Control.Path} is missing.");
                    break;
                case UnexpectedNodeDiff u:
                    sb.AppendDiff($"The {NodeName(u.Test)} at {u.Test.Path} was not expected.");
                    break;
                case UnexpectedAttrDiff u:
                    sb.AppendDiff($"The attribute at {u.Test.Path} was not expected.");
                    break;
                default:
                    sb.AppendDiff("Other error");
                    break;
            }
        }

        private static void FormatNodeDiff(NodeDiff n, StringBuilder sb)
        {
            if (n.Target == DiffTarget.Text && n.Control.Path.Equals(n.Test.Path, StringComparison.Ordinal))
            {
                sb.AppendDiff($"The text in {n.Control.Path} is different.", n.Test.Node.Text(), n.Control.Node.Text());
            }
            else if (n.Target == DiffTarget.Text)
            {
                sb.AppendDiff($"The expected {NodeName(n.Control)} at {n.Control.Path} and the actual {NodeName(n.Test)} at {n.Test.Path} is different.");
            }
            else if (n.Control.Path.Equals(n.Test.Path, StringComparison.Ordinal))
            {
                sb.AppendDiff($"The {NodeName(n.Control)}s at {n.Control.Path} are different.");
            }
            else
            {
                sb.AppendDiff($"The expected {NodeName(n.Control)} at {n.Control.Path} and the actual {NodeName(n.Test)} at {n.Test.Path} are different.");
            }
        }

        private static void FormatAttrDiff(AttrDiff a, StringBuilder sb)
        {
            if (a.Control.Path.Equals(a.Test.Path, StringComparison.Ordinal))
            {
                sb.AppendDiff($"The values of the attributes at {a.Control.Path} are different.", a.Test.Attribute.Value, a.Control.Attribute.Value);
            }
            else
            {
                sb.AppendDiff($"The value of the attribute {a.Control.Path} and actual attribute {a.Test.Path} are different.", a.Test.Attribute.Value, a.Control.Attribute.Value);
            }
        }

        private static void AppendDiff(this StringBuilder sb, string message, string? actual = null, string? expected = null)
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

        private static string NodeName(ComparisonSource source)
        {
            return source.Node.NodeType.ToString().ToLowerInvariant();
        }
    }
}
