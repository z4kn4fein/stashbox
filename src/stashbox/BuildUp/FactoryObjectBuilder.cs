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
        private readonly IExpressionBuilder expressionBuilder;

        private FactoryObjectBuilder(IContainerContext containerContext, IContainerExtensionManager containerExtensionManager,
            IMetaInfoProvider metaInfoProvider, IExpressionBuilder expressionBuilder, InjectionParameter[] injectionParameters = null)
        {
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.injectionParameters = injectionParameters;
            this.expressionBuilder = expressionBuilder;
        }

        public FactoryObjectBuilder(Func<IStashboxContainer, object> containerFactory, IContainerContext containerContext,
            IContainerExtensionManager containerExtensionManager, IMetaInfoProvider metaInfoProvider, IExpressionBuilder expressionBuilder, InjectionParameter[] injectionParameters = null)
            : this(containerContext, containerExtensionManager, metaInfoProvider, expressionBuilder, injectionParameters)
        {
            this.containerFactory = containerFactory;
        }

        public FactoryObjectBuilder(Func<object> factory, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager,
            IMetaInfoProvider metaInfoProvider, IExpressionBuilder expressionBuilder, InjectionParameter[] injectionParameters = null)
            : this(containerContext, containerExtensionManager, metaInfoProvider, expressionBuilder, injectionParameters)
        {
            this.singleFactory = factory;
        }
        
        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            Expression<Func<object>> lambda;
            if (this.containerFactory != null)
                lambda = () => this.containerFactory(this.containerContext.Container);
            else
                lambda = () => this.singleFactory();

            var expr = Expression.Invoke(lambda);
            
            return this.expressionBuilder.CreateFillExpression(this.containerExtensionManager, this.containerContext,
                   expr, resolutionInfo, resolveType, this.injectionParameters,
                   this.metaInfoProvider.GetResolutionMembers(resolutionInfo, this.injectionParameters), 
                   this.metaInfoProvider.GetResolutionMethods(resolutionInfo, this.injectionParameters));
        }

        public bool HandlesObjectDisposal => false;

        public void CleanUp()
        { }
    }
}