using Stashbox.Configuration;
using System;

namespace Stashbox.Registration
{
    /// <summary>
    /// Describes a <see cref="Func{TResult}"/> service registrations.
    /// </summary>
    public class FuncRegistration : ServiceRegistration
    {
        /// <summary>
        /// The delegate to resolve when the registration is a 'Func{}'.
        /// </summary>
        public readonly Delegate FuncDelegate;

        internal FuncRegistration(Type implementationType, RegistrationContext registrationContext,
            ContainerConfiguration containerConfiguration, bool isDecorator, Delegate funcDelegate)
            : base(implementationType, registrationContext, containerConfiguration, isDecorator)
        {
            this.FuncDelegate = funcDelegate;
        }
    }
}