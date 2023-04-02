using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Mjml.Net.Internal
{
    internal static class ReflectionHelper
    {
        public static Func<T1, T2, TReturn> CreateFactory<T1, T2, TReturn>(this ConstructorInfo constructorInfo)
        {
            var parameters = new[]
            {
                Expression.Parameter(typeof(T1)),
                Expression.Parameter(typeof(T2))
            };

            var constructorExpression = Expression.New(constructorInfo, parameters);

            return Expression.Lambda<Func<T1, T2, TReturn>>(constructorExpression, parameters).Compile();

        }
        public static Func<object, TReturn> CreateILDelegate<TReturn>(this MethodInfo methodInfo)
        {
            var parameterTypes = new Type[]
            {
                typeof(object)
            };

            var method = new DynamicMethod(methodInfo.Name, typeof(TReturn), parameterTypes);

            var il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_S, 0);
            il.Emit(OpCodes.Call, methodInfo);
            il.Emit(OpCodes.Ret);

            return (Func<object, TReturn>)method.CreateDelegate(typeof(Func<object, TReturn>));
        }
    }
}
