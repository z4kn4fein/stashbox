using Stashbox.Entity;
using System.Collections.Generic;

namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IRegistrationExtension : IContainerExtension
    {
        void OnRegistration(IContainerContext containerContext, RegistrationInfo registrationInfo, HashSet<InjectionParameter> injectionParameters = null);
    }
}
