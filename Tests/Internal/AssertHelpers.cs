using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static void HtmlFileAsset(string name, string actual, bool ignoreComments = false)
        {
            var expected = TestHelper.GetContent(name);

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

            HtmlAssertCore(lhs, rhs, ignoreComments);
        }

        public static void HtmlAssert(string expected, string actual, bool ignoreComments = false)
        {
            var lhs = Cleanup(expected);
            var rhs = Cleanup(actual);

            HtmlAssertCore(lhs, rhs, ignoreComments);
        }

        private static void HtmlAssertCore(string expected, string actual, bool ignoreComments)
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

                        if (ignoreComments)
                        {
                            options.IgnoreComments();
                        }
                    })
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

        private static string Cleanup(string source)
        {
            var regex = new Regex(@"<!--\d{0,}\[(.*)\]\d{0,}>");

            // Replace ending conditional tag to normal comment.
            source = source.Replace("<![endif]-->", "<!-- [endif] -->", StringComparison.OrdinalIgnoreCase);

            // Replace start condition tal to normal comment
            source = regex.Replace(source, x => $"<!-- [{x.Groups[1].Value}] -->");

            return source;
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
                        sb.AppendLine($"   *   Actual: '{diff.Test.Node.Text()}'.");
                        sb.AppendLine($"   * Expected: '{diff.Control.Node.Text()}'.");
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
                        sb.AppendLine($"   *   Actual: '{diff.Test.Attribute.Value}'.");
                        sb.AppendLine($"   * Expected: '{diff.Control.Attribute.Value}'.");
                        break;
                    case AttrDiff diff:
                        sb.AppendLine($"The value of the attribute {diff.Control.Path} and actual attribute {diff.Test.Path} are different.");
                        sb.AppendLine($"   *   Actual: '{diff.Test.Attribute.Value}'.");
                        sb.AppendLine($"   * Expected: '{diff.Control.Attribute.Value}'.");
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
    }
}
