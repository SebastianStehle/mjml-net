using Squidex.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace Tools
{
    internal static class ConvertJS
    {
        public static void Run()
        {
            var propertyRegex = new Regex("'?(?<Name>[^'\\s]*)'?: '(?<Value>[^']*)'", RegexOptions.Singleline | RegexOptions.Compiled);

            var directory = new DirectoryInfo("../../../../Mjml.Net");

            foreach (var file in directory.GetFiles("*.cs", SearchOption.AllDirectories))
            {
                var suffix = "Component.cs";

                if (!file.Name.EndsWith(suffix))
                {
                    continue;
                }

                var fileProps = file.Name.Substring(0, file.Name.Length - suffix.Length);
                var fileText = File.ReadAllText(file.FullName);

                var defaultAttributes = new Dictionary<string, string>();

                var startOfDefaultAttribute = fileText.IndexOf("static defaultAttributes = {");

                if (startOfDefaultAttribute >= 0)
                {
                    var end = fileText.IndexOf("}", startOfDefaultAttribute);

                    var range = fileText.Substring(startOfDefaultAttribute + 1, end - startOfDefaultAttribute);

                    foreach (Match match in propertyRegex.Matches(range))
                    {
                        var name = match.Groups["Name"].Value;
                        var value = match.Groups["Value"].Value;

                        defaultAttributes[name] = value;
                    }
                }

                var sb = new StringBuilder();

                var startOfAllowedAttributes = fileText.IndexOf("static allowedAttributes = {");

                if (startOfAllowedAttributes >= 0)
                {
                    var end = fileText.IndexOf("}", startOfAllowedAttributes);

                    var range = fileText.Substring(startOfAllowedAttributes + 1, end - startOfAllowedAttributes);

                    var matches = propertyRegex.Matches(range).OfType<Match>().Select(match =>
                    {
                        var name = match.Groups["Name"].Value;
                        var type = match.Groups["Value"].Value;

                        return (name, type);
                    }).OrderBy(x => x.name).ToList();

                    if (matches.Count > 0)
                    {
                        sb.AppendTabbed(0, "namespace Mjml.Net.Components.Body");
                        sb.AppendTabbed(0, "{");
                        sb.AppendTabbed(1, $"public partial struct {fileProps}Props");
                        sb.AppendTabbed(1, "{");
                    }

                    var i = 1;
                    foreach (var (name, type) in matches)
                    {
                        var actualType = "String";

                        if (type == "unit(px,%)")
                        {
                            actualType = "PixelsOrPercent";
                        }
                        else if (type == "unit(px)")
                        {
                            actualType = "Pixels";
                        }
                        else if (type == "color")
                        {
                            actualType = "Color";
                        }
                        else if (type == "unit(px,%){1,4}")
                        {
                            actualType = "FourPixelsOrPercent";
                        }
                        else if (type == "enum(top,bottom,middle)")
                        {
                            actualType = "VerticalAlign";
                        }
                        else if (type == "enum(left,center,right)")
                        {
                            actualType = "Align";
                        }
                        else if (type == "string")
                        {
                            actualType = "String";
                        }
                        else
                        {
                            actualType = type;
                        }

                        sb.AppendTabbed(2, $"[Bind(\"{name}\", BindType.{actualType})]");

                        if (defaultAttributes.TryGetValue(name, out var defaultValue) && defaultValue.Length > 0)
                        {
                            sb.AppendTabbed(2, $"public string {name.ToPascalCase()} = \"{defaultValue}\";");
                        }
                        else
                        {
                            sb.AppendTabbed(2, $"public string? {name.ToPascalCase()};");
                        }

                        if (i != matches.Count)
                        {
                            sb.AppendLine();
                        }

                        i++;
                    }

                    sb.AppendTabbed(1, "}");
                    sb.AppendTabbed(0, "}");
                }

                if (sb.Length > 0)
                {
                    File.WriteAllText(file.FullName, sb.ToString());

                    Console.WriteLine("{0} changed", file.FullName);
                }
            }
        }

        private static void AppendTabbed(this StringBuilder sb, int tabs, string line)
        {
            sb.Append(new string(' ', tabs * 4));
            sb.AppendLine(line);
        }
    }
}
