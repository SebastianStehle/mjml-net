namespace Mjml.Net;

public interface IValidator
{
    void Attribute(string name, string value, IComponent component, ValidationErrors errors, ref ValidationContext context);

    void Components(IComponent root, ValidationErrors errors, ref ValidationContext context);
}
