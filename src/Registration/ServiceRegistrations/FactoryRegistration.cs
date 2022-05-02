using System;
using System.Linq.Expressions;

namespace Stashbox.Registration.ServiceRegistrations
{
    /// <summary>
    /// Describes a factory service registration.
    /// </summary>
    public class FactoryRegistration : ComplexRegistration
    {
        /// <summary>
        /// Container factory of the registration.
        /// </summary>
        public readonly Delegate Factory;

        /// <summary>
        /// Parameters to inject for the factory registration.
        /// </summary>
        public readonly Type[] FactoryParameters;

        /// <summary>
        /// Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.
        /// </summary>
        public readonly bool IsFactoryDelegateACompiledLambda;

        internal FactoryRegistration(Delegate factory, Type[] factoryParameters, bool isCompiledLambda, ServiceRegistration baseRegistration)
            : base(baseRegistration)
        {
            this.Factory = factory;
            this.FactoryParameters = factoryParameters;
            this.IsFactoryDelegateACompiledLambda = isCompiledLambda;
        }
    }
}