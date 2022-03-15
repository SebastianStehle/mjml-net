namespace Mjml.Net.Components
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public sealed record ContainerWidth(double Value, string String, string StringWithUnit)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    {
        public static readonly ContainerWidth Default = new ContainerWidth(600, "600", "600px");
    }
}
