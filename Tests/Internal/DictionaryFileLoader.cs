using Mjml.Net;

namespace Tests.Internal
{
    internal class DictionaryFileLoader : IFileLoader
    {
        private readonly Dictionary<string, string?> files = new Dictionary<string, string?>();

        public string? this[string key]
        {
            get => files.GetValueOrDefault(key);
            set => files[key] = value;
        }

        public void Add(string file, string content)
        {
            files[file] = content;
        }

        public string? LoadText(string path)
        {
            return files.GetValueOrDefault(path);
        }
    }
}
