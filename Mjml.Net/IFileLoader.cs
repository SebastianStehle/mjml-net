namespace Mjml.Net
{
    /// <summary>
    /// Provides files for mj-include components.
    /// </summary>
    public interface IFileLoader
    {
        /// <summary>
        /// Determines if the file loader contains the path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True, if the file exists, false otherwise.</returns>
        bool ContainsFile(string path);

        /// <summary>
        /// Loads the file as text from the specified path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>
        /// The text of the file or null, if not found.
        /// </returns>
        string? LoadText(string path);
    }
}
