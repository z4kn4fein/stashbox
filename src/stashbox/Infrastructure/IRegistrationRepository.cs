using System;
using System.Collections.Generic;
namespace Stashbox.Infrastructure
{
    public interface IRegistrationRepository
    {
        void AddRegistration(Type typeKey, IServiceRegistration registration, string nameKey);
        bool TryGetAllRegistrations(Type typeKey, out IEnumerable<IServiceRegistration> registrations);
        bool TryGetRegistration(Type typeKey, out IServiceRegistration registration, string nameKey = null);
        bool TryGetTypedRepositoryRegistrations(Type typeKey, out IDictionary<string, IServiceRegistration> registrations);
        bool ConstainsTypeKey(Type typeKey);
    }
}
