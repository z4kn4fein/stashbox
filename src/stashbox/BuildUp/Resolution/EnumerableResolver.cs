using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Reflection;

namespace Stashbox.BuildUp.Resolution
{
    internal class EnumerableResolver : Resolver
    {
        private readonly IServiceRegistration[] registrationCache;
        private delegate object ResolverDelegate(ResolutionInfo resolutionInfo);
        private readonly ResolverDelegate resolverDelegate;

        internal EnumerableResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            containerContext.RegistrationRepository.TryGetAllRegistrations(new TypeInformation { Type = typeInfo.Type.GetEnumerableType() },
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
            var upper = registrationCache.Length;
            var result = new T[upper];
            for (var i = 0; i < upper; i++)
            {
                result[i] = (T)registrationCache[i].GetInstance(new ResolutionInfo
                {
                    ResolveType = base.TypeInfo,
                    FactoryParams = resolutionInfo.FactoryParams,
                    OverrideManager = resolutionInfo.OverrideManager
                });
            }
            return result;
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
