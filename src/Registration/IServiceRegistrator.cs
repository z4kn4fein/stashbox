using System;

namespace Stashbox.Registration
{
    internal interface IServiceRegistrator
    {
        IRegistrationContext PrepareContext(Type serviceType, Type implementationType);

        IRegistrationContext<TService> PrepareContext<TService>(Type serviceType, Type implementationType);

        IRegistrationContext PrepareContext(Type serviceType, Type implementationType, RegistrationContextData registrationContextData);

        IDecoratorRegistrationContext PrepareDecoratorContext(Type serviceType, Type implementationType);

        IStashboxContainer Register(IServiceRegistration serviceRegistration, IRegistrationContext registrationContext);

        IStashboxContainer Register(IServiceRegistration serviceRegistration, Type serviceType, bool replace);

        IStashboxContainer ReMap(IServiceRegistration serviceRegistration, IRegistrationContext registrationContext);
    }
}
