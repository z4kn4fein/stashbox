using Stashbox.Infrastructure.Registration;
using Stashbox.Utils;
using System;
using System.Linq;

namespace Stashbox.Registration.Extensions
{
    internal static class ServiceRepositoryExtensions
    {
        public static bool ContainsRegistration(this AvlTreeKeyValue<Type, ConcurrentOrderedKeyStore<object, IServiceRegistration>> repository, Type type, object name)
        {
            var registrations = repository.GetOrDefault(type);
            if (name != null && registrations != null)
                return registrations.GetOrDefault(name) != null;

            if (registrations != null || !type.IsClosedGenericType()) return registrations != null;

            registrations = repository.GetOrDefault(type.GetGenericTypeDefinition());
            return registrations?.Any(reg => reg.ValidateGenericContraints(type)) ?? false;
        }
    }
}
