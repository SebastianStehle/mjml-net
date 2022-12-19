using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Mjml.Net
{
    internal static class DefaultPools
    {
        public static readonly ObjectPool<StringBuilder> StringBuilders = new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());
    }
}
