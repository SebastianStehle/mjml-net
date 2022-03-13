using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Mjml.Net
{
    internal static class ObjectPools
    {
        public static readonly ObjectPool<StringBuilder> StringBuilder =
            new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());

        public static readonly ObjectPool<MjmlRenderContext> Contexts =
            new DefaultObjectPool<MjmlRenderContext>(new MjmlRenderContextPolicy());

        private sealed class MjmlRenderContextPolicy : PooledObjectPolicy<MjmlRenderContext>
        {
            public override MjmlRenderContext Create()
            {
                return new MjmlRenderContext();
            }

            public override bool Return(MjmlRenderContext obj)
            {
                obj.Clear();

                return true;
            }
        }
    }
}
