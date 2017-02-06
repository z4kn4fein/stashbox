using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Resolution
{
    internal class EnumerableResolver : Resolver
    {
        private readonly IServiceRegistration[] registrationCache;
        private readonly TypeInformation enumerableType;

        public EnumerableResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.enumerableType = new TypeInformation
            {
                Type = typeInfo.Type.GetEnumerableType()
            };

            var registrations = containerContext.RegistrationRepository.GetRegistrationsOrDefault(this.enumerableType);

            if (registrations != null)
                registrationCache = base.BuilderContext.ContainerConfiguration.EnumerableOrderRule(registrations).ToArray();
        }

        public override Type WrappedType => this.enumerableType.Type;

        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            if (registrationCache == null)
                return Expression.NewArrayInit(this.enumerableType.Type);

            var length = registrationCache.Length;
            var enumerableItems = new Expression[length];
            for (var i = 0; i < length; i++)
                enumerableItems[i] = registrationCache[i].GetExpression(resolutionInfo, this.enumerableType);

            return Expression.NewArrayInit(this.enumerableType.Type, enumerableItems);
        }

        public static bool IsAssignableToGenericType(Type type, Type genericType)
        {
            if (type == null || genericType == null) return false;

            return type == genericType
              || MapsToGenericTypeDefinition(type, genericType)
              || HasInterfaceThatMapsToGenericTypeDefinition(type, genericType)
              || IsAssignableToGenericType(type.GetTypeInfo().BaseType, genericType);
        }

        private static bool HasInterfaceThatMapsToGenericTypeDefinition(Type type, Type genericType)
        {
            return type.GetTypeInfo().ImplementedInterfaces
              .Where(it => it.GetTypeInfo().IsGenericType)
              .Any(it => it.GetGenericTypeDefinition() == genericType);
        }

        private static bool MapsToGenericTypeDefinition(Type type, Type genericType)
        {
            return genericType.GetTypeInfo().IsGenericTypeDefinition
              && type.GetTypeInfo().IsGenericType
              && type.GetGenericTypeDefinition() == genericType;
        }
    }
}
