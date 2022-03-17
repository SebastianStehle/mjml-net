using Mjml.Net.Components;
using Mjml.Net.Types;

namespace Mjml.Net
{
    public static class AttributeTypes
    {
        public static readonly IType Align = new EnumType("left", "center", "right");

        public static readonly IType Boolean = new EnumType("true", "false");

        public static readonly IType Color = new ColorType();

        public static readonly IType Pixels = new NumberType(Unit.Pixels);

        public static readonly IType PixelsOrAuto = new OneOfType(new EnumType("auto"), Pixels);

        public static readonly IType PixelsOrPercent = new NumberType(Unit.Pixels, Unit.Percent);

        public static readonly IType PixelsOrPercentOrNone = new NumberType(Unit.Pixels, Unit.Percent, Unit.None);

        public static readonly IType FourPixelsOrPercent = new ManyType(PixelsOrPercent, 1, 4);

        public static readonly IType String = new StringType();

        public static readonly IType VerticalAlign = new EnumType("top", "middle", "bottom");

        public static readonly IType SocialTableLayout = new EnumType("auto", "fixed");

        public static readonly IType SocialMode = new EnumType("vertical", "horizontal");
    }
}
