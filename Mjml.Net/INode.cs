using System.Xml;

namespace Mjml.Net
{
    public interface INode
    {
        /// <summary>
        /// Get the attribute of the node with the given name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="withoutDefaults">Ignore defaults.</param>
        /// <returns>
        /// The attribute of the node or null if not found.
        /// </returns>
        string? GetAttribute(string name, bool withoutDefaults = false);

        /// <summary>
        /// Get the content of the node.
        /// </summary>
        /// <returns>
        /// The content of the node or null if not found.
        /// </returns>
        string? GetContent();

        /// <summary>
        /// Get raw access to the reader.
        /// </summary>
        /// <remarks>
        /// This is needed for components such as mj-attributes.
        /// </remarks>
        XmlReader Reader { get; }

        /// <summary>
        /// The current component.
        /// </summary>
        IComponent Component { get; }
    }
}
