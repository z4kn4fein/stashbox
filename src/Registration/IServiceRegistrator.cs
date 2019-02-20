using System;

namespace Stashbox.Registration
{
    internal interface IServiceRegistrator
    {
        IRegistrationContext PrepareContext(Type serviceType, Type implementationType);
        
        IRegistrationContext<TService> PrepareContext<TService>(Type serviceType, Type implementationType);
        
        IRegistrationContext PrepareContext(Type serviceType, Type implementationType, RegistrationContextData registrationContextData);
        
        IDecoratorRegistrationContext PrepareDecoratorContext(Type serviceType, Type implementationType);
        
        IStashboxContainer Register(IServiceRegistration serviceRegistration, bool replace);
        
        IStashboxContainer ReMap(IServiceRegistration serviceRegistration);
    }
}
