using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Css.Dom;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
using AngleSharp.Diffing.Strategies.AttributeStrategies;
using AngleSharp.Diffing.Strategies.TextNodeStrategies;
using AngleSharp.Dom;
using Xunit;

#pragma warning disable MA0011 // IFormatProvider is missing

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
            // We use a lot of conditional comments in html. These comments are treated as comments, which makes them hard to diff.
            // If we replace them as normal comments the inner html become normal nodes and we can compare them.
            static string Cleanup(string source)
            {
                var regex = new Regex(@"<!--\d{0,}\[(.*)\]\d{0,}>");

                // Replace ending conditional tag to normal comment.
                source = source.Replace("<![endif]-->", "<!-- [endif] -->", StringComparison.OrdinalIgnoreCase);

                // Replace start condition tal to normal comment
                source = regex.Replace(source, x => $"<!-- [{x.Groups[1].Value}] -->");

                return source;
            }

            var diffs =
                DiffBuilder
                    .Compare(Cleanup(expected))
                    .WithTest(Cleanup(actual))
                    .WithOptions(options => options
                        .AddAttributeComparer()
                        .AddAttributeNameMatcher()
                        .AddBooleanAttributeComparer(BooleanAttributeComparision.Strict)
                        .AddClassAttributeComparer()
                        .AddCssSelectorMatcher()
                        .AddElementComparer()
                        .AddIgnoreElementSupport()
                        .AddSearchingNodeMatcher()
                        .AddStyleAttributeComparer(ignoreOrder: true)
                        .AddStyleSheetComparer()
                        .AddTextComparer(WhitespaceOption.Normalize, ignoreCase: false)
                        .IgnoreDiffAttributes()
                    )
                    .Build();

            var cleaned = diffs.Where(d =>
            {
                // Ingore unexpected empty attributes, because mjml sometimes renders style=""
                if (d is UnexpectedAttrDiff u && string.IsNullOrEmpty(u.Test.Attribute.Value))
                {
                    return false;
                }

                // Ingore missing empty attributes.
                if (d is MissingAttrDiff m && string.IsNullOrEmpty(m.Control.Attribute.Value))
                {
                    return false;
                }

                // Some problem with comments diff.
                if (d is NodeDiff n && n.Control.Path.Equals(n.Test.Path, StringComparison.Ordinal) && n.Control.Node.NodeType == NodeType.Comment)
                {
                    return false;
                }

                return true;
            }).ToList();

            Assert.True(!cleaned.Any(), PrintDiffs(cleaned));
        }

        private static string PrintDiffs(IEnumerable<IDiff> diffs)
        {
            var sb = new StringBuilder();

            var i = 1;
            foreach (var d in diffs)
            {
                sb.Append(' ');
                sb.Append(i);
                sb.Append(' ');

                switch (d)
                {
                    case NodeDiff diff when diff.Target == DiffTarget.Text && diff.Control.Path.Equals(diff.Test.Path, StringComparison.Ordinal):
                        sb.AppendLine($"The text in {diff.Control.Path} is different.");
                        sb.AppendLine($"   * Test:    '{diff.Test.Node.Text()}'.");
                        sb.AppendLine($"   * Control: '{diff.Control.Node.Text()}'.");
                        break;
                    case NodeDiff diff when diff.Target == DiffTarget.Text:
                        sb.AppendLine($"The expected {NodeName(diff.Control)} at {diff.Control.Path} and the actual {NodeName(diff.Test)} at {diff.Test.Path} is different.");
                        break;
                    case NodeDiff diff when diff.Control.Path.Equals(diff.Test.Path, StringComparison.Ordinal):
                        sb.AppendLine($"The {NodeName(diff.Control)}s at {diff.Control.Path} are different.");
                        break;
                    case NodeDiff diff:
                        sb.AppendLine($"The expected {NodeName(diff.Control)} at {diff.Control.Path} and the actual {NodeName(diff.Test)} at {diff.Test.Path} are different.");
                        break;
                    case AttrDiff diff when diff.Control.Path.Equals(diff.Test.Path, StringComparison.Ordinal):
                        sb.AppendLine($"The values of the attributes at {diff.Control.Path} are different.");
                        sb.AppendLine($"   * Test:    '{diff.Test.Attribute.Value}'.");
                        sb.AppendLine($"   * Control: '{diff.Control.Attribute.Value}'.");
                        break;
                    case AttrDiff diff:
                        sb.AppendLine($"The value of the attribute {diff.Control.Path} and actual attribute {diff.Test.Path} are different.");
                        break;
                    case MissingNodeDiff diff:
                        sb.AppendLine($"The {NodeName(diff.Control)} at {diff.Control.Path} is missing.");
                        break;
                    case MissingAttrDiff diff:
                        sb.AppendLine($"The attribute at {diff.Control.Path} is missing.");
                        break;
                    case UnexpectedNodeDiff diff:
                        sb.AppendLine($"The {NodeName(diff.Test)} at {diff.Test.Path} was not expected.");
                        break;
                    case UnexpectedAttrDiff diff:
                        sb.AppendLine($"The attribute at {diff.Test.Path} was not expected.");
                        break;
                    default:
                        sb.AppendLine("Other error");
                        break;
                }

                i++;
            }

            return sb.ToString();

            static string NodeName(ComparisonSource source) => source.Node.NodeType.ToString().ToLowerInvariant();
        }

        private static string Trim(string value)
        {
            var lines = value.Split('\n');

            return string.Join(Environment.NewLine, lines.Select(x => x.Trim()).Where(x => x.Length > 0));
        }
    }
}
