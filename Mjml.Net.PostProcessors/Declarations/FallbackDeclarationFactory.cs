using AngleSharp.Css;

namespace Mjml.Net.Declarations;

public class FallbackDeclarationFactory : IDeclarationFactory
{
    private readonly DefaultDeclarationFactory defaultFactory = new DefaultDeclarationFactory();

    public DeclarationInfo Create(string propertyName)
    {
        var declaration = defaultFactory.Create(propertyName);

        var withConverter = new DeclarationInfo(
            declaration.Name,
            new FallbackConverter(declaration.Converter),
            declaration.Flags,
            declaration.InitialValue,
            declaration.Shorthands,
            declaration.Longhands);

        return withConverter;
    }
}
