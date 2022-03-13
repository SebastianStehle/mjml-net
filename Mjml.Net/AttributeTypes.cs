using Mjml.Net.AttributeValues;
using Mjml.Net.Components;

namespace Mjml.Net
{
    public static class AttributeTypes
    {
        public static readonly IAttribute Align = new EnumAttribute("left", "center", "right");

        public static readonly IAttribute Boolean = new EnumAttribute("true", "false");

        public static readonly IAttribute Color = new ColorAttribute();

        public static readonly IAttribute Pixels = new NumberAttribute(Unit.Pixels);

        public static readonly IAttribute PixelsOrAuto = new OneOfAttribute(new EnumAttribute("auto"), Pixels);

        public static readonly IAttribute PixelsOrPercent = new NumberAttribute(Unit.Pixels, Unit.Percent);

        public static readonly IAttribute FourPixelsOrPercent = new ManyAttribute(PixelsOrPercent, 1, 4);

        public static readonly IAttribute String = new StringAttribute();
    }
}
