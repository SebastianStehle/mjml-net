using System;
using System.Text;

namespace Mjml.Net;

public sealed class InnerTextOrHtml
{
    private static readonly char[] TrimChars = { ' ', '\n', '\r' };
    private readonly List<string> parts;

    public InnerTextOrHtml(int capacity = 10)
    {
        parts = new List<string>(capacity);
    }

    public InnerTextOrHtml(string input)
    {
        parts = new List<string>(1) { input };
    }

    public void Add(string part)
    {
        parts.Add(part);
    }

    public void AddNonEmpty(string part)
    {
        if (part.AsSpan().IsWhiteSpace())
        {
            return;
        }

        parts.Add(part);
    }

    public bool IsEmpty()
    {
        if (parts.Count == 0)
        {
            return true;
        }

        foreach (var part in parts)
        {
            if (part.AsSpan().IsWhiteSpace())
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

        var lastPart = parts.Count - 1;

        for (var i = 0; i < parts.Count; i++)
        {
            var part = parts[i].AsSpan();

            if (i == 0)
            {
                part = part.TrimStart(TrimChars);
            }

            if (i == lastPart)
            {
                part = part.TrimEnd(TrimChars);
            }

            sb.Append(part);
        }
    }

    public void AppendToIntended(StringBuilder sb, int indent)
    {
        if (parts.Count == 0)
        {
            return;
        }

        var lastPart = parts.Count - 1;

        for (var i = 0; i < parts.Count; i++)
        {
            var part = parts[i].AsSpan();

            if (i == 0)
            {
                part = part.TrimStart(TrimChars);
            }

            if (i == lastPart)
            {
                part = part.TrimEnd(TrimChars);
            }

            AppendIntended(sb, part, indent);
        }
    }

    public static void AppendIntended(StringBuilder sb, ReadOnlySpan<char> span, int indent)
    {
        sb.EnsureCapacity(sb.Length + span.Length);

        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == '\n')
            {
                sb.Append(span[.. (i + 1)]);

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
