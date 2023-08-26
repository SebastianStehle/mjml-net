using System.Text;
using Microsoft.Extensions.ObjectPool;
using Mjml.Net.Internal;

namespace Mjml.Net;

internal static class DefaultPools
{
    public static readonly ObjectPool<StringBuilder> StringBuilders = new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());

    public static readonly ObjectPool<Binder> Binders = new DefaultObjectPool<Binder>(new BinderPolicy());

    public static readonly ObjectPool<MjmlRenderContext> RenderContexts = new DefaultObjectPool<MjmlRenderContext>(new MjmlRenderContextPolicy());

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
    private sealed class BinderPolicy : PooledObjectPolicy<Binder>
    {
        public override Binder Create()
        {
            return new Binder();
        }

        public override bool Return(Binder obj)
        {
            obj.Clear();

            return true;
        }
    }
}
