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
        private readonly RegistrationContext registrationContext;
        private readonly IServiceRegistrator serviceRegistrator;

        public DecoratorRegistrationContext(RegistrationContext registrationContext, IServiceRegistrator serviceRegistrator)
        {
            this.registrationContext = registrationContext;
            this.serviceRegistrator = serviceRegistrator;
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

        public IStashboxContainer Register() => this.serviceRegistrator.Register(this.registrationContext, true);
    }
}
