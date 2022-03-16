using Squidex.Text;
using System.Text.RegularExpressions;

namespace Tools
{
    internal static class MigrateCS
    {
        public static void Run()
        {
            var directory = new DirectoryInfo("../../../../Mjml.Net");

            var argumentRegex = new Regex("\\[\"(?<Name>.*)\"\\] = AttributeTypes\\.(?<Type>[^,]*),?");
            var argumentAccessRegex = new Regex("node\\.GetAttribute\\(\"(?<Name>[^\"]*)\"\\)");

            var defaultRegex = new Regex("\\[\"(?<Name>.*)\"\\] = \"(?<Value>.*)\"");

            foreach (var file in directory.GetFiles("*.cs", SearchOption.AllDirectories))
            {
                if (!file.Name.EndsWith("Component.cs"))
                {
                    continue;
                }

                var text = File.ReadAllText(file.FullName);

                var defaultAttributes = new Dictionary<string, string>();

                foreach (Match match in defaultRegex.Matches(text))
                {
                    var name = match.Groups["Name"].Value;
                    var value = match.Groups["Value"].Value;

                    defaultAttributes[name] = value;
                }


                var changed = argumentRegex.Replace(text, x =>
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
                        assignment = $" = \"{value}\"";
                    }

                    return $"{attribute}\npublic string? {name.ToPascalCase()}{assignment}; \n";
                });

                changed = argumentAccessRegex.Replace(changed, x =>
                {
                    var name = x.Groups["Name"].Value;

                    return $"props.{name.ToPascalCase()}";
                });

                if (changed == text)
                {
                    continue;
                }

                File.WriteAllText(file.FullName, changed);

                Console.WriteLine("{0} changed", file.FullName);
            }
        }
    }
}
