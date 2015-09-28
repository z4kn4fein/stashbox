using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.Registration
{
    public class ServiceRegistration : IServiceRegistration
    {
        private readonly ILifetime lifetimeManager;
        private readonly IObjectBuilder objectBuilder;
        private readonly IContainerContext containerContext;

        public RegistrationInfo RegistrationInfo { get; }

        public ServiceRegistration(ILifetime lifetimeManager, IObjectBuilder objectBuilder, IContainerContext containerContext, RegistrationInfo registrationInfo)
        {
            this.lifetimeManager = lifetimeManager;
            this.objectBuilder = objectBuilder;
            this.containerContext = containerContext;
            this.RegistrationInfo = registrationInfo;
        }

        public object GetInstance(ResolutionInfo resolutionInfo)
        {
            return this.lifetimeManager.GetInstance(this.objectBuilder, this.containerContext, resolutionInfo);
        }

        public void CleanUp()
        {
            this.objectBuilder.CleanUp();
            this.lifetimeManager.CleanUp();
        }
    }
}
