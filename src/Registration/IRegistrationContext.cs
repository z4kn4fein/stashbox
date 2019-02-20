using System;
using Stashbox.Utils;

namespace Stashbox.Registration
{
    internal interface IRegistrationContext<TService> : IRegistrationContext, IFluentServiceRegistrator<TService>
    { }
    
    internal interface IRegistrationContext : IFluentServiceRegistrator
    {
        RegistrationContextData Context { get; }
        
        bool ReplaceExistingRegistration { get; }

        ArrayStore<Type> AdditionalServiceTypes { get; }
    }
}
