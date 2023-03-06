using System.Text;
using System.Text.RegularExpressions;
using Squidex.Text;

namespace Tools
{
    internal static class ConvertJS
    {
        public static void Run()
        {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
            var propertyRegex = new Regex("'?(?<Name>[a-z\\-]*)'?: '(?<Value>[^']*)'");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
            var directory = new DirectoryInfo("../../../../Mjml.Net");

            foreach (var file in directory.GetFiles("*.cs", SearchOption.AllDirectories))
            {
                var suffix = "Component.cs";

                if (!file.Name.EndsWith(suffix, StringComparison.Ordinal))
                {
                    continue;
                }

                var fileName = file.Name[..^3];
                var fileText = File.ReadAllText(file.FullName);

                // Remove this, otherwise we get issues with our ending detection.
                fileText = fileText.Replace("{1,4}", string.Empty, StringComparison.Ordinal);

                var defaultAttributes = new Dictionary<string, string>();

                var startOfDefaultAttribute = fileText.IndexOf("static defaultAttributes = {", StringComparison.Ordinal);

                if (startOfDefaultAttribute >= 0)
                {
                    var end = fileText.IndexOf("}", startOfDefaultAttribute, StringComparison.Ordinal);

                    var range = fileText.Substring(startOfDefaultAttribute + 1, end - startOfDefaultAttribute);

                    foreach (Match match in propertyRegex.Matches(range))
                    {
                        var name = match.Groups["Name"].Value;
                        var value = match.Groups["Value"].Value;

                        defaultAttributes[name] = value;
                    }
                }

                var sb = new StringBuilder();

                var startOfAllowedAttributes = fileText.IndexOf("static allowedAttributes = {", StringComparison.Ordinal);

                if (startOfAllowedAttributes >= 0)
                {
                    var end = fileText.IndexOf("}", startOfAllowedAttributes, StringComparison.Ordinal);

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
                        sb.AppendTabbed(1, $"public partial class {fileName}");
                        sb.AppendTabbed(1, "{");
                    }

                    var i = 1;
                    foreach (var (name, type) in matches)
                    {
                        var actualType = "String";

                        switch (type)
                        {
                            case "unit(px,%)":
                                actualType = "PixelsOrPercent";
                                break;
                            case "unit(px)":
                                actualType = "Pixels";
                                break;
                            case "color":
                                actualType = "Color";
                                break;
                            case "unit(px,%){1,4}":
                                actualType = "FourPixelsOrPercent";
                                break;
                            case "enum(top,bottom,middle)":
                                actualType = "VerticalAlign";
                                break;
                            case "enum(left,center,right)":
                                actualType = "Align";
                                break;
                            case "string":
                                actualType = "String";
                                break;
                            default:
                                actualType = type;
                                break;
                        }

                        sb.AppendTabbed(2, @$"[Bind(""{name}"", BindType.{actualType})]");

                        if (defaultAttributes.TryGetValue(name, out var defaultValue) && defaultValue.Length > 0)
                        {
                            sb.AppendTabbed(2, @$"public string {name.ToPascalCase()} = ""{defaultValue}"";");
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
