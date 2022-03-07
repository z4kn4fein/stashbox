using System;
using System.Linq.Expressions;
using Stashbox.Configuration;

namespace Stashbox.Registration
{
    /// <summary>
    /// Describes a factory service registration.
    /// </summary>
    public class FactoryRegistration : ServiceRegistration
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
        
        internal FactoryRegistration(Type implementationType, RegistrationContext registrationContext, 
            ContainerConfiguration containerConfiguration, bool isDecorator, Delegate factory, Type[] factoryParameters) 
            : base(implementationType, registrationContext, containerConfiguration, isDecorator)
        {
            this.Factory = factory;
            this.FactoryParameters = factoryParameters;
            this.IsFactoryDelegateACompiledLambda = registrationContext.IsFactoryDelegateACompiledLambda;
        }
    }
}