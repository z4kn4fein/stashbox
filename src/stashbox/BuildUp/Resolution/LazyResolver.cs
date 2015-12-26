using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Resolution
{
    internal class LazyResolver : Resolver
    {
        private readonly IServiceRegistration registrationCache;
        private delegate object ResolverDelegate(ResolutionInfo resolutionInfo);
        private readonly ResolverDelegate resolverDelegate;
        private readonly TypeInformation lazyArgumentInfo;

        internal LazyResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.lazyArgumentInfo = new TypeInformation
            {
                Type = typeInfo.Type.GenericTypeArguments[0],
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            containerContext.RegistrationRepository.TryGetRegistrationWithConditions(this.lazyArgumentInfo, out this.registrationCache);

            var genericLazyResolverMethod = this.GetType().GetTypeInfo().GetDeclaredMethod("ResolveLazy");
            var resolver = genericLazyResolverMethod.MakeGenericMethod(typeInfo.Type.GenericTypeArguments[0]);
            resolverDelegate = (ResolverDelegate)resolver.CreateDelegate(typeof(ResolverDelegate), this);
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            return this.resolverDelegate(resolutionInfo);
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            return Expression.Call(Expression.Constant(this), "ResolveLazy", new[] { this.lazyArgumentInfo.Type }, resolutionInfoExpression);
        }

        private Lazy<T> ResolveLazy<T>(ResolutionInfo resolutionInfo) where T : class
        {
            return new Lazy<T>(() => (T)registrationCache.GetInstance(resolutionInfo, this.lazyArgumentInfo));
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
