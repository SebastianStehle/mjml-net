using System.Text;
using Grynwald.MarkdownGenerator;

namespace Mjml.Net.Compare;

// The default formatter escapes every character with a meaning in Markdown (e.g. '-' or '/'), which makes the raw file hard to read.
public sealed class MinimalTextFormatter : ITextFormatter
{
    private const string SpecialChars = "\\|*_`[]<>";

    public static readonly MinimalTextFormatter Instance = new MinimalTextFormatter();

    public static readonly MdSerializationOptions Options = new MdSerializationOptions { TextFormatter = Instance };

    public string EscapeText(string text)
    {
        var sb = new StringBuilder(text.Length);

        foreach (var c in text)
        {
            if (SpecialChars.Contains(c, StringComparison.Ordinal))
            {
                sb.Append('\\');
            }

            sb.Append(c);
        }

        return sb.ToString();
    }
}
