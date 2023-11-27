using System.Text.RegularExpressions;
using Squidex.Text;

namespace Tools;

internal static partial class MigrateCS
{
    public static void Run()
    {
        var directory = new DirectoryInfo("../../../../Mjml.Net");

        var defaultRegex = DefaultRegexBuilder();
        var argumentRegex = ArgumentRegexBuilder();
        var argumentAccessRegex = ArgumentAcessRegexBuilder();

        foreach (var file in directory.GetFiles("*.cs", SearchOption.AllDirectories))
        {
            if (!file.Name.EndsWith("Component.cs", StringComparison.Ordinal))
            {
                continue;
            }

            var fileText = File.ReadAllText(file.FullName);

            var defaultAttributes = new Dictionary<string, string>();

            foreach (var match in defaultRegex.Matches(fileText).OfType<Match>())
            {
                var name = match.Groups["Name"].Value;
                var value = match.Groups["Value"].Value;

                defaultAttributes[name] = value;
            }

            var changed = argumentRegex.Replace(fileText, x =>
            {
                var name = x.Groups["Name"].Value;
                var type = x.Groups["Type"].Value;

                var attribute = $"[Bind(\"{name}\")]";
                var assignment = string.Empty;

                if (type != "String")
                {
                    attribute = $"[Bind(\"{name}\", BindType.{type})]";
                }

                if (defaultAttributes.TryGetValue(name, out var value) && value.Length > 0)
                {
                    return $"{attribute}\npublic string {name.ToPascalCase()} = \"{value}\";\n";
                }
                else
                {
                    return $"{attribute}\npublic string? {name.ToPascalCase()};\n";
                }
            });

            changed = argumentAccessRegex.Replace(changed, x =>
            {
                var name = x.Groups["Name"].Value;

                return $"props.{name.ToPascalCase()}";
            });

            if (changed == fileText)
            {
                continue;
            }

            File.WriteAllText(file.FullName, changed);

            Console.WriteLine("{0} changed", file.FullName);
        }
    }

    [GeneratedRegex("\\[\"(?<Name>.*)\"\\] = AttributeTypes\\.(?<Type>[^,]*),?", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex ArgumentRegexBuilder();

    [GeneratedRegex("node\\.GetAttribute\\(\"(?<Name>[^\"]*)\"\\)", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex ArgumentAcessRegexBuilder();

    [GeneratedRegex("\\[\"(?<Name>.*)\"\\] = \"(?<Value>.*)\"", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex DefaultRegexBuilder();
}
