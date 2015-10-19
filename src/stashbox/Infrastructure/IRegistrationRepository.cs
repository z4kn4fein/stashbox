using Stashbox.Entity;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    public interface IRegistrationRepository
    {
        void AddRegistration(Type typeKey, IServiceRegistration registration, string nameKey);
        bool TryGetAllRegistrations(TypeInformation typeInfo, out IServiceRegistration[] registrations);
        bool TryGetRegistrationWithConditions(TypeInformation typeInfo, out IServiceRegistration registration);
        bool TryGetRegistration(TypeInformation typeInfo, out IServiceRegistration registration);
        bool TryGetTypedRepositoryRegistrations(TypeInformation typeInfo, out IDictionary<string, IServiceRegistration> registrations);
        bool ConstainsTypeKey(TypeInformation typeInfo);
        bool ConstainsTypeKeyWithConditions(TypeInformation typeInfo);
        bool ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo);
        void CleanUp();
    }
}
