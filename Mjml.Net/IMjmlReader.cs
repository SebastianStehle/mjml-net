namespace Mjml.Net;

/// <summary>
/// Reads MJML fragments.
/// </summary>
public interface IMjmlReader
{
    /// <summary>
    /// Read a xml fragment from a string.
    /// </summary>
    /// <param name="xml">The xml fragment reader.</param>
    void ReadFragment(string xml);
}
