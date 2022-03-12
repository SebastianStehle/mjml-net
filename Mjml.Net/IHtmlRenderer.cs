namespace Mjml.Net
{
    public interface IHtmlRenderer
    {
        IElementHtmlRenderer StartElement(string elementName);

        void EndElement(string elementName);

        void Plain(string? value);

        void Content(string? value);

        void RenderChildren();

        void RenderChildren<T>(ChildOptions<T> options);

        void SetContext(string name, object? value);

        void SetGlobalData(string name, object values);

        object? GetContext(string name);
    }

    public interface IChildRenderer
    {
        void Render();

        INode Node { get; }
    }

    public interface IElementHtmlRenderer
    {
        IElementHtmlRenderer Attr(string name, string? value);

        IElementHtmlRenderer Class(string? value);

        IElementHtmlRenderer Style(string name, string? value);

        IElementHtmlRenderer Done();
    }
}