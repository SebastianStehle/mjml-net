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
        /// <summary>
        /// Adds the content.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="content">The file content.</param>
        /// <returns>The current instance.</returns>
        public InMemoryFileLoader AddContent(string path, string? content)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this[path] = content;

            return this;
        }

        /// <inheritdoc />
        public bool ContainsFile(string path)
        {
            return ContainsKey(path);
        }

        /// <inheritdoc />
        public TextReader? LoadReader(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var text = LoadText(path);

            if (text != null)
            {
                return new StringReader(text);
            }

            return null;
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
