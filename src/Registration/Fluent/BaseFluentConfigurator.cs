using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TConfigurator"></typeparam>
    public class BaseFluentConfigurator<TConfigurator> : IBaseFluentConfigurator<TConfigurator>
        where TConfigurator : BaseFluentConfigurator<TConfigurator>
    {
        internal Type ServiceType { get; }

        internal Type ImplementationType { get; }

        internal RegistrationContextData Context { get; }

        internal bool ReplaceExistingRegistration { get; private set; }

        internal IEnumerable<Type> AdditionalServiceTypes { get; set; }

        internal BaseFluentConfigurator(Type serviceType, Type implementationType)
            : this(serviceType, implementationType, RegistrationContextData.New())
        { }

        internal BaseFluentConfigurator(Type serviceType, Type implementationType, RegistrationContextData registrationContextData)
        {
            this.ServiceType = serviceType;
            this.ImplementationType = implementationType;
            this.Context = registrationContextData;
            this.AdditionalServiceTypes = ArrayStore<Type>.Empty;
        }

        /// <inheritdoc />
        public TConfigurator WithInjectionParameters(params InjectionParameter[] injectionParameters)
        {
            var length = injectionParameters.Length;
            for (int i = 0; i < length; i++)
                this.AddInjectionParameter(injectionParameters[i]);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithInjectionParameter(string name, object value)
        {
            this.AddInjectionParameter(new InjectionParameter { Name = name, Value = value });
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithAutoMemberInjection(Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, Func<TypeInformation, bool> filter = null)
        {
            this.Context.AutoMemberInjectionEnabled = true;
            this.Context.AutoMemberInjectionRule = rule;
            this.Context.MemberInjectionFilter = filter;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> rule)
        {
            this.Context.ConstructorSelectionRule = rule;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithConstructorByArgumentTypes(params Type[] argumentTypes)
        {
            var constructor = this.ImplementationType.GetConstructorByTypes(argumentTypes);
            if (constructor == null)
                this.ThrowConstructorNotFoundException(this.ImplementationType, argumentTypes);

            this.Context.SelectedConstructor = constructor;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithConstructorByArguments(params object[] arguments)
        {
            var argTypes = arguments.Select(arg => arg.GetType()).ToArray();
            var constructor = this.ImplementationType.GetConstructorByTypes(argTypes);
            if (constructor == null)
                this.ThrowConstructorNotFoundException(this.ImplementationType, argTypes);

            this.Context.SelectedConstructor = constructor;
            this.Context.ConstructorArguments = arguments;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator InjectMember(string memberName, object dependencyName = null)
        {
            this.Context.InjectionMemberNames.Add(memberName, dependencyName);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithoutDisposalTracking()
        {
            this.Context.IsLifetimeExternallyOwned = true;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator ReplaceExisting()
        {
            this.ReplaceExistingRegistration = true;
            return (TConfigurator)this;
        }

        private void AddInjectionParameter(InjectionParameter injectionParameter)
        {
            var store = (ArrayStore<InjectionParameter>)this.Context.InjectionParameters;
            this.Context.InjectionParameters = store.Add(injectionParameter);
        }

        private void ThrowConstructorNotFoundException(Type type, params Type[] argTypes)
        {
            if (argTypes.Length == 0)
                throw new ConstructorNotFoundException(type);

            if (argTypes.Length == 1)
                throw new ConstructorNotFoundException(type, argTypes[0]);

            throw new ConstructorNotFoundException(type, argTypes);
        }
    }
}
