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

    public FilesystemFileLoader(string baseFolder, Encoding? encoding, bool enableCaching = true)
    {
        this.baseFolder = baseFolder;
        EnableCaching = enableCaching;
        this.encoding = encoding ?? Encoding.UTF8;
    }

    public bool EnableCaching { get; set; }
    public ConcurrentDictionary<string, string> CachedFiles { get; } = new (new FilePathEqualityComparer());

    public virtual string LoadText(IncludeComponent.FileContext context)
    {
        var fileFullPath = Path.Combine(baseFolder, context.FilePath);

        if (EnableCaching)
        {
            return CachedFiles.GetOrAdd(fileFullPath, File.ReadAllText, encoding);
        }

        return File.ReadAllText(fileFullPath, encoding);
    }
}
