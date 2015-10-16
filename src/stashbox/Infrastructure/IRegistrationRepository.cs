using Stashbox.Entity;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    public interface IRegistrationRepository
    {
        void AddRegistration(Type typeKey, IServiceRegistration registration, string nameKey);
        bool TryGetAllRegistrations(TypeInformation typeInfo, out IEnumerable<IServiceRegistration> registrations);
        bool TryGetRegistration(TypeInformation typeInfo, out IServiceRegistration registration);
        bool TryGetTypedRepositoryRegistrations(TypeInformation typeInfo, out IDictionary<string, IServiceRegistration> registrations);
        bool ConstainsTypeKey(TypeInformation typeInfo);
        bool ConstainsTypeKeyWithoutGenericDefinitionExtraction(TypeInformation typeInfo);
    }
}
