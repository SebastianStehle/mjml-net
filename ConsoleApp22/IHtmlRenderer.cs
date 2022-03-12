namespace ConsoleApp22
{
    public interface IHtmlRenderer
    {
        IElementHtmlRenderer StartElement(string elementName);

        void EndElement(string elementName);

        void Plain(string value);

        void RenderChildren();

        void SetContext(string name, object? value);

        object? GetContext(string name);
    }

    public interface IElementHtmlRenderer
    {
        IElementHtmlRenderer SetAttribute(string name, string? value);

        IElementHtmlRenderer SetClass(string? value);

        IElementHtmlRenderer SetStyle(string name, string? value);

        IElementHtmlRenderer Done();
    }
}