using Mjml.Net.Components;

namespace Mjml.Net.Includes.Filesystem;

/// <summary>
///     Resolves full path for <see cref="IncludedFileInfo" />
/// </summary>
public class FilesystemPathResolver : IMjIncludePathResolver
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FilesystemPathResolver" /> class.
    /// </summary>
    /// <param name="basePath">The base path for resolving relative include paths.</param>
    public FilesystemPathResolver(string? basePath = null)
    {
        BasePath = basePath;
    }

    /// <summary>
    /// The base path for resolving relative include paths.
    /// </summary>
    public string? BasePath { get; }

    public string ResolveFilePath(IncludedFileInfo fileInfo)
    {
        var mjIncludeValues = fileInfo.Flatten()
            .Select(x => Path.GetDirectoryName(x.MjIncludeValue))
            .Prepend(BasePath)
            .Append(Path.GetFileName(fileInfo.MjIncludeValue))
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        var combinedPath = Path.Combine(mjIncludeValues!);

        return combinedPath;
    }
}
