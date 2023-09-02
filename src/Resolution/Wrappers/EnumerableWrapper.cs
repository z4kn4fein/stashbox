using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.Utils;

namespace Stashbox.Resolution.Wrappers;

internal class EnumerableWrapper : IEnumerableWrapper
{
    public Expression WrapExpression(TypeInformation originalTypeInformation,
        TypeInformation wrappedTypeInformation,
        IEnumerable<ServiceContext> serviceContexts) =>
        wrappedTypeInformation.Type.InitNewArray(serviceContexts.Select(e => e.ServiceExpression));

    public bool TryUnWrap(Type type, out Type unWrappedType)
    {
        var enumerableType = type.GetEnumerableType();
        if (enumerableType == null)
        {
            unWrappedType = TypeCache.EmptyType;
            return false;
        }

        unWrappedType = enumerableType;
        return true;
    }
}