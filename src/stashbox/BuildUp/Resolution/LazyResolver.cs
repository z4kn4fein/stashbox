using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Reflection;

namespace Stashbox.BuildUp.Resolution
{
    internal class LazyResolver : Resolver
    {
        private readonly IServiceRegistration registrationCache;
        private delegate object ResolverDelegate(ResolutionInfo resolutionInfo);
        private readonly ResolverDelegate resolverDelegate;

        internal LazyResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            containerContext.RegistrationRepository.TryGetRegistration(typeInfo.Type.GenericTypeArguments[0],
                out this.registrationCache, base.TypeInfo.DependencyName);

            var genericLazyResolverMethod = this.GetType().GetTypeInfo().GetDeclaredMethod("ResolveLazy");
            var resolver = genericLazyResolverMethod.MakeGenericMethod(typeInfo.Type.GenericTypeArguments[0]);
            resolverDelegate = (ResolverDelegate)resolver.CreateDelegate(typeof(ResolverDelegate), this);
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            return this.resolverDelegate(resolutionInfo);
        }

        private object ResolveLazy<T>(ResolutionInfo resolutionInfo) where T : class
        {
            return new Lazy<T>(() => (T)registrationCache.GetInstance(new ResolutionInfo
            {
                ResolveType = new TypeInformation
                {
                    Type = base.TypeInfo.Type.GenericTypeArguments[0],
                    DependencyName = base.TypeInfo.DependencyName
                },
                FactoryParams = resolutionInfo.FactoryParams,
                OverrideManager = resolutionInfo.OverrideManager,
                ParentType = resolutionInfo.ResolveType
            }));
        }
    }

    internal class LazyResolverFactory : ResolverFactory
    {
        public override Resolver Create(IContainerContext containerContext, TypeInformation typeInfo)
        {
            return new LazyResolver(containerContext, typeInfo);
        }
    }
}
