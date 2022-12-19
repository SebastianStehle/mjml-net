using Mjml.Net;

namespace Tests.Internal
{
    /// <summary>
    /// Provides the files from an in memory store.
    /// </summary>
    /// <remarks>
    /// Useful for preloading.
    /// </remarks>
    public sealed class InMemoryFileLoader : Dictionary<string, string?>, IFileLoader
    {
        /// <inheritdoc />
        public bool ContainsFile(string path)
        {
            return ContainsKey(path);
        }

        /// <inheritdoc />
        public string? LoadText(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return this.GetValueOrDefault(path);
        }
    }
}
