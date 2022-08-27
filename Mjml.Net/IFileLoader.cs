namespace Mjml.Net
{
    /// <summary>
    /// Provides files for mj-include components.
    /// </summary>
    public interface IFileLoader
    {
        /// <summary>
        /// Loads the file as text from the specified path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>
        /// The text of the file or null, if not found.
        /// </returns>
        string? LoadText(string path);

        /// <summary>
        /// Loads the file as stream from the specified path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>
        /// The stream to the file or null, if not found.
        /// </returns>
        TextReader? LoadReader(string path);
    }
}
