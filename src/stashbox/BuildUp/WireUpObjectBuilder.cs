using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp
{
    internal class WireUpObjectBuilder : IObjectBuilder
    {
        private object instance;
        private readonly object syncObject = new object();
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IObjectExtender objectExtender;
        private readonly IContainerContext containerContext;
        private readonly MethodInfo buildMethodInfo;
        private bool instanceBuilt;

        public WireUpObjectBuilder(object instance, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            this.instance = instance;
            this.containerExtensionManager = containerExtensionManager;
            this.objectExtender = objectExtender;
            this.containerContext = containerContext;
            this.buildMethodInfo = this.GetType().GetTypeInfo().GetDeclaredMethod("BuildInstance");
        }

        public object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (this.instanceBuilt) return this.instance;
            lock (this.syncObject)
            {
                if (this.instanceBuilt) return this.instance;
                this.instanceBuilt = true;
                this.instance = this.objectExtender.FillResolutionMembers(this.instance, this.containerContext, resolutionInfo);
                this.instance = this.objectExtender.FillResolutionMethods(this.instance, this.containerContext, resolutionInfo);
                this.instance = this.containerExtensionManager.ExecutePostBuildExtensions(this.instance, this.containerContext, resolutionInfo, resolveType);
            }

            return this.instance;
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
        {
            if (this.instance == null) return;
            lock (this.syncObject)
            {
                if (this.instance == null) return;
                var disposable = this.instance as IDisposable;
                disposable?.Dispose();
                this.instance = null;
            }
        }
    }
}