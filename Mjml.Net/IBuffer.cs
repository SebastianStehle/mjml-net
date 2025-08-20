using System.Text;

namespace Mjml.Net;

public interface IBuffer : IDisposable
{
    bool IsEmpty { get; }

    int AppendTo(StringBuilder sb);

    string ToText();
}
