namespace Mjml.Net.Compare;

public sealed record ComparePaths(string Root, string Node, string Templates, string Benchmark, string Readme)
{
    public static ComparePaths Resolve()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "Mjml.Net.slnx")))
        {
            directory = directory.Parent;
        }

        var root = directory?.FullName ?? throw new InvalidOperationException("Cannot find repository root (Mjml.Net.slnx).");

        return new ComparePaths(
            root,
            Path.Combine(root, "Mjml.Net.Compare", "node"),
            Path.Combine(root, "Mjml.Net.Benchmark", "Templates"),
            Path.Combine(root, "benchmark.md"),
            Path.Combine(root, "README.md"));
    }
}
