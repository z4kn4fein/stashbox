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

        public IDecoratorRegistrationContext WithFactory(Func<object> singleFactory)
        {
            this.registrationContext.WithFactory(singleFactory);
            return this;
        }

        public IDecoratorRegistrationContext WithFactory(Func<IStashboxContainer, object> containerFactory)
        {
            this.registrationContext.WithFactory(containerFactory);
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

        public IDecoratorRegistrationContext WithInstance(object instance)
        {
            this.registrationContext.WithInstance(instance);
            return this;
        }

        public IStashboxContainer Register()
        {
            this.registrationContext.ContainerContext.DecoratorRepository
                .AddDecorator(this.registrationContext.TypeFrom, this.registrationContext.CreateServiceRegistration());

            return this.registrationContext.ContainerContext.Container;
        }
    }
}
