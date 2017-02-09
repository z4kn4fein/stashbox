using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq.Expressions;
using Stashbox.BuildUp.Expressions;

namespace Stashbox.BuildUp
{
    internal class FactoryObjectBuilder : IObjectBuilder
    {
        private readonly Func<IStashboxContainer, object> containerFactory;
        private readonly Func<object> singleFactory;
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IContainerContext containerContext;

        private FactoryObjectBuilder(IContainerContext containerContext, IContainerExtensionManager containerExtensionManager,
            IMetaInfoProvider metaInfoProvider, InjectionParameter[] injectionParameters = null)
        {
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.injectionParameters = injectionParameters;
        }

        public FactoryObjectBuilder(Func<IStashboxContainer, object> containerFactory, IContainerContext containerContext,
            IContainerExtensionManager containerExtensionManager, IMetaInfoProvider metaInfoProvider, InjectionParameter[] injectionParameters = null)
            : this(containerContext, containerExtensionManager, metaInfoProvider, injectionParameters)
        {
            this.containerFactory = containerFactory;
        }

        public FactoryObjectBuilder(Func<object> factory, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager,
            IMetaInfoProvider metaInfoProvider, InjectionParameter[] injectionParameters = null)
            : this(containerContext, containerExtensionManager, metaInfoProvider, injectionParameters)
        {
            this.singleFactory = factory;
        }
        
        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            Expression<Func<object>> lamdba;
            if (this.containerFactory != null)
                lamdba = () => this.containerFactory(this.containerContext.Container);
            else
                lamdba = () => this.singleFactory();

            var expr = Expression.Invoke(lamdba);
            
            return ExpressionDelegateFactory.CreateFillExpression(this.containerExtensionManager, this.containerContext,
                   expr, resolutionInfo, resolveType, this.injectionParameters,
                   this.metaInfoProvider.GetResolutionMembers(resolutionInfo, this.injectionParameters), 
                   this.metaInfoProvider.GetResolutionMethods(resolutionInfo, this.injectionParameters));
        }
        
        public void CleanUp()
        { }
    }
}