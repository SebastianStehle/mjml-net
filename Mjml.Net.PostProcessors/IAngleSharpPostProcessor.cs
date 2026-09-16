using AngleSharp.Dom;

namespace Mjml.Net;

public interface IAngleSharpPostProcessor
{
    /// <summary>
    /// Checks the rendered HTML before it is parsed. If no processor needs to process the HTML, it is returned as it is.
    /// </summary>
    /// <param name="html">The rendered HTML.</param>
    /// <returns>True, if the document needs to be processed.</returns>
    bool ShouldProcess(string html)
    {
        return true;
    }

    ValueTask ProcessAsync(IDocument document, MjmlOptions options,
        CancellationToken ct);
}
