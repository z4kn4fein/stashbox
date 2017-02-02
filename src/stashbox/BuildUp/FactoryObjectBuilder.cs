using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp
{
    internal class FactoryObjectBuilder : IObjectBuilder
    {
        private readonly Func<IStashboxContainer, object> containerFactory;
        private readonly Func<object> singleFactory;
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IObjectExtender objectExtender;
        private readonly MethodInfo buildMethodInfo;
        private readonly IContainerContext containerContext;

        private FactoryObjectBuilder(IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
            this.objectExtender = objectExtender;
            this.buildMethodInfo = this.GetType().GetTypeInfo().GetDeclaredMethod("BuildInstance");
        }

        public FactoryObjectBuilder(Func<IStashboxContainer, object> containerFactory, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            : this(containerContext, containerExtensionManager, objectExtender)
        {
            this.containerFactory = containerFactory;
        }

        public FactoryObjectBuilder(Func<object> factory, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            : this(containerContext, containerExtensionManager, objectExtender)
        {
            this.singleFactory = factory;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            object instance = null;

            if (this.containerFactory != null)
                instance = this.containerFactory.Invoke(this.containerContext.Container);

            if (this.singleFactory != null)
                instance = this.singleFactory.Invoke();

            var builtInstance = this.objectExtender.FillResolutionMembers(instance, this.containerContext, resolutionInfo);
            builtInstance = this.objectExtender.FillResolutionMembers(builtInstance, this.containerContext, resolutionInfo);
            return this.containerExtensionManager.ExecutePostBuildExtensions(builtInstance, this.containerContext, resolutionInfo, resolveType);
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            var callExpression = Expression.Call(Expression.Constant(this), this.buildMethodInfo, Expression.Constant(resolveType));
            return Expression.Convert(callExpression, resolveType.Type);
        }

        public void ServiceUpdated(RegistrationInfo registrationInfo)
        {
            this.objectExtender.ServiceUpdated(registrationInfo);
        }

        public void CleanUp()
        { }
    }
}