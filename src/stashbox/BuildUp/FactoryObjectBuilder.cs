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
        private readonly Func<object, object> oneParamsFactory;
        private readonly Func<object, object, object> twoParamsFactory;
        private readonly Func<object, object, object, object> threeParamsFactory;
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IObjectExtender objectExtender;
        private readonly MethodInfo buildMethodInfo;

        private FactoryObjectBuilder(IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            this.containerExtensionManager = containerExtensionManager;
            this.objectExtender = objectExtender;
            this.buildMethodInfo = this.GetType().GetTypeInfo().GetDeclaredMethod("BuildInstance");
        }

        public FactoryObjectBuilder(Func<IStashboxContainer, object> containerFactory, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            : this(containerExtensionManager, objectExtender)
        {
            this.containerFactory = containerFactory;
        }

        public FactoryObjectBuilder(Func<object> factory, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            : this(containerExtensionManager, objectExtender)
        {
            this.singleFactory = factory;
        }

        public FactoryObjectBuilder(Func<object, object> oneParamsFactory, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            : this(containerExtensionManager, objectExtender)
        {
            this.oneParamsFactory = oneParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object> twoParamsFactory,IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            : this(containerExtensionManager, objectExtender)
        {
            this.twoParamsFactory = twoParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object, object> threeParamsFactory, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            : this(containerExtensionManager, objectExtender)
        {
            this.threeParamsFactory = threeParamsFactory;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            object instance = null;

            if (this.containerFactory != null)
                instance = this.containerFactory.Invoke(resolutionInfo.RequestScope);

            if (this.singleFactory != null)
                instance = this.singleFactory.Invoke();

            if (this.oneParamsFactory != null)
                instance = this.oneParamsFactory.Invoke(resolutionInfo.FactoryParams.ElementAt(0));

            if (this.twoParamsFactory != null)
                instance = this.twoParamsFactory.Invoke(resolutionInfo.FactoryParams.ElementAt(0), resolutionInfo.FactoryParams.ElementAt(1));

            if (this.threeParamsFactory != null)
                instance = this.threeParamsFactory.Invoke(
                    resolutionInfo.FactoryParams.ElementAt(0),
                    resolutionInfo.FactoryParams.ElementAt(1),
                    resolutionInfo.FactoryParams.ElementAt(2));

            var builtInstance = this.objectExtender.FillResolutionMembers(instance, resolutionInfo.RequestScope.ContainerContext, resolutionInfo);
            builtInstance = this.objectExtender.FillResolutionMembers(builtInstance, resolutionInfo.RequestScope.ContainerContext, resolutionInfo);
            return this.containerExtensionManager.ExecutePostBuildExtensions(builtInstance, resolutionInfo.RequestScope.ContainerContext, resolutionInfo, resolveType);
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            var callExpression = Expression.Call(Expression.Constant(this), this.buildMethodInfo, resolutionInfoExpression, Expression.Constant(resolveType));
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