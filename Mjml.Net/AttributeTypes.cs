using Mjml.Net.Types;

namespace Mjml.Net;

public static class AttributeTypes
{
    public static readonly IType Align = new EnumType(false, "left", "center", "right");

    public static readonly IType AlignJustify = new EnumType(false, "left", "center", "right", "justify");

    public static readonly IType Boolean = new EnumType(false, "true", "false");

    public static readonly IType Color = new ColorType();

    public static readonly IType Direction = new EnumType(false, "ltr", "rtl");

    public static readonly IType LeftRight = new EnumType(false, "left", "right");

    public static readonly IType IncludeType = new EnumType(true, "mjml", "html", "css");

    public static readonly IType Pixels = new NumberType(Unit.Pixels);

    public static readonly IType PixelsOrAuto = new OneOfType(new EnumType(false, "auto"), Pixels);

    public static readonly IType PixelsOrEm = new NumberType(Unit.Pixels, Unit.Em);

    public static readonly IType PixelsOrPercent = new NumberType(Unit.Pixels, Unit.Percent);

    public static readonly IType PixelsOrPercentOrNone = new NumberType(Unit.Pixels, Unit.Percent, Unit.None);

    public static readonly IType FourPixelsOrPercent = new ManyType(PixelsOrPercent, 1, 4);

    public static readonly IType String = new StringType(false);

    public static readonly IType RequiredString = new StringType(true);

    public static readonly IType VerticalAlign = new EnumType(false, "top", "middle", "bottom");

    public static readonly IType SocialTableLayout = new EnumType(false, "auto", "fixed");

    public static readonly IType SocialMode = new EnumType(false, "vertical", "horizontal");

    public static readonly IType TextAlign = new EnumType(false, "left", "right", "center", "justify");
}
