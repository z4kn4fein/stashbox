using Stashbox.Entity;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    public interface IRegistrationRepository
    {
        void AddRegistration(Type typeKey, IServiceRegistration registration, string nameKey);
        void AddOrUpdateRegistration(Type typeKey, IServiceRegistration registration, string nameKey);
        void AddGenericDefinition(Type typeKey, IServiceRegistration registration, string nameKey);
        void AddOrUpdateGenericDefinition(Type typeKey, IServiceRegistration registration, string nameKey);
        IEnumerable<IServiceRegistration> GetAllRegistrations();
        bool TryGetRegistrationWithConditions(TypeInformation typeInfo, out IServiceRegistration registration);
        bool TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo,
            out IServiceRegistration registration);
        bool TryGetRegistration(TypeInformation typeInfo, out IServiceRegistration registration);
        bool TryGetTypedRepositoryRegistrations(TypeInformation typeInfo, out IServiceRegistration[] registrations);
        bool Constains(Type type, string name);
        bool ConstainsTypeKeyWithConditions(TypeInformation typeInfo);
        bool ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo);
        void CleanUp();
    }
}
