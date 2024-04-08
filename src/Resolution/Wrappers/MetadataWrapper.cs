using Stashbox.Registration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Stashbox.Utils;

namespace Stashbox.Resolution.Wrappers;

internal class MetadataWrapper : IMetadataWrapper
{
    private static readonly HashSet<Type> SupportedTypes =
    [
        typeof(ValueTuple<,>),
        typeof(Tuple<,>),
        typeof(Metadata<,>)
    ];

    private static bool IsMetadataType(Type type) => type.IsClosedGenericType() && SupportedTypes.Contains(type.GetGenericTypeDefinition());

    public Expression WrapExpression(TypeInformation originalTypeInformation, TypeInformation wrappedTypeInformation, 
        ServiceContext serviceContext)
    {
        var arguments = originalTypeInformation.Type.GetGenericArguments();
        var constructor = originalTypeInformation.Type.GetConstructor(arguments)!;
        var metadata = serviceContext.ServiceRegistration?.Options.GetOrDefault(RegistrationOption.Metadata);
        return constructor.MakeNew(serviceContext.ServiceExpression, metadata.AsConstant());
    }

    public bool TryUnWrap(Type type, out Type unWrappedType, out Type metadataType)
    {
        if (!IsMetadataType(type))
        {
            unWrappedType = TypeCache.EmptyType;
            metadataType = TypeCache.EmptyType;
            return false;
        }

        var arguments = type.GetGenericArguments();
        var serviceType = arguments[0];
        metadataType = arguments[1];
        unWrappedType = serviceType;
        return true;
    }
}