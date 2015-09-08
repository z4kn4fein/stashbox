using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.Registration
{
    public class ServiceRegistration : IServiceRegistration
    {
        private readonly ILifetime lifetimeManager;
        private readonly IObjectBuilder objectBuilder;
        private readonly IBuilderContext builderContext;

        public ServiceRegistration(ILifetime lifetimeManager, IObjectBuilder objectBuilder, IBuilderContext builderContext)
        {
            this.lifetimeManager = lifetimeManager;
            this.objectBuilder = objectBuilder;
            this.builderContext = builderContext;
        }

        public object GetInstance(ResolutionInfo resolutionInfo)
        {
            return this.lifetimeManager.GetInstance(this.objectBuilder, this.builderContext, resolutionInfo);
        }

        public void CleanUp()
        {
            this.objectBuilder.CleanUp();
            this.lifetimeManager.CleanUp();
        }
    }
}
