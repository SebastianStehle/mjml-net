using System.Runtime.InteropServices;

namespace Mjml.Net;

/// <summary>
/// Provides an equality comparer for paths. Works with relative paths as well.
/// </summary>
public class FilePathEqualityComparer : IEqualityComparer<string>
{
    private bool isPathCaseSensitive;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePathEqualityComparer"/> class.
    /// </summary>
    /// <param name="isPathCaseSensitive">Determine if paths should be treated as case-sensitive.
    /// Null value (default) means that this value will be computed based on current OS platform.</param>
    public FilePathEqualityComparer(bool? isPathCaseSensitive = null)
    {
        // For different OS we have different path case-sensitivity behavior.
        this.isPathCaseSensitive = isPathCaseSensitive ??
                                   (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                                    RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD));
    }

    public bool Equals(string? x, string? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        var comparer = isPathCaseSensitive
            ? StringComparison.InvariantCulture
            : StringComparison.InvariantCultureIgnoreCase;

        return string.Equals(Path.GetFullPath(x), Path.GetFullPath(y), comparer);
    }

    public int GetHashCode(string obj)
    {
        return Path.GetFullPath(obj).GetHashCode(StringComparison.InvariantCultureIgnoreCase);
    }
}
