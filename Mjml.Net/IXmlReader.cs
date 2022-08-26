using System.Xml;

namespace Mjml.Net
{
    /// <summary>
    /// Reads XML for MJML.
    /// </summary>
    public interface IXmlReader
    {
        /// <summary>
        /// Read a xml fragment.
        /// </summary>
        /// <param name="xml">The xml fragment reader.</param>
        /// <param name="parent">The parent component.</param>
        void ReadFragment(XmlReader xml, IComponent? parent);
    }
}
