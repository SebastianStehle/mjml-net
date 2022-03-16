using Squidex.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace Tools
{
    internal class ConvertJS
    {
        public static void Run()
        {
            var propertyRegex = new Regex("'?(?<Name>[^'\\s]*)'?: '(?<Value>[^']*)'", RegexOptions.Singleline | RegexOptions.Compiled);

            var directory = new DirectoryInfo("../../../../Mjml.Net");

            foreach (var file in directory.GetFiles("*.cs", SearchOption.AllDirectories))
            {
                if (!file.Name.EndsWith("Component.cs"))
                {
                    continue;
                }

                var text = File.ReadAllText(file.FullName);

                var defaultAttributes = new Dictionary<string, string>();

                var startOfDefaultAttribute = text.IndexOf("static defaultAttributes = {");

                if (startOfDefaultAttribute >= 0)
                {
                    var end = text.IndexOf("}", startOfDefaultAttribute);

                    var range = text.Substring(startOfDefaultAttribute + 1, end - startOfDefaultAttribute);

                    foreach (Match match in propertyRegex.Matches(range))
                    {
                        var name = match.Groups["Name"].Value;
                        var value = match.Groups["Value"].Value;

                        defaultAttributes[name] = value;
                    }
                }

                var sb = new StringBuilder();

                var startOfAllowedAttributes = text.IndexOf("static allowedAttributes = {");

                if (startOfAllowedAttributes >= 0)
                {
                    var end = text.IndexOf("}", startOfAllowedAttributes);

                    var range = text.Substring(startOfAllowedAttributes + 1, end - startOfAllowedAttributes);

                    var matches = propertyRegex.Matches(range).OfType<Match>().Select(match =>
                    {
                        var name = match.Groups["Name"].Value;
                        var type = match.Groups["Value"].Value;

                        return (name, type);
                    }).OrderBy(x => x.name);

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

                        sb.AppendLine($"[Bind(\"{name}\", BindType.{actualType})]");

                        if (defaultAttributes.TryGetValue(name, out var defaultValue) && defaultValue.Length > 0)
                        {
                            sb.AppendLine($"public string {name.ToPascalCase()} = \"{defaultValue}\";");
                        }
                        else
                        {
                            sb.AppendLine($"public string? {name.ToPascalCase()};");
                        }

                        sb.AppendLine();
                    }
                }

                if (sb.Length > 0)
                {
                    File.WriteAllText(file.FullName, sb.ToString());

                    Console.WriteLine("{0} changed", file.FullName);

                }
            }
        }
    }
}
