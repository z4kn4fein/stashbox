﻿using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class WireUpObjectBuilder : IObjectBuilder
    {
        private readonly object instance;
        private readonly Type instanceType;
        private volatile object builtInstance;
        private readonly object syncObject = new object();
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IObjectExtender objectExtender;
        private readonly IContainerContext containerContext;

        public WireUpObjectBuilder(object instance, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            this.instance = instance;
            this.instanceType = instance.GetType();
            this.containerExtensionManager = containerExtensionManager;
            this.objectExtender = objectExtender;
            this.containerContext = containerContext;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (this.builtInstance != null) return this.builtInstance;
            lock (this.syncObject)
            {
                if (this.builtInstance != null) return this.builtInstance;
                this.builtInstance = this.objectExtender.FillResolutionMembers(this.instance, this.containerContext, resolutionInfo);
                this.builtInstance = this.objectExtender.FillResolutionMethods(this.builtInstance, this.containerContext, resolutionInfo);
                this.builtInstance = this.containerExtensionManager.ExecutePostBuildExtensions(this.builtInstance, this.instanceType, this.containerContext, resolutionInfo, resolveType);
            }

            return this.builtInstance;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            var callExpression = Expression.Call(Expression.Constant(this), "BuildInstance", null, resolutionInfoExpression, Expression.Constant(resolveType));
            return Expression.Convert(callExpression, resolveType.Type);
        }

        public void ServiceUpdated(RegistrationInfo registrationInfo)
        {
            this.objectExtender.ServiceUpdated(registrationInfo);
        }

        public void CleanUp()
        {
            if (this.builtInstance == null) return;
            lock (this.syncObject)
            {
                if (this.builtInstance == null) return;
                var disposable = this.builtInstance as IDisposable;
                disposable?.Dispose();
                this.builtInstance = null;
            }
        }
    }
}