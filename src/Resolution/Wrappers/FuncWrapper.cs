using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Wrappers;

internal class FuncWrapper : IParameterizedWrapper
{
    public Expression WrapExpression(TypeInformation originalTypeInformation, TypeInformation wrappedTypeInformation,
        ServiceContext serviceContext, IEnumerable<ParameterExpression> parameterExpressions) =>
        serviceContext.ServiceExpression.AsLambda(originalTypeInformation.Type, parameterExpressions);

    public bool TryUnWrap(Type type, out Type unWrappedType, out IEnumerable<Type> parameterTypes)
    {
        if (!type.IsSubclassOf(TypeCache<Delegate>.Type))
        {
            unWrappedType = TypeCache.EmptyType;
            parameterTypes = TypeCache.EmptyTypes;
            return false;
        }

        var method = type.GetMethod("Invoke");
        if (method == null || method.ReturnType == TypeCache.VoidType)
        {
            unWrappedType = TypeCache.EmptyType;
            parameterTypes = TypeCache.EmptyTypes;
            return false;
        }

        unWrappedType = method.ReturnType;
        parameterTypes = method.GetParameters().Select(p => p.ParameterType);
        return true;
    }
}