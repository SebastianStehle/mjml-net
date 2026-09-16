namespace Mjml.Net.Compare;

public static class ReadmeUpdater
{
    private const string MarkerStart = "<!-- COMPARISON:START -->";
    private const string MarkerEnd = "<!-- COMPARISON:END -->";

    // On the first run, the section is inserted before this heading (or appended if it does not exist).
    private const string InsertBefore = "### BenchmarkDotNet";

    public static async Task UpdateAsync(string path, string summary)
    {
        var readme = await File.ReadAllTextAsync(path);
        var section = $"{MarkerStart}\n{summary.Trim()}\n{MarkerEnd}";

        var indexStart = readme.IndexOf(MarkerStart, StringComparison.Ordinal);
        var indexEnd = readme.IndexOf(MarkerEnd, StringComparison.Ordinal);
        var indexInsert = readme.IndexOf(InsertBefore, StringComparison.Ordinal);

        if (indexStart >= 0 && indexEnd > indexStart)
        {
            readme = readme[..indexStart] + section + readme[(indexEnd + MarkerEnd.Length)..];
        }
        else if (indexInsert >= 0)
        {
            readme = readme[..indexInsert] + section + "\n\n" + readme[indexInsert..];
        }
        else
        {
            readme = readme.TrimEnd() + "\n\n" + section + "\n";
        }

        await File.WriteAllTextAsync(path, readme);
    }
}
