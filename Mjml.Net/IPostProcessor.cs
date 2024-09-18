namespace Mjml.Net;

public interface IPostProcessor
{
    ValueTask<string> PostProcessAsync(string html, MjmlOptions options,
        CancellationToken ct);
}
