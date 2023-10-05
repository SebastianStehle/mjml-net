using Mjml.Net.Components;
using Mjml.Net.Includes.InMemory;

namespace Mjml.Net.Includes;

/// <summary>
/// Provides files from a local filesystem.
/// </summary>
public abstract class CacheableFileLoader : Dictionary<string, string?>, IFileLoader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CacheableFileLoader"/> class.
    /// </summary>
    /// <param name="pathResolver">A path resolver. It resolves full path to the target file taking into account all previously loaded includes.</param>
    /// <param name="pathEqualityComparer">An equality comparer for paths.</param>
    /// <param name="enableCaching">Indicates if loader should cache the content of loaded files for future usage.</param>
    protected CacheableFileLoader(
        IMjIncludePathResolver? pathResolver = null,
        IEqualityComparer<string>? pathEqualityComparer = null,
        bool enableCaching = true
    )
        : base(pathEqualityComparer)
    {
        EnableCaching = enableCaching;
        PathResolver = pathResolver ?? new NoopPathResolver();
    }

    /// <summary>
    /// Indicates if loader should cache the content of loaded files for future usage.
    /// </summary>
    public bool EnableCaching { get; set; }

    protected IMjIncludePathResolver PathResolver { get; set; }

    public virtual string? LoadText(IncludedFileInfo context)
    {
        var fileFullPath = PathResolver.ResolveFilePath(context);

        if (EnableCaching && TryGetValue(fileFullPath, out var cachedValue))
        {
            return cachedValue;
        }

        var content = LoadText(fileFullPath, context);

        if (EnableCaching)
        {
            this[fileFullPath] = content;
        }

        return content;
    }

    protected abstract string? LoadText(string resolvedPath, IncludedFileInfo context);
}
