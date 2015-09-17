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
        private ResolverDelegate resolverDelegate;

        internal LazyResolver(IBuilderContext builderContext, TypeInformation typeInfo)
            : base(builderContext, typeInfo)
        {
            builderContext.RegistrationRepository.TryGetRegistration(typeInfo.Type.GenericTypeArguments[0],
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
                    Type = resolutionInfo.ResolveType.Type.GenericTypeArguments[0],
                    DependencyName = resolutionInfo.ResolveType.DependencyName
                },
                FactoryParams = resolutionInfo.FactoryParams,
                OverrideManager = resolutionInfo.OverrideManager
            }));
        }
    }

    internal class LazyResolverFactory : ResolverFactory
    {
        public override Resolver Create(IBuilderContext builderContext, TypeInformation typeInfo)
        {
            return new LazyResolver(builderContext, typeInfo);
        }
    }
}
