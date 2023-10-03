using Mjml.Net.Components;

namespace Mjml.Net;

/// <summary>
/// Provides files for mj-include components.
/// </summary>
public interface IFileLoader
{
    /// <summary>
    /// Loads the file as text from the specified path. fileInfo.FilePath should be used in case of nested includes since it combines all parent files paths.
    /// </summary>
    /// <param name="fileInfo">The information about the current file path for resolving.</param>
    /// <returns>
    /// The text of the file or null, if not found and an optional fileInfo.
    /// </returns>
    string? LoadText(IncludedFileInfo fileInfo);
}
