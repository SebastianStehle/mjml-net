using Mjml.Net.Types;
using Mjml.Net.Components;

namespace Mjml.Net
{
    public static class AttributeTypes
    {
        public static readonly IType Align = new EnumType("left", "center", "right");

        public static readonly IType Boolean = new EnumType("true", "false");

        public static readonly IType Color = new ColorType();

        public static readonly IType Pixels = new NumberType(UnitType.Pixels);

        public static readonly IType PixelsOrAuto = new OneOfType(new EnumType("auto"), Pixels);

        public static readonly IType PixelsOrPercent = new NumberType(UnitType.Pixels, UnitType.Percent);

        public static readonly IType FourPixelsOrPercent = new ManyType(PixelsOrPercent, 1, 4);

        public static readonly IType String = new StringType();
    }
}
