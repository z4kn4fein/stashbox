using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a singleton lifetime manager.
    /// </summary>
    public class SingletonLifetime : FactoryLifetimeDescriptor
    {
        /// <inheritdoc />
        protected override int LifeSpan { get; } = 20;

        /// <inheritdoc />
        protected override Expression ApplyLifetime(Func<IResolutionScope, object> factory,
            ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            var rootScope = resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.ReBuildSingletonsInChildContainerEnabled
                ? resolutionContext.RequestInitiatorContainerContext.RootScope
                : resolutionContext.CurrentContainerContext.RootScope;

            // do not build singletons during validation, we just have to ensure the expression tree is valid
            if (resolutionContext.IsValidationRequest)
                return rootScope.AsConstant().CallMethod(Constants.GetOrAddScopedObjectMethod,
                    serviceRegistration.RegistrationId.AsConstant(),
                    factory.AsConstant(),
                    resolveType.AsConstant(),
                    false.AsConstant()).ConvertTo(resolveType);

            return rootScope.GetOrAddScopedObject(serviceRegistration.RegistrationId, factory, resolveType)
                    .AsConstant();
        }
    }
}
