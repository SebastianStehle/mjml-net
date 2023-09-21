using Mjml.Net;
using Mjml.Net.Components;

namespace Tests.Internal;

/// <summary>
/// Provides the files from an in memory store.
/// </summary>
/// <remarks>
/// Useful for preloading.
/// </remarks>
public sealed class InMemoryFileLoader : Dictionary<string, string?>, IFileLoader
{
    /// <inheritdoc />
    public string? LoadText(IncludeComponent.FileContext context)
    {
        return this.GetValueOrDefault(context.FilePath);
    }
}
