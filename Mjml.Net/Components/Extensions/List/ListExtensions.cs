using Mjml.Net.Components.Body;
using Mjml.Net.Components.Extensions.List;

namespace Mjml.Net;

public static class ListExtensions
{
    public static IMjmlRenderer AddList(this IMjmlRenderer renderer)
    {
        return renderer.Add<ListComponent>().Add<ListItemComponent>();
    }
}
