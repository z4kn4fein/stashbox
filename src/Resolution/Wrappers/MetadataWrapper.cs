using Stashbox.Registration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Wrappers
{
    internal class MetadataWrapper : IServiceWrapper
    {
        private static readonly HashSet<Type> SupportedTypes = new()
        {
            typeof(ValueTuple<,>),
            typeof(Tuple<,>),
            typeof(Metadata<,>),
        };

        private static bool IsMetadataType(Type type) => type.IsClosedGenericType() && SupportedTypes.Contains(type.GetGenericTypeDefinition());

        public Expression WrapExpression(TypeInformation originalTypeInformation, TypeInformation wrappedTypeInformation, 
            ServiceContext serviceContext)
        {
            var arguments = originalTypeInformation.Type.GetGenericArguments();
            var constructor = originalTypeInformation.Type.GetConstructor(arguments)!;
            var metadata = serviceContext.ServiceRegistration?.Options.GetOrDefault(RegistrationOption.Metadata);
            return constructor.MakeNew(serviceContext.ServiceExpression, metadata.AsConstant());
        }

        public bool TryUnWrap(TypeInformation typeInformation, out TypeInformation unWrappedType)
        {
            if (!IsMetadataType(typeInformation.Type))
            {
                unWrappedType = TypeInformation.Empty;
                return false;
            }

            var arguments = typeInformation.Type.GetGenericArguments();
            var serviceType = arguments[0];
            var metadataType = arguments[1];
            unWrappedType = typeInformation.Clone(serviceType, metadataType: metadataType);
            return true;
        }
    }
}
