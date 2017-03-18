using Stashbox.BuildUp.Expressions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Registration
{
    internal class ScopedRegistrationContext : RegistrationContextBase
    {
        public ScopedRegistrationContext(Type typeFrom, Type typeTo, IContainerContext containerContext, 
            IExpressionBuilder expressionBuilder, IContainerExtensionManager containerExtensionManager)
            : base(typeFrom, typeTo, containerContext, expressionBuilder, containerExtensionManager)
        { }

        public IServiceRegistration InitFromScope(RegistrationContextData scopeData)
        {
            base.RegistrationContextData = scopeData;
            return base.CompleteRegistration();
        }

        public IServiceRegistration CreateRegistration(RegistrationContextData scopeData, bool isDecorator = false)
        {
            base.RegistrationContextData = scopeData;
            return base.CreateServiceRegistration(isDecorator);
        }
    }
}
