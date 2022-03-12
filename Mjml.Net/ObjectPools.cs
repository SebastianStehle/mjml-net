using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace Mjml.Net
{
    internal static class ObjectPools
    {
        public static readonly ObjectPool<StringBuilder> StringBuilder =
            new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());

        public static readonly ObjectPool<MjmlRenderContext> Contexts =
            new DefaultObjectPool<MjmlRenderContext>(new DefaultPooledObjectPolicy<MjmlRenderContext>());
    }
}
