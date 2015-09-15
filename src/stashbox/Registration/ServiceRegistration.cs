using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.Registration
{
    public class ServiceRegistration : IServiceRegistration
    {
        private readonly ILifetime lifetimeManager;
        private readonly IObjectBuilder objectBuilder;
        private readonly IBuilderContext builderContext;

        public RegistrationInfo RegistrationInfo { get; }

        public ServiceRegistration(ILifetime lifetimeManager, IObjectBuilder objectBuilder, IBuilderContext builderContext, RegistrationInfo registrationInfo)
        {
            this.lifetimeManager = lifetimeManager;
            this.objectBuilder = objectBuilder;
            this.builderContext = builderContext;
            this.RegistrationInfo = registrationInfo;
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
