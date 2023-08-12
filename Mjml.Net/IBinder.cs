namespace Mjml.Net;

/// <summary>
/// Providers values from MJML.
/// </summary>
public interface IBinder
{
    /// <summary>
    /// Get the attribute of the node with the given name.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <returns>
    /// The attribute of the node or null if not found.
    /// </returns>
    string? GetAttribute(string name);

    /// <summary>
    /// Get the text content of the node.
    /// </summary>
    /// <returns>The content of the node or null if not found.</returns>
    InnerTextOrHtml? GetText();
}
