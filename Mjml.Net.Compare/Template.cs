namespace Mjml.Net.Compare;

public sealed record Template(string Name, string Source)
{
    public static List<Template> LoadAll(string directory)
    {
        return Directory.GetFiles(directory, "*.mjml")
            .Order(StringComparer.Ordinal)
            .Select(x => new Template(Path.GetFileName(x), File.ReadAllText(x)))
            .ToList();
    }
}
