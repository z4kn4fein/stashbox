using System;

namespace Stashbox.Registration.Fluent
{
    internal interface IRegistrationConfiguration
    {
        Type ServiceType { get; }

        Type ImplementationType { get; }

        RegistrationContext Context { get; }
    }
}
