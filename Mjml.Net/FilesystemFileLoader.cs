using System.Collections.Concurrent;
using System.Text;
using Mjml.Net.Components;

namespace Mjml.Net;

/// <summary>
/// Provides files from a local filesystem.
/// </summary>
public class FilesystemFileLoader : IFileLoader
{
    private readonly string baseFolder;
    private readonly Encoding encoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilesystemFileLoader"/> class.
    /// </summary>
    /// <param name="baseFolder">A base folder for relative includes. By default it should be the directory of the rendering file.</param>
    /// <param name="encoding">The encoding to use when reading files.</param>
    /// <param name="enableCaching">Indicates if loader should cache the content of loaded files for future usage.</param>
    public FilesystemFileLoader(string baseFolder, Encoding? encoding, bool enableCaching = true)
    {
        this.baseFolder = baseFolder;
        EnableCaching = enableCaching;
        this.encoding = encoding ?? Encoding.UTF8;
    }

    /// <summary>
    /// Indicates if loader should cache the content of loaded files for future usage.
    /// </summary>
    public bool EnableCaching { get; set; }

    /// <summary>
    /// A concurrent dictionary of already loaded files. You can modify this dictionary externally to preload files.
    /// </summary>
    public ConcurrentDictionary<string, string> CachedFiles { get; } = new (new FilePathEqualityComparer());

    public virtual string LoadText(IncludedFileInfo context)
    {
        var fileFullPath = Path.Combine(baseFolder, context.FilePath);

        if (EnableCaching)
        {
            return CachedFiles.GetOrAdd(fileFullPath, File.ReadAllText, encoding);
        }

        return File.ReadAllText(fileFullPath, encoding);
    }
}
