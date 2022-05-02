using Stashbox.Lifetime;
using System;

namespace Stashbox.Registration.ServiceRegistrations
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

        internal FuncRegistration(Delegate funcDelegate, Type implementationType, object? name,
            LifetimeDescriptor lifetimeDescriptor, bool isDecorator)
            : base(implementationType, name, lifetimeDescriptor, isDecorator)
        {
            this.FuncDelegate = funcDelegate;
        }
    }
}