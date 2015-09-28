using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.Lifetime
{
    public class TransientLifetime : ILifetime
    {
        public object GetInstance(IObjectBuilder objectBuilder, IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            Shield.EnsureNotNull(objectBuilder);
            return objectBuilder.BuildInstance(containerContext, resolutionInfo);
        }

        public void CleanUp()
        {
        }
    }
}
