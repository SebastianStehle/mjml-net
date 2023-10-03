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
    public InMemoryFileLoader()
        : base(new FilePathEqualityComparer())
    {
    }

    /// <inheritdoc />
    public string? LoadText(IncludedFileInfo context)
    {
        return this.GetValueOrDefault(context.FilePath);
    }
}
