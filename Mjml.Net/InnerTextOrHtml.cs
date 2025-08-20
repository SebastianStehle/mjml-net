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

    public void Add(string part)
    {
        parts.Add(part);
    }

    public bool IsEmpty()
    {
        return !parts.Exists(x => !x.AsSpan().IsWhiteSpace());
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

        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == '\n')
            {
                sb.Append(span[..(i + 1)]);

                // Add space characters before each line.
                WriteLineStart(sb, indent);

                // Start the span after the newline.
                span = span[(i + 1)..];
                i = 0;
            }
        }

        sb.Append(span);
    }

    private static void WriteLineStart(StringBuilder sb, int indent)
    {
        for (var i = 0; i < indent; i++)
        {
            sb.Append(' ');
        }
    }
}
