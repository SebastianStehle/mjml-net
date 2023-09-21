using Mjml.Net.Components;

namespace Mjml.Net;

/// <summary>
/// Provides files for mj-include components.
/// </summary>
public interface IFileLoader
{
    /// <summary>
    /// Loads the file as text from the specified path. context.FilePath should be used in case of nested includes since it combines all parent files paths.
    /// </summary>
    /// <param name="context">The context for resolving current file.</param>
    /// <returns>
    /// The text of the file or null, if not found and an optional context.
    /// </returns>
    string? LoadText(IncludeComponent.FileContext context);
}
