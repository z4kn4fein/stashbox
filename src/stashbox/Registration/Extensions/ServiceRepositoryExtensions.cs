using Stashbox.Infrastructure.Registration;
using Stashbox.Utils;
using System;
using System.Linq;

namespace Stashbox.Registration.Extensions
{
    internal static class ServiceRepositoryExtensions
    {
        public static void AddOrUpdateRegistration(this HashMap<Type, ConcurrentOrderedKeyStore<object, IServiceRegistration>> repository, IServiceRegistration registration, bool remap, bool replace)
        {
            var newRepository = new ConcurrentOrderedKeyStore<object, IServiceRegistration>();
            newRepository.AddOrUpdate(registration.RegistrationContext.Name, registration);

            if (remap)
                repository.AddOrUpdate(registration.ServiceType, newRepository, (oldValue, newValue) => newValue);
            else
                repository.AddOrUpdate(registration.ServiceType, newRepository,
                    (oldValue, newValue) => oldValue.AddOrUpdate(registration.RegistrationContext.Name, registration, replace));
        }

        public static bool ContainsRegistration(this HashMap<Type, ConcurrentOrderedKeyStore<object, IServiceRegistration>> repository, Type type, object name)
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
