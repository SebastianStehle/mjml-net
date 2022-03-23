namespace Mjml.Net
{
    /// <summary>
    /// Generates IDs for HTML tags.
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// Generates a new ID and returns the result.
        /// </summary>
        /// <returns>The created ID.</returns>
        string Next();
    }
}
