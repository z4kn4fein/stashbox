using Stashbox.BuildUp.Expressions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;

namespace Stashbox.Registration
{
    internal class ScopedRegistrationContext : RegistrationContextBase
    {
        private readonly IContainerExtensionManager containerExtensionManager;

        public ScopedRegistrationContext(Type typeFrom, Type typeTo, IContainerContext containerContext, 
            IExpressionBuilder expressionBuilder, IContainerExtensionManager containerExtensionManager)
            : base(typeFrom, typeTo, containerContext, expressionBuilder)
        {
            this.containerExtensionManager = containerExtensionManager;
        }

        public void InitFromScope(RegistrationContextData scopeData)
        {
            base.RegistrationContextData = scopeData;
            base.PrepareRegistration(this.containerExtensionManager);
        }
    }
}
