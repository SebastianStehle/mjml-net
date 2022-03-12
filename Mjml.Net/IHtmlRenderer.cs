namespace Mjml.Net
{
    public interface IHtmlRenderer
    {
        IElementHtmlRenderer StartElement(string elementName);

        void EndElement(string elementName);

        /// <summary>
        /// Renders a plain value.
        /// </summary>
        /// <param name="value">The value to render.</param>
        void Plain(string? value);

        /// <summary>
        /// Renders the content of an element.
        /// </summary>
        /// <param name="value">The value to render.</param>
        void Content(string? value);

        void RenderChildren();

        void RenderChildren<T>(ChildOptions<T> options);

        void SetContext(string name, object? value);

        void SetGlobalData(string name, object value, bool skipIfAdded = true);

        void SetDefaultAttribute(string name, string? type, string value);

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
    }
}