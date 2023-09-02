using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Wrappers;

internal class KeyValueWrapper : IServiceWrapper
{
    private static readonly HashSet<Type> SupportedTypes = new()
    {
        typeof(KeyValuePair<,>),
        typeof(ReadOnlyKeyValue<,>),
    };

    private static bool IsKeyValueType(Type type) => type.IsClosedGenericType() && SupportedTypes.Contains(type.GetGenericTypeDefinition());

    public Expression WrapExpression(TypeInformation originalTypeInformation, TypeInformation wrappedTypeInformation, 
        ServiceContext serviceContext)
    {
        var arguments = originalTypeInformation.Type.GetGenericArguments();
        var constructor = originalTypeInformation.Type.GetConstructor(arguments)!;
        var name = serviceContext.ServiceRegistration?.Name;
        return constructor.MakeNew(name.AsConstant(), serviceContext.ServiceExpression);
    }

    public bool TryUnWrap(Type type, out Type unWrappedType)
    {
        if (!IsKeyValueType(type))
        {
            unWrappedType = TypeCache.EmptyType;
            return false;
        }

        var arguments = type.GetGenericArguments();
        var nameType = arguments[0];

        if (nameType != TypeCache<object>.Type)
        {
            unWrappedType = TypeCache.EmptyType;
            return false;
        }

        unWrappedType = arguments[1];
        return true;
    }
}