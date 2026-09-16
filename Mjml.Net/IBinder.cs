namespace Mjml.Net;

public interface IBinder
{
    string[] ClassNames { get; }

    IReadOnlyDictionary<string, string> Attributes { get; }

    InnerTextOrHtml? GetText();
}
