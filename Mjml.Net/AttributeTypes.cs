using Mjml.Net.AttributeValues;

namespace Mjml.Net
{
    public static class AttributeTypes
    {
        public static readonly IAttribute Color = new ColorAttribute();

        public static readonly IAttribute Pixels = new NumberAttribute("px");

        public static readonly IAttribute PixelsOrPercent = new NumberAttribute("px", "%");

        public static readonly IAttribute String = new StringAttribute();
    }
}
