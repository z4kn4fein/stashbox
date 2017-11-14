using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Collections.Generic;

namespace Stashbox.Registration
{
    internal class DecoratorRegistrationContext : IDecoratorRegistrationContext
    {
        private readonly RegistrationContext registrationContext;
        private readonly IServiceRegistrator serviceRegistrator;
        private bool replaceExistingRegistration;

        public Type ServiceType => this.registrationContext.ServiceType;

        public Type ImplementationType => this.registrationContext.ImplementationType;

        public DecoratorRegistrationContext(RegistrationContext registrationContext, IServiceRegistrator serviceRegistrator)
        {
            this.registrationContext = registrationContext;
            this.serviceRegistrator = serviceRegistrator;
        }

        public IStashboxContainer Register() => this.serviceRegistrator.Register(this.registrationContext, true, this.replaceExistingRegistration);

        public IStashboxContainer ReMap() => this.serviceRegistrator.ReMap(this.registrationContext, true);

        public IFluentDecoratorRegistrator WithInjectionParameters(params InjectionParameter[] injectionParameters)
        {
            this.registrationContext.WithInjectionParameters(injectionParameters);
            return this;
        }

        public IFluentDecoratorRegistrator WithAutoMemberInjection(
            Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter)
        {
            this.registrationContext.WithAutoMemberInjection(rule);
            return this;
        }

        public IFluentDecoratorRegistrator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> rule)
        {
            this.registrationContext.WithConstructorSelectionRule(rule);
            return this;
        }

        public IFluentDecoratorRegistrator WithoutDisposalTracking()
        {
            this.registrationContext.WithoutDisposalTracking();
            return this;
        }

        public IFluentDecoratorRegistrator ReplaceExisting()
        {
            this.replaceExistingRegistration = true;
            return this;
        }

        public IFluentDecoratorRegistrator WithConstructorByArgumentTypes(params Type[] argumentTypes)
        {
            this.registrationContext.WithConstructorByArgumentTypes(argumentTypes);
            return this;
        }

        public IFluentDecoratorRegistrator WithConstructorByArguments(params object[] arguments)
        {
            this.registrationContext.WithConstructorByArguments(arguments);
            return this;
        }

        public IFluentDecoratorRegistrator InjectMember(string memberName, object dependencyName = null)
        {
            this.registrationContext.InjectMember(memberName, dependencyName);
            return this;
        }
    }
}
