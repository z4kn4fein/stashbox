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

        public DecoratorRegistrationContext(IRegistrationContext registrationContext)
        {
            this.registrationContext = registrationContext;
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

        public IStashboxContainer Register()
        {
            this.registrationContext.ContainerContext.DecoratorRepository
                .AddDecorator(this.registrationContext.TypeFrom, this.registrationContext.CreateServiceRegistration(isDecorator: true));
            this.registrationContext.ContainerContext.DelegateRepository.InvalidateDelegateCache();

            return this.registrationContext.ContainerContext.Container;
        }
    }
}
