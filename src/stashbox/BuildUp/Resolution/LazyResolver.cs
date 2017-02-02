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
        private readonly TypeInformation lazyArgumentInfo;
        private readonly Expression expression;

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

            this.registrationCache = containerContext.RegistrationRepository.GetRegistrationOrDefault(this.lazyArgumentInfo);
            if (this.registrationCache == null)
                throw new ResolutionFailedException(typeInfo.Type.FullName);

            var genericLazyResolverMethod = this.GetType().GetTypeInfo().GetDeclaredMethod("ResolveLazy");
            var method = genericLazyResolverMethod.MakeGenericMethod(typeInfo.Type.GenericTypeArguments[0]);
            this.expression = Expression.Call(Expression.Constant(this), method);
        }
        
        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            return this.expression;
        }

        private Lazy<T> ResolveLazy<T>(ResolutionInfo resolutionInfo) where T : class
        {
            return new Lazy<T>(() => (T)registrationCache.GetInstance(resolutionInfo, this.lazyArgumentInfo));
        }
    }
}
