using System.Text;

#pragma warning disable CA1822 // Mark members as static

namespace Mjml.Net;

internal sealed class RenderBuffer
{
    private readonly StringBuilder sb = new StringBuilder();
    private readonly bool beautify;
    private bool elementSelfClosed;
    private bool elementStarted;
    private int numClasses;
    private int numStyles;
    private int indent;
    private string? pendingConditionalStart;
    private string? pendingConditionalEnd;

    public RenderBuffer(StringBuilder sb, bool beautify)
    {
        this.sb = sb;
        this.beautify = beautify;
    }

    public StringBuilder StringBuilder
    {
        get => sb!;
    }

    public void ReturnStringBuilder(StringBuilder stringBuilder)
    {
        DefaultPools.StringBuilders.Return(stringBuilder);
    }

    public void FlushAll()
    {
        FlushElement();
        FlushConditionalStart();
        FlushConditionalEnd();
    }

    public void StartElement(string elementName, bool close = false)
    {
        FlushElement();
        FlushConditionalStart();
        FlushConditionalEnd();

        if (string.IsNullOrEmpty(elementName))
        {
            return;
        }

        WriteLineStart();

        sb.Append('<');
        sb.Append(elementName);

        elementSelfClosed = close;
        elementStarted = true;
        numClasses = 0;
        numStyles = 0;
    }

    public void Attr(string name, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        StartAttr(name);
        sb.Append(value);
        EndAttr();
    }

    public void StartAttr(string name)
    {
        sb.Append(' ');
        sb.Append(name);
        sb.Append("=\"");
    }

    public void EndAttr()
    {
        sb.Append('"');
    }

    public void Class(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        StartClass();
        sb.Append(value);
        EndClass();
    }

    public void StartClass()
    {
        if (numClasses == 0)
        {
            // Open the class attribute.
            sb.Append(" class=\"");
        }
        else
        {
            sb.Append(' ');
        }
    }

    public void EndClass()
    {
        numClasses++;
    }

    public void Style(string name, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        StartStyle(name);
        sb.Append(value);
        EndStyle();
    }

    public void StartStyle(string name)
    {
        if (numClasses > 0)
        {
            // Close the open class attribute.
            sb.Append("\" ");

            // Reset the number of classes so we do not close it again in the flush.
            numClasses = 0;
        }

        if (numStyles == 0)
        {
            // Open the styles attribute.
            sb.Append(" style=\"");
        }

        sb.Append(name);
        sb.Append(':');
    }

    public void EndStyle()
    {
        sb.Append(';');

        numStyles++;
    }

    public void EndElement(string elementName)
    {
        FlushElement();
        FlushConditionalStart();
        FlushConditionalEnd();

        indent--;

        WriteLineStart();

        sb.Append("</");
        sb.Append(elementName);
        sb.Append('>');

        WriteLineEnd();
    }

    private void FlushElement()
    {
        if (!elementStarted)
        {
            return;
        }

        if (numClasses > 0 || numStyles > 0)
        {
            // Close the open class or style attribute.
            sb.Append('\"');
        }

        if (elementSelfClosed)
        {
            sb.Append("/>");
        }
        else
        {
            indent++;
            sb.Append('>');
        }

        WriteLineEnd();

        elementStarted = false;
        elementSelfClosed = false;
    }

    public void Content(string? value)
    {
        FlushElement();

        if (value == null)
        {
            return;
        }

        TextCore(value);
    }

    public void Content(InnerTextOrHtml? value)
    {
        FlushElement();

        if (value == null)
        {
            return;
        }

        TextCore(value);
    }

    private void TextCore(string value)
    {
        WriteLineStart();

        if (beautify)
        {
            InnerTextOrHtml.AppendIntended(sb, value.AsSpan(), indent * 2);
        }
        else
        {
            sb.Append(value);
        }

        WriteLineEnd();
    }

    private void TextCore(InnerTextOrHtml value)
    {
        WriteLineStart();

        if (beautify)
        {
            value.AppendToIntended(sb, indent * 2);
        }
        else
        {
            value.AppendTo(sb);
        }

        WriteLineEnd();
    }

    public void StartText()
    {
        FlushElement();
        FlushConditionalStart();
        FlushConditionalEnd();

        WriteLineStart();
    }

    public void Plain(StringBuilder? value)
    {
        FlushElement();
        FlushConditionalStart();
        FlushConditionalEnd();

        if (value?.Length == 0)
        {
            return;
        }

        sb.Append(value);

        WriteLineEnd();
    }

    public void Plain(InnerTextOrHtml value)
    {
        FlushElement();
        FlushConditionalStart();
        FlushConditionalEnd();

        if (value.IsEmpty())
        {
            return;
        }

        value.AppendTo(sb);

        WriteLineEnd();
    }

    public void Plain(ReadOnlySpan<char> value)
    {
        FlushElement();
        FlushConditionalStart();
        FlushConditionalEnd();

        if (value.Length == 0)
        {
            return;
        }

        sb.Append(value);

        WriteLineEnd();
    }

    public void WriteLineEnd()
    {
        if (beautify)
        {
            sb.AppendLine();
        }
    }

    private void WriteLineStart()
    {
        if (beautify)
        {
            for (var i = 0; i < indent; i++)
            {
                sb.Append("  ");
            }
        }
    }

    public void StartConditional(string content)
    {
        FlushElement();

        if (pendingConditionalEnd != null)
        {
            pendingConditionalEnd = null;
        }
        else
        {
            pendingConditionalStart = content;
        }
    }

    public void EndConditional(string content)
    {
        FlushElement();

        pendingConditionalEnd = content;
    }

    private void FlushConditionalEnd()
    {
        if (pendingConditionalEnd != null)
        {
            TextCore(pendingConditionalEnd);
            pendingConditionalEnd = null;
        }
    }

    private void FlushConditionalStart()
    {
        if (pendingConditionalStart != null)
        {
            TextCore(pendingConditionalStart);
            pendingConditionalStart = null;
        }
    }
}
