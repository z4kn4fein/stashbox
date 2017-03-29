using System;
using System.Collections.Generic;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Registration
{
    internal class DecoratorRegistrationContext : IDecoratorRegistrationContext
    {
        private readonly IRegistrationContext registrationContext;
        private readonly IContainerContext containerContext;
        private readonly Type typeFrom;

        public DecoratorRegistrationContext(IRegistrationContext registrationContext, IContainerContext containerContext, Type typeFrom)
        {
            this.registrationContext = registrationContext;
            this.containerContext = containerContext;
            this.typeFrom = typeFrom;
        }

        public IDecoratorRegistrationContext WithInjectionParameters(params InjectionParameter[] injectionParameters)
        {
            this.registrationContext.WithInjectionParameters(injectionParameters);
            return this;
        }

        public IDecoratorRegistrationContext WithAutoMemberInjection(
            Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter)
        {
            this.registrationContext.WithAutoMemberInjection(rule);
            return this;
        }

        public IDecoratorRegistrationContext WithConstructorSelectionRule(Func<IEnumerable<ResolutionConstructor>, ResolutionConstructor> rule)
        {
            this.registrationContext.WithConstructorSelectionRule(rule);
            return this;
        }

        public IDecoratorRegistrationContext WithoutDisposalTracking()
        {
            this.registrationContext.WithoutDisposalTracking();
            return this;
        }

        public IStashboxContainer Register()
        {
            this.containerContext.DecoratorRepository
                .AddDecorator(this.typeFrom, this.registrationContext.CreateServiceRegistration(isDecorator: true));
            this.containerContext.DelegateRepository.InvalidateDelegateCache();

            return this.containerContext.Container;
        }
    }
}
