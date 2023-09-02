using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Wrappers;

internal class LazyWrapper : IServiceWrapper
{
    private static bool IsLazy(Type type) => type.IsClosedGenericType() && type.GetGenericTypeDefinition() == typeof(Lazy<>);

    public Expression WrapExpression(TypeInformation originalTypeInformation, TypeInformation wrappedTypeInformation,
        ServiceContext serviceContext)
    {
        var ctorParamType = TypeCache.FuncType.MakeGenericType(wrappedTypeInformation.Type);
        var lazyConstructor = originalTypeInformation.Type.GetConstructor(ctorParamType)!;
        return lazyConstructor.MakeNew(serviceContext.ServiceExpression.AsLambda());
    }

    public bool TryUnWrap(Type type, out Type unWrappedType)
    {
        if (!IsLazy(type))
        {
            unWrappedType = TypeCache.EmptyType;
            return false;
        }

        unWrappedType = type.GetGenericArguments()[0];
        return true;
    }
}