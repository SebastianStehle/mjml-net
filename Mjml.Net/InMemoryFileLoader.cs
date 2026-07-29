using Mjml.Net;

namespace Tests.Internal;

/// <summary>
/// Provides the files from an in memory store.
/// </summary>
/// <remarks>
/// Useful for preloading.
/// </remarks>
public sealed class InMemoryFileLoader(IReadOnlyDictionary<string, string> content) : IFileLoader
{
    private readonly Stack<string> pathStack = new Stack<string>();

    /// <inheritdoc />
    public string? LoadText(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        pathStack.TryPeek(out var parentPath);

        path = BuildPath(path, parentPath);

        pathStack.Push(path);

        return content.GetValueOrDefault(path);
    }

    private static string BuildPath(string path, string? parentPath)
    {
        if (path.StartsWith('/') || path.StartsWith('\\'))
        {
            return path;
        }

        if (parentPath == null)
        {
            return path;
        }

        var folderIndex = parentPath.LastIndexOf('/');

        if (folderIndex >= 0)
        {
            return string.Concat(parentPath.AsSpan(0, folderIndex), "/", path);
        }

        folderIndex = parentPath.LastIndexOf('\\');

        if (folderIndex >= 0)
        {
            return string.Concat(parentPath.AsSpan(0, folderIndex), "\\", path);
        }

        return path;
    }
}
