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
    /// <returns>
    /// The text of the file or null, if not found and an optional context.
    /// </returns>
    string? LoadText(string path);
}
