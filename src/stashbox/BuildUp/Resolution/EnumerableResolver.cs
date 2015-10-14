using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.BuildUp.Resolution
{
    internal class EnumerableResolver : Resolver
    {
        private readonly IEnumerable<IServiceRegistration> registrationCache;
        private delegate object ResolverDelegate(ResolutionInfo resolutionInfo);
        private readonly ResolverDelegate resolverDelegate;

        internal EnumerableResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            containerContext.RegistrationRepository.TryGetAllRegistrations(typeInfo.Type.GetEnumerableType(),
                out this.registrationCache);

            var genericLazyResolverMethod = this.GetType().GetTypeInfo().GetDeclaredMethod("ResolveArray");
            var resolver = genericLazyResolverMethod.MakeGenericMethod(typeInfo.Type.GetEnumerableType());
            resolverDelegate = (ResolverDelegate)resolver.CreateDelegate(typeof(ResolverDelegate), this);
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            return this.resolverDelegate(resolutionInfo);
        }

        private object ResolveArray<T>(ResolutionInfo resolutionInfo) where T : class
        {
            return registrationCache.Select(registration => (T)registration.GetInstance(new ResolutionInfo
            {
                ResolveType = base.TypeInfo,
                FactoryParams = resolutionInfo.FactoryParams,
                OverrideManager = resolutionInfo.OverrideManager,
                ParentType = resolutionInfo.ResolveType
            })).ToArray();
        }
    }

    internal class EnumerableResolverFactory : ResolverFactory
    {
        public override Resolver Create(IContainerContext containerContext, TypeInformation typeInfo)
        {
            return new EnumerableResolver(containerContext, typeInfo);
        }
    }
}
