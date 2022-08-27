namespace Mjml.Net
{
    /// <summary>
    /// Reads MJML fragments.
    /// </summary>
    public interface IMjmlReader
    {
        /// <summary>
        /// Read a xml fragment from a stream reader.
        /// </summary>
        /// <param name="mjml">The mjml fragment.</param>
        void ReadFragment(TextReader mjml);

        /// <summary>
        /// Read a xml fragment from a string.
        /// </summary>
        /// <param name="xml">The xml fragment reader.</param>
        void ReadFragment(string xml);
    }
}
