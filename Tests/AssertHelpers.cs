using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
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

            Assert.True(!diffs.Any(), PrintDiffs(diffs));
        }

        private static string PrintDiffs(IEnumerable<IDiff> diffs)
        {
            return string.Join(Environment.NewLine, diffs.Select((x, i) =>
            {
                var diffText = x switch
                {
                    NodeDiff diff when diff.Target == DiffTarget.Text && diff.Control.Path.Equals(diff.Test.Path, StringComparison.Ordinal)
                        => $"The text in {diff.Control.Path} is different.",
                    NodeDiff diff when diff.Target == DiffTarget.Text
                        => $"The expected {NodeName(diff.Control)} at {diff.Control.Path} and the actual {NodeName(diff.Test)} at {diff.Test.Path} is different.",
                    NodeDiff diff when diff.Control.Path.Equals(diff.Test.Path, StringComparison.Ordinal)
                        => $"The {NodeName(diff.Control)}s at {diff.Control.Path} are different.",
                    NodeDiff diff
                        => $"The expected {NodeName(diff.Control)} at {diff.Control.Path} and the actual {NodeName(diff.Test)} at {diff.Test.Path} are different.",
                    AttrDiff diff when diff.Control.Path.Equals(diff.Test.Path, StringComparison.Ordinal)
                        => $"The values of the attributes at {diff.Control.Path} are different.",
                    AttrDiff diff
                        => $"The value of the attribute {diff.Control.Path} and actual attribute {diff.Test.Path} are different.",
                    MissingNodeDiff diff
                        => $"The {NodeName(diff.Control)} at {diff.Control.Path} is missing.",
                    MissingAttrDiff diff
                        => $"The attribute at {diff.Control.Path} is missing.",
                    UnexpectedNodeDiff diff
                        => $"The {NodeName(diff.Test)} at {diff.Test.Path} was not expected.",
                    UnexpectedAttrDiff diff
                        => $"The attribute at {diff.Test.Path} was not expected.",
                    _ => throw new InvalidOperationException($"Unknown diff type detected: {x.GetType()}"),
                };
                return $"  {i + 1}: {diffText}";
            })) + Environment.NewLine;

            static string NodeName(ComparisonSource source) => source.Node.NodeType.ToString().ToLowerInvariant();
        }

        private static string Trim(string value)
        {
            var lines = value.Split('\n');

            return string.Join(Environment.NewLine, lines.Select(x => x.Trim()).Where(x => x.Length > 0));
        }
    }
}
