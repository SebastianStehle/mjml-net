using System.Globalization;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Parser.Tokens;
using AngleSharp.Text;
using Mjml.Net.Internal;

namespace Mjml.Net
{
    public static partial class XmlFixer
    {
        private static readonly HashSet<string> VoidTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            TagNames.Area,
            TagNames.Base,
            TagNames.Br,
            TagNames.Col,
            TagNames.Embed,
            TagNames.Hr,
            TagNames.Img,
            TagNames.Input,
            TagNames.Link,
            TagNames.Meta,
            TagNames.Param,
            TagNames.Source,
            TagNames.Track,
            TagNames.Wbr
        };

        private static readonly Func<TextSource, IEntityProvider, IDisposable> HtmlTokenizerFactory;
        private static readonly Func<object, HtmlToken> GetMethod;

        static XmlFixer()
        {
            var tokenizerClassType = typeof(HtmlEntityProvider).Assembly.GetType("AngleSharp.Html.Parser.HtmlTokenizer")!;
            var tokenizerConstructor = tokenizerClassType.GetConstructors()[0];

            HtmlTokenizerFactory = tokenizerConstructor.CreateFactory<TextSource, IEntityProvider, IDisposable>();

            GetMethod = tokenizerClassType.GetMethod("Get")!.CreateILDelegate<HtmlToken>();
        }

        public static string Process(string mjml, MjmlOptions options)
        {
            if (options.XHtmlConverter == XHtmlConverterVersion.V2)
            {
                return ProcessV2(mjml);
            }
            else
            {
                return ProcessV1(mjml, options);
            }
        }

        public static string ProcessV2(string mjml)
        {
            var sb = DefaultPools.StringBuilders.Get();
            try
            {
                using var htmlInput = new TextSource(mjml);
                using var htmlReader = HtmlTokenizerFactory(htmlInput, HtmlEntityProvider.Resolver);

                HtmlToken token;
                while ((token = GetMethod(htmlReader)) != null && token.Type != HtmlTokenType.EndOfFile)
                {
                    WriteToken(sb, token);
                }

                return sb.ToString();
            }
            finally
            {
                DefaultPools.StringBuilders.Return(sb);
            }
        }

        private static void WriteToken(StringBuilder sb, HtmlToken token)
        {
            switch (token)
            {
                case HtmlTagToken startTag when token.Type == HtmlTokenType.StartTag:
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"<{startTag.Name}");

                        foreach (var attribute in startTag.Attributes)
                        {
                            sb.Append(' ');
                            sb.Append(attribute.Name);
                            sb.Append('=');
                            sb.Append('"');
                            sb.AppendEscaped(attribute.Value);
                            sb.Append('"');
                        }

                        if (startTag.IsSelfClosing || VoidTags.Contains(startTag.Name))
                        {
                            sb.Append("/>");
                        }
                        else
                        {
                            sb.Append('>');
                        }
                    }
                    break;
                case HtmlToken endTag when token.Type == HtmlTokenType.EndTag:
                    {
                        if (VoidTags.Contains(endTag.Name))
                        {
                            sb.Append(CultureInfo.InvariantCulture, $"<{endTag.Name}/>");
                        }
                        else
                        {
                            sb.Append(CultureInfo.InvariantCulture, $"</{endTag.Name}>");
                        }
                    }
                    break;
                case HtmlToken character when token.Type == HtmlTokenType.Character:
                    sb.AppendEscaped(character.Data);
                    break;
            }
        }

        private static void AppendEscaped(this StringBuilder sb, string text)
        {
            foreach (var c in text)
            {
                if (c == Symbols.LessThan)
                {
                    sb.Append("&lt;");
                }
                else if (c == Symbols.GreaterThan)
                {
                    sb.Append("&gt;");
                }
                else if (c == Symbols.DoubleQuote)
                {
                    sb.Append("&#34;");
                }
                else if (c == Symbols.SingleQuote)
                {
                    sb.Append("&#39;");
                }
                else if (c == Symbols.Ampersand)
                {
                    sb.Append("&amp;");
                }
                else
                {
                    sb.Append(c);
                }
            }
        }

        public static string ProcessV1(string mjml, MjmlOptions options)
        {
            var sb = DefaultPools.StringBuilders.Get();

            try
            {
                var span = mjml.AsSpan();

                static ReadOnlySpan<char> FixNextSequence(ReadOnlySpan<char> span, StringBuilder sb, MjmlOptions options)
                {
                    var nextEntityOrTag = span.IndexOfAny('&', '<');

                    if (nextEntityOrTag < 0)
                    {
                        // Add everything that is left.
                        sb.Append(span);
                        return default;
                    }

                    if (nextEntityOrTag > 0)
                    {
                        // Add everything to the character.
                        sb.Append(span[..nextEntityOrTag]);

                        // Move to the character.
                        span = span[nextEntityOrTag..];
                    }

                    var first = span[0];

                    if (first == '&')
                    {
                        if (!IsEntity(span))
                        {
                            sb.Append("&amp;");
                        }
                        else
                        {
                            sb.Append(first);
                        }
                    }
                    else if (StartWithIgnoreWhitespace(span, "<br*>", out var charsRead))
                    {
                        var afterBr = span[charsRead..];

                        if (!StartWithIgnoreWhitespace(afterBr, "*</br*>", out _))
                        {
                            sb.Append("<br />");
                            return afterBr;
                        }
                        else
                        {
                            sb.Append(first);
                        }
                    }
                    else
                    {
                        sb.Append(first);
                    }

                    // Skip the start character.
                    return span[1..];
                }

                while (span.Length > 0)
                {
                    span = FixNextSequence(span, sb, options);
                }

                return sb.ToString();
            }
            finally
            {
                DefaultPools.StringBuilders.Return(sb);
            }
        }

        private static bool IsEntity(ReadOnlySpan<char> input)
        {
            var indexOfEnding = input.IndexOf(';');

            if (indexOfEnding < 0 || indexOfEnding > 5)
            {
                return false;
            }

            for (var i = 1; i < indexOfEnding - 1; i++)
            {
                var c = input[i];

                if (!char.IsLetterOrDigit(c) && c != '#')
                {
                    return false;
                }
            }

            return true;
        }

        private static bool StartWithIgnoreWhitespace(ReadOnlySpan<char> input, string test, out int charsRead)
        {
            var i = 0;
            foreach (var testCharacter in test)
            {
                if (i >= input.Length)
                {
                    charsRead = -1;
                    return false;
                }

                var c = input[i];

                if (testCharacter == '*')
                {
                    while (char.IsWhiteSpace(c) && i < input.Length - 2)
                    {
                        i++;
                        c = input[i];
                    }

                    continue;
                }

                if (char.ToUpperInvariant(testCharacter) != char.ToUpperInvariant(c))
                {
                    charsRead = -1;
                    return false;
                }

                i++;
            }

            charsRead = i;
            return true;
        }
    }
}
