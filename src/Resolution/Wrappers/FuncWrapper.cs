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

    public bool TryUnWrap(TypeInformation typeInformation, out TypeInformation unWrappedType, out IEnumerable<Type> parameterTypes)
    {
        if (!typeInformation.Type.IsSubclassOf(TypeCache<Delegate>.Type))
        {
            unWrappedType = TypeInformation.Empty;
            parameterTypes = TypeCache.EmptyTypes;
            return false;
        }

        var method = typeInformation.Type.GetMethod("Invoke");
        if (method == null || method.ReturnType == TypeCache.VoidType)
        {
            unWrappedType = TypeInformation.Empty;
            parameterTypes = TypeCache.EmptyTypes;
            return false;
        }

        unWrappedType = typeInformation.Clone(method.ReturnType);
        parameterTypes = method.GetParameters().Select(p => p.ParameterType);
        return true;
    }
}