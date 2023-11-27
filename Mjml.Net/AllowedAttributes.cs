namespace Mjml.Net;

public sealed class AllowedAttributes : Dictionary<string, IType>
{
    public AllowedAttributes()
    {
    }

    public AllowedAttributes(AllowedAttributes source)
        : base(source)
    {
    }
}
