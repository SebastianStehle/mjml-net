using Mjml.Net.Components;

namespace Mjml.Net.Includes.InMemory;

/// <summary>
/// Provides the files from an in-memory store.
/// </summary>
/// <remarks>
/// Useful for preloading.
/// </remarks>
public sealed class InMemoryFileLoader : CacheableFileLoader
{
    public InMemoryFileLoader(
        IMjIncludePathResolver? pathResolver = null,
        IEqualityComparer<string>? equalityComparer = null
    )
        : base(pathResolver, equalityComparer)
    {
    }

    protected override string? LoadText(string resolvedPath, IncludedFileInfo context)
    {
        return this.GetValueOrDefault(resolvedPath);
    }
}
