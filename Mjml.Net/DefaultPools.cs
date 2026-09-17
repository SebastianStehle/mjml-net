using System.Text;
using Microsoft.Extensions.ObjectPool;
using Mjml.Net.Internal;

namespace Mjml.Net;

internal static class DefaultPools
{
    // Rendered emails are typically 10-130K characters.
    // With the default limit of 4K characters the output buffer would never be reused.
    public static readonly ObjectPool<StringBuilder> StringBuilders = new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy
    {
        MaximumRetainedCapacity = 256 * 1024
    });

    public static readonly ObjectPool<Binder> Binders = new DefaultObjectPool<Binder>(new BinderPolicy());

    public static readonly ObjectPool<MjmlRenderContext> RenderContexts = new DefaultObjectPool<MjmlRenderContext>(new MjmlRenderContextPolicy());

    public static readonly ObjectPool<HtmlReaderWrapper> HtmlReaders = new DefaultObjectPool<HtmlReaderWrapper>(new HtmlReaderPolicy());

    private sealed class HtmlReaderPolicy : PooledObjectPolicy<HtmlReaderWrapper>
    {
        public override HtmlReaderWrapper Create()
        {
            return new HtmlReaderWrapper(string.Empty);
        }

        public override bool Return(HtmlReaderWrapper obj)
        {
            obj.Clear();
            return true;
        }
    }

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
