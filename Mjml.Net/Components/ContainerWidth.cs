namespace Mjml.Net.Components
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public sealed record ContainerWidth(double Value, string String, string StringWithUnit)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    {
        public static readonly ContainerWidth Default = Create(600);

        public static ContainerWidth Create(double number)
        {
            return new ContainerWidth(number, number.ToInvariantString(), $"{number}px");
        }
    }
}
