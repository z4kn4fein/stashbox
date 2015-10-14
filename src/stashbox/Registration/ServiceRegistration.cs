using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;

namespace Stashbox.Registration
{
    public class ServiceRegistration : IServiceRegistration
    {
        private readonly ILifetime lifetimeManager;
        private readonly IObjectBuilder objectBuilder;
        private readonly IContainerContext containerContext;
        private readonly Type targetTypeCondition;
        private readonly Func<ResolutionInfo, bool> resolutionCondition;

        public ServiceRegistration(ILifetime lifetimeManager, IObjectBuilder objectBuilder, IContainerContext containerContext, Type targetTypeCondition = null, Func<ResolutionInfo, bool> resolutionCondition = null)
        {
            this.lifetimeManager = lifetimeManager;
            this.objectBuilder = objectBuilder;
            this.containerContext = containerContext;
            this.targetTypeCondition = targetTypeCondition;
            this.resolutionCondition = resolutionCondition;
        }

        public object GetInstance(ResolutionInfo resolutionInfo)
        {
            return this.lifetimeManager.GetInstance(this.objectBuilder, this.containerContext, resolutionInfo);
        }

        public bool IsUsable(ResolutionInfo resolutionInfo)
        {
            return (this.targetTypeCondition == null && this.resolutionCondition == null) ||
                   (this.targetTypeCondition != null && this.targetTypeCondition == resolutionInfo.ParentType.Type) ||
                   (this.resolutionCondition != null && this.resolutionCondition(resolutionInfo));
        }

        public bool IsUsableAtPlanBuilding(Type targetConditionType)
        {
            return (this.targetTypeCondition == null && this.resolutionCondition == null) ||
                   (this.targetTypeCondition != null && this.targetTypeCondition == targetConditionType && this.resolutionCondition == null);
        }

        public void CleanUp()
        {
            this.objectBuilder.CleanUp();
            this.lifetimeManager.CleanUp();
        }
    }
}
