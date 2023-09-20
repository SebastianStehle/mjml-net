using Mjml.Net;

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
    public (string? Content, object? Context) LoadText(string path, object? parentContext)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        path = BuildPath(path, parentContext);

        return (this.GetValueOrDefault(path), path);
    }

    private static string BuildPath(string path, object? parentContext)
    {
        if (path.StartsWith('/') || path.StartsWith('\\'))
        {
            return path;
        }

        if (parentContext is not string parentPath)
        {
            return path;
        }

        var folderIndex = parentPath.LastIndexOf('/');

        if (folderIndex >= 0)
        {
            var folderPart = parentPath.Substring(0, folderIndex);

            return $"{folderPart}/{path}";
        }

        folderIndex = parentPath.LastIndexOf('\\');

        if (folderIndex >= 0)
        {
            var folderPart = parentPath.Substring(0, folderIndex);

            return $"{folderPart}\\{path}";
        }

        return path;
    }
}
