using System.Runtime.InteropServices;

namespace Mjml.Net.Includes.Filesystem;

/// <summary>
/// An equality comparer for paths in a filesystem. Works with both relative and absolute paths.
/// </summary>
public class FilePathEqualityComparer : IEqualityComparer<string>
{
    private readonly string? workingDirectory;
    private readonly StringComparison equalityComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePathEqualityComparer"/> class.
    /// </summary>
    /// <param name="workingDirectory">A base directory to compare relative and absolute paths. Must be absolute to handle paths correctly.</param>
    /// <param name="isPathCaseSensitive">Determine if paths should be treated as case-sensitive.
    ///     Null value (default) means that this value will be computed based on current OS platform.</param>
    public FilePathEqualityComparer(string? workingDirectory = null, bool? isPathCaseSensitive = null)
    {
        this.workingDirectory = workingDirectory;

        // For different OS we have different path case-sensitivity behavior.
        isPathCaseSensitive ??= RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);

        equalityComparer = isPathCaseSensitive == true
            ? StringComparison.InvariantCulture
            : StringComparison.InvariantCultureIgnoreCase;
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

        if (Path.IsPathRooted(x) != Path.IsPathRooted(y))
        {
            if (workingDirectory == null || !Path.IsPathRooted(workingDirectory))
            {
                // In this case we can't be sure that these paths are the same. It depends on app working directory.
                return false;
            }

            x = Path.Combine(workingDirectory, x);
            y = Path.Combine(workingDirectory, y);
        }

        return string.Equals(Path.GetFullPath(x), Path.GetFullPath(y), equalityComparer);
    }

    public int GetHashCode(string obj)
    {
        return Path.GetFullPath(obj).GetHashCode(equalityComparer);
    }
}
