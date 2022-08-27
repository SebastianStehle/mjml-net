using Mjml.Net;

namespace Tests.Internal
{
    /// <summary>
    /// Provides the files from an in memory store.
    /// </summary>
    /// <remarks>
    /// Useful for preloading.
    /// </remarks>
    public sealed class InMemoryFileLoader : IFileLoader
    {
        private readonly Dictionary<string, string?> files = new Dictionary<string, string?>();

        /// <summary>
        /// Gets or sets a file content by path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The text of the file or null, if not found.</returns>
        public string? this[string path]
        {
            get => LoadText(path);
            set => Add(path, value);
        }

        /// <summary>
        /// Adds the content.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="content">The file content.</param>
        /// <returns>The current instance.</returns>
        public InMemoryFileLoader Add(string path, string? content)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            files[path] = content;

            return this;
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

            return files.GetValueOrDefault(path);
        }
    }
}
