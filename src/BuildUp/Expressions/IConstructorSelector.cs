using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Registration;
using Stashbox.Resolution;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IConstructorSelector
    {
        ResolutionConstructor CreateResolutionConstructor(IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            ConstructorInformation constructor);

        ResolutionConstructor SelectConstructor(IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            ConstructorInformation[] constructors);
    }
}
