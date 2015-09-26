using Stashbox.Entity;
using System;

namespace Stashbox.Infrastructure
{
    public interface IRegistrationContext
    {
        IRegistrationContext WithLifetime(ILifetime lifetime);
        IRegistrationContext WithName(string name);
        IRegistrationContext WithInjectionParameters(params InjectionParameter[] injectionParameters);
        IRegistrationContext WithFactoryParameters(Func<object, object> singleParameterFactory);
        IRegistrationContext WithFactoryParameters(Func<object, object, object> twoParametersFactory);
        IRegistrationContext WithFactoryParameters(Func<object, object, object, object> threeParametersFactory);
        IStashboxContainer Register();
    }
}
