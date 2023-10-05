using System.Text;
using Mjml.Net.Components;

namespace Mjml.Net.Includes.Filesystem;

/// <summary>
/// Provides files from a local filesystem.
/// </summary>
public class FilesystemFileLoader : CacheableFileLoader
{
    private readonly Encoding encoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilesystemFileLoader"/> class.
    /// </summary>
    /// <param name="baseFolder">A base folder for relative includes. By default it should be the directory of the rendering file.</param>
    /// <param name="encoding">The encoding to use when reading files. Default is UTF8.</param>
    /// <param name="pathResolver">A path resolver. It resolves full path to the target file taking into account all previously loaded includes.</param>
    /// <param name="pathEqualityComparer">An equality comparer for paths.</param>
    /// <param name="enableCaching">Indicates if loader should cache the content of loaded files for future usage.</param>
    public FilesystemFileLoader(
        string baseFolder,
        Encoding? encoding = null,
        IMjIncludePathResolver? pathResolver = null,
        IEqualityComparer<string>? pathEqualityComparer = null,
        bool enableCaching = true
    )
        : base(
            pathResolver ?? new FilesystemPathResolver(baseFolder),
            pathEqualityComparer ?? new FilePathEqualityComparer(baseFolder),
            enableCaching
        )
    {
        this.encoding = encoding ?? Encoding.UTF8;
    }

    protected override string? LoadText(string resolvedPath, IncludedFileInfo context)
    {
        return !File.Exists(resolvedPath) ? null : File.ReadAllText(resolvedPath, encoding);
    }
}
