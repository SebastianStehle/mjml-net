using System.Text;

namespace Mjml.Net;

public interface IBuffer : IDisposable
{
    bool IsEmpty { get; }

    int AppendToAndDispose(StringBuilder sb);

    string ToText();
}
