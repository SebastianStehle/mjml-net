using System.Text;

namespace Mjml.Net;

public sealed class InnerTextOrHtml
{
    private delegate void Formatter<T>(ReadOnlySpan<char> span, T args);
    private static readonly char[] TrimChars = [' ', '\n', '\r'];
    private readonly List<string> parts;

    public InnerTextOrHtml(int capacity = 10)
    {
        parts = new List<string>(capacity);
    }

    public InnerTextOrHtml(string input)
    {
        parts = [input];
    }

    public string? SinglePart()
    {
        if (parts.Count == 1)
        {
            return parts[0];
        }

        return null;
    }

    public void Add(string part)
    {
        parts.Add(part);
    }

    public bool IsEmpty()
    {
        for (var i = 0; i < parts.Count; i++)
        {
            if (!parts[i].AsSpan().IsWhiteSpace())
            {
                return false;
            }
        }

        return true;
    }

    public void AppendTo(StringBuilder sb)
    {
        if (parts.Count == 0)
        {
            return;
        }

        AppendCore(sb, (span, sb) => sb.Append(span));
    }

    public void AppendToIntended(StringBuilder sb, int indent)
    {
        if (parts.Count == 0)
        {
            return;
        }

        AppendCore((sb, indent), (span, args) => AppendIntended(args.sb, span, args.indent));
    }

    private void AppendCore<T>(T args, Formatter<T> formatter)
    {
        if (parts.Count == 0)
        {
            return;
        }

        var sliceStart = 0;
        var sliceEnd = parts.Count - 1;

        // Skip over all strings at the end that contain only whitespaces, because we cannot do that in the loop.
        while (sliceEnd > sliceStart)
        {
            if (parts[sliceEnd].AsSpan().IsWhiteSpace())
            {
                sliceEnd--;
            }
            else
            {
                break;
            }
        }

        for (var i = sliceStart; i <= sliceEnd; i++)
        {
            var part = parts[i].AsSpan();

            // Trim only the first and last element, because they do not contain whitespaces.
            if (i == sliceStart)
            {
                part = part.TrimStart(TrimChars);

                if (part.Length == 0)
                {
                    sliceStart++;
                    continue;
                }
            }

            if (i == sliceEnd)
            {
                part = part.TrimEnd(TrimChars);
            }

            formatter(part, args);
        }
    }

    public static void AppendIntended(StringBuilder sb, ReadOnlySpan<char> span, int indent)
    {
        sb.EnsureCapacity(sb.Length + span.Length);

        // IndexOf is vectorized, which is faster than checking each character.
        int index;
        while ((index = span.IndexOf('\n')) >= 0)
        {
            sb.Append(span[..(index + 1)]);

            // Add space characters before each line.
            WriteLineStart(sb, indent);

            span = span[(index + 1)..];
        }

        sb.Append(span);
    }

    private static void WriteLineStart(StringBuilder sb, int indent)
    {
        if (indent > 0)
        {
            sb.Append(' ', indent);
        }
    }
}
