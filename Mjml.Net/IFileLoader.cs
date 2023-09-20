namespace Mjml.Net;

/// <summary>
/// Provides files for mj-include components.
/// </summary>
public interface IFileLoader
{
    /// <summary>
    /// Loads the file as text from the specified path and usigng context object from the parent file.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="parentContext">The parent context.</param>
    /// <returns>
    /// The text of the file or null, if not found and an optional context.
    /// </returns>
    (string? Content, object? Context) LoadText(string path, object? parentContext);
}
