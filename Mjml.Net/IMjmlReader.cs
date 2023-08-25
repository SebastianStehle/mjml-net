namespace Mjml.Net;

/// <summary>
/// Reads MJML fragments.
/// </summary>
public interface IMjmlReader
{
    /// <summary>
    /// Read a xml fragment from a string.
    /// </summary>
    /// <param name="mjml">The mjml fragment reader.</param>
    /// <param name="file">The fle for debugging purposes.</param>
    /// <param name="parent">The parent component.</param>
    void ReadFragment(string mjml, string? file, IComponent parent);
}
