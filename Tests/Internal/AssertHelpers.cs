using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
using AngleSharp.Diffing.Core.Diffs;
using AngleSharp.Diffing.Strategies.AttributeStrategies;
using AngleSharp.Diffing.Strategies.TextNodeStrategies;
using AngleSharp.Dom;
using Mjml.Net;

#pragma warning disable MA0011 // IFormatProvider is missing

namespace Tests.Internal;

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
                .Compare(expected)
                .WithTest(actual)
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
                    options.IgnoreEmptyStyles();
                    options.IgnoreElement("br");

                    if (ignoreIds)
                    {
                        options.IgnoreAttribute("for");
                        options.IgnoreAttribute("id");
                    }
                })
                .Build().ToList();

        Assert.True(diffs.Count == 0, FormatDiffs(diffs));
    }

    private static string FormatDiffs(IEnumerable<IDiff> diffs)
    {
        var sb = new StringBuilder();

        var i = 1;
        foreach (var diff in diffs)
        {
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
            case ElementDiff d when d.Kind == ElementDiffKind.ClosingStyle:
                sb.AppendDiff($"Different closing style at {d.Control.Path}.");
                break;
            case ElementDiff d when d.Kind == ElementDiffKind.Name:
                sb.AppendDiff($"Different element names at {d.Control.Path}.", Name(d.Test), Name(d.Control));
                break;
            case CommentDiff d:
                sb.AppendDiff($"Different comments at {d.Control.Path}.", d.Test.Node.Text(), d.Control.Node.Text());
                break;
            case TextDiff d:
                sb.AppendDiff($"Different texts at {d.Control.Path}.", d.Test.Node.Text(), d.Control.Node.Text());
                break;
            case AttrDiff d when d.Kind == AttrDiffKind.Name:
                sb.AppendDiff($"Different attribute names at {d.Control.Path}.", d.Test.Attribute.Name, d.Control.Attribute.Name);
                break;
            case AttrDiff d when d.Kind == AttrDiffKind.Value:
                sb.AppendDiff($"Different attribute values at {d.Control.Path}.", d.Test.Attribute.Value, d.Control.Attribute.Value);
                break;
            case MissingNodeDiff d:
                sb.AppendDiff($"Missing node {Name(d.Control)} at {d.Control.Path}.");
                break;
            case MissingAttrDiff d:
                sb.AppendDiff($"Missing attribute {d.Control.Attribute.Name} at {d.Control.Path}.");
                break;
            case UnexpectedNodeDiff d:
                sb.AppendDiff($"Unexpected node at {d.Test.Path}.");
                break;
            case UnexpectedAttrDiff d:
                sb.AppendDiff($"Unexpected attribute at {d.Test.Path}.");
                break;
            default:
                sb.AppendDiff("Other error");
                break;
        }
    }

    private static string Name(this ComparisonSource source)
    {
        return source.Node.NodeType.ToString().ToLowerInvariant();
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
