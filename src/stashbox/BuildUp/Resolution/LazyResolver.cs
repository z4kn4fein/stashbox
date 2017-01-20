using Stashbox.Entity;
using Stashbox.Exceptions;
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
        private readonly MethodInfo resolverMethodInfo;

        public LazyResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.lazyArgumentInfo = new TypeInformation
            {
                Type = typeInfo.Type.GenericTypeArguments[0],
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            if (!containerContext.RegistrationRepository.TryGetRegistrationWithConditions(this.lazyArgumentInfo, out this.registrationCache))
                throw new ResolutionFailedException(typeInfo.Type.FullName);

            var genericLazyResolverMethod = this.GetType().GetTypeInfo().GetDeclaredMethod("ResolveLazy");
            this.resolverMethodInfo = genericLazyResolverMethod.MakeGenericMethod(typeInfo.Type.GenericTypeArguments[0]);
            this.resolverDelegate = (ResolverDelegate)this.resolverMethodInfo.CreateDelegate(typeof(ResolverDelegate), this);
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            return this.resolverDelegate(resolutionInfo);
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            return Expression.Call(Expression.Constant(this), resolverMethodInfo, resolutionInfoExpression);
        }

        private Lazy<T> ResolveLazy<T>(ResolutionInfo resolutionInfo) where T : class
        {
            return new Lazy<T>(() => (T)registrationCache.GetInstance(resolutionInfo, this.lazyArgumentInfo));
        }
    }
}
