using Mjml.Net.Helpers;
using Mjml.Net.Types;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mjml.Net.Components;

public sealed partial class IncludeComponent : Component
{
    private static readonly (Type, string) ContextKey = (typeof(object), "FileContext");

    public override string ComponentName => "mj-include";

    public override bool Raw => true;

    [Bind("path", BindType.RequiredString)]
    public string Path;

    [Bind("type", typeof(TypeValidator))]
    public string Type;

    public IncludeType ActualType { get; private set; }

    public override void Read(IHtmlReader htmlReader, IMjmlReader mjmlReader, GlobalContext context)
    {
        var actualPath = Binder.GetAttribute("path");
        var actualType = Binder.GetAttribute("type");

        switch (actualType)
        {
            case "html":
                ActualType = IncludeType.Html;
                break;
            case "css":
                ActualType = IncludeType.Css;
                break;
            case "mjml":
                ActualType = IncludeType.Mjml;
                break;
            default:
                if (actualPath?.EndsWith(".html", StringComparison.OrdinalIgnoreCase) == true)
                {
                    ActualType = IncludeType.Html;
                }
                else if (actualPath?.EndsWith(".css", StringComparison.OrdinalIgnoreCase) == true)
                {
                    ActualType = IncludeType.Css;
                }
                else
                {
                    ActualType = IncludeType.Mjml;
                }

                break;
        }

        if (ActualType != IncludeType.Mjml || string.IsNullOrWhiteSpace(actualPath))
        {
            return;
        }

        if (context.Options.FileLoader == null)
        {
            return;
        }

        context.GlobalData.TryGetValue(ContextKey, out var parentContext);
        var currentContext = new FileContext(actualPath, parentContext as FileContext);

        var content = context.Options.FileLoader.LoadText(currentContext);

        if (!string.IsNullOrWhiteSpace(content))
        {
            context.GlobalData[ContextKey] = currentContext;

            mjmlReader.ReadFragment(content, actualPath, Parent!);

            // Restore the previous context.
            if (parentContext != null)
            {
                context.GlobalData[ContextKey] = parentContext;
            }
            else
            {
                context.GlobalData.Remove(ContextKey);
            }
        }
    }

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        if (ActualType == IncludeType.Mjml || string.IsNullOrWhiteSpace(Path))
        {
            // Render the children that have been added in the bind method.
            RenderChildren(renderer, context);
            return;
        }

        if (context.Options.FileLoader == null)
        {
            return;
        }

        context.GlobalData.TryGetValue(ContextKey, out var parentContext);

        var content = context.Options.FileLoader.LoadText(new FileContext(Path, parentContext as FileContext));

        if (content == null)
        {
            return;
        }

        if (ActualType == IncludeType.Html)
        {
            // Allow pretty formatting and indentation.
            renderer.Content(content);
        }
        else if (ActualType == IncludeType.Css)
        {
            // Allow multiple styles and render them later.
            context.AddGlobalData(Style.Static(new InnerTextOrHtml(content)));
        }
    }

    internal sealed class TypeValidator : EnumType
    {
        public TypeValidator()
            : base(false, "mjml", "html", "css")
        {
        }
    }

    /// <summary>
    /// Represents information about included file.
    /// </summary>
    /// <param name="MjIncludeValue">The value that is indicated in path attribute of mj-include tag.</param>
    /// <param name="ParentFileContext">The context of file that included current file.</param>
    public sealed record FileContext(string MjIncludeValue, FileContext? ParentFileContext = null) : GlobalData
    {
        private static readonly char[] DirectorySeparators = { '/', '\\' };

        private string? filePath;
        private string? directory;

        /// <summary>
        /// Returns a file name to read. This field is calculated based on all previously included files.
        /// </summary>
        public string FilePath => filePath ??= GetContextFilePath();

        /// <summary>
        /// Returns a directory for a file. This field is calculated based on all previously included files.
        /// </summary>
        public string Directory => directory ??= GetContextDirectory();

        /// <summary>
        /// This field can be used to store any useful information related to included file.
        /// </summary>
        public dynamic Extras { get; set; }

        private string GetContextDirectory()
        {
            var normalizedMjIncludeValue = RemoveTrailingDotSlashes(MjIncludeValue);

            // Determine current file directory
            var lastSlashIndex = normalizedMjIncludeValue.LastIndexOfAny(DirectorySeparators);
            var fileDirectory = lastSlashIndex == -1 ? string.Empty : normalizedMjIncludeValue[..lastSlashIndex];

            // If it is an absolute path, then parent directory is not needed. Proceed as is.
            if (System.IO.Path.IsPathRooted(normalizedMjIncludeValue))
            {
                return fileDirectory;
            }

            // If it is a relative path, then combine parent context directory and current file directory
            if (string.IsNullOrEmpty(ParentFileContext?.Directory))
            {
                return fileDirectory;
            }

            // If current file does not have parent directory - return parent file directory
            if (string.IsNullOrEmpty(fileDirectory))
            {
                return ParentFileContext.Directory;
            }

            return CombineDirectories(ParentFileContext.Directory, fileDirectory);
        }

        private string GetContextFilePath()
        {
            if (ParentFileContext is null)
            {
                return MjIncludeValue;
            }

            return CombineDirectories(Directory, System.IO.Path.GetFileName(MjIncludeValue));
        }

        private string CombineDirectories(string baseDirectory, string subdirectoryOrFileName)
        {
            var isBaseDirectoryAbsolute = System.IO.Path.IsPathRooted(baseDirectory);

            baseDirectory = baseDirectory.Trim(DirectorySeparators);
            subdirectoryOrFileName = subdirectoryOrFileName.Trim(DirectorySeparators);

            var segments1 = baseDirectory.Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);
            var segments2 = subdirectoryOrFileName.Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);

            var resultSegments = new Stack<string>(segments1.Length + segments2.Length);
            var meaningfulSegmentsCount = 0;

            foreach (var segment in segments1.Union(segments2).Where(x => x != "."))
            {
                if (segment == "..")
                {
                    if (meaningfulSegmentsCount > 0)
                    {
                        resultSegments.Pop();
                        meaningfulSegmentsCount--;
                    }
                    else
                    {
                        if (isBaseDirectoryAbsolute)
                        {
                            throw new InvalidOperationException("The path can't go beyond the filesystem root");
                        }

                        resultSegments.Push(segment);
                    }
                }
                else
                {
                    resultSegments.Push(segment);
                    meaningfulSegmentsCount++;
                }
            }

            var result = string.Join("/", resultSegments.Reverse());

            if (isBaseDirectoryAbsolute)
            {
                return System.IO.Path.GetPathRoot(baseDirectory) + result;
            }

            return "./" + result;
        }

        private string RemoveTrailingDotSlashes(string input)
        {
            if (input.Length <= 1 || input[0] != '.' || !DirectorySeparators.Contains(input[1]))
            {
                return input;
            }

            for (var i = 2; i < input.Length; i++)
            {
                if (!DirectorySeparators.Contains(input[i]))
                {
                    return input[i..];
                }
            }

            return string.Empty;
        }
    }
}
