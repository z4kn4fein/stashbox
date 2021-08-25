﻿using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the generic fluent service registration api.
    /// </summary>
    public class RegistrationConfigurator<TService, TImplementation> :
        FluentServiceConfigurator<TService, TImplementation, RegistrationConfigurator<TService, TImplementation>>
    {
        internal RegistrationConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        { }

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="wireUp">If true, the instance will be wired into the container, it will perform member and method injection on it.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator<TService, TImplementation> WithInstance(TService instance, bool wireUp = false)
        {
            this.Context.ExistingInstance = instance;
            this.Context.IsWireUp = wireUp;
            this.ImplementationType = instance.GetType();
            return this;
        }
    }

    /// <summary>
    /// Represents the fluent service registration api.
    /// </summary>
    public class RegistrationConfigurator : FluentServiceConfigurator<RegistrationConfigurator>
    {
        internal RegistrationConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
        }

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The container factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithFactory(Func<IDependencyResolver, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, Constants.ResolverType, Constants.ObjectType);
            return this;
        }

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithFactory<T1>(Func<T1, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), Constants.ObjectType);
            return this;
        }

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithFactory<T1, T2>(Func<T1, T2, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), Constants.ObjectType);
            return this;
        }

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithFactory<T1, T2, T3>(Func<T1, T2, T3, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), Constants.ObjectType);
            return this;
        }

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(T4), Constants.ObjectType);
            return this;
        }

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), Constants.ObjectType);
            return this;
        }

        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithFactory(Func<object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, Constants.ObjectType);
            return this;
        }

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="wireUp">If true, the instance will be wired into the container, it will perform member and method injection on it.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithInstance(object instance, bool wireUp = false)
        {
            this.Context.ExistingInstance = instance;
            this.Context.IsWireUp = wireUp;
            this.ImplementationType = instance.GetType();
            return this;
        }
    }
}
