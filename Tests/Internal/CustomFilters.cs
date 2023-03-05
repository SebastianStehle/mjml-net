using AngleSharp.Diffing.Core;
using AngleSharp.Diffing.Strategies;
using AngleSharp.Dom;
using AngleSharp.Html.Parser.Tokens;

namespace Tests.Internal
{
    public static class CustomFilters
    {
        public static void IgnoreAttribute(this IDiffingStrategyCollection builder, string name)
        {
            builder.AddFilter((in AttributeComparisonSource source, FilterDecision currentDecision) =>
            {
                if (currentDecision == FilterDecision.Exclude)
                {
                    return currentDecision;
                }

                if (source.Attribute.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return FilterDecision.Exclude;
                }

                return currentDecision;
            });
        }

        public static void IgnoreEmptyAttributes(this IDiffingStrategyCollection builder)
        {
            builder.AddFilter((in AttributeComparisonSource source, FilterDecision currentDecision) =>
            {
                if (currentDecision.IsExclude())
                {
                    return currentDecision;
                }

                if (string.IsNullOrWhiteSpace(source.Attribute.Value))
                {
                    return FilterDecision.Exclude;
                }

                return currentDecision;
            });
        }

        public static void IgnoreCommentContent(this IDiffingStrategyCollection builder)
        {
            builder.AddComparer((in Comparison source, CompareResult currentDecision) =>
            {
                if (currentDecision == CompareResult.Skip)
                {
                    return currentDecision;
                }

                if (currentDecision == CompareResult.Different && source.Test.Node.NodeType == NodeType.Comment)
                {
                    return CompareResult.Skip;
                }

                return currentDecision;
            });
        }
    }
}
