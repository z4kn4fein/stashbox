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

        private readonly ConstructorInfo lazyConstructor;

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

            this.registrationCache = containerContext.RegistrationRepository.GetRegistrationOrDefault(this.lazyArgumentInfo, true);
            if (this.registrationCache == null)
                throw new ResolutionFailedException(typeInfo.Type.FullName);

            var ctorParamType = typeof(Func<>).MakeGenericType(this.lazyArgumentInfo.Type);
            this.lazyConstructor = base.TypeInfo.Type.GetConstructor(ctorParamType);
        }
        
        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            var expr = Expression.Lambda(registrationCache.GetExpression(resolutionInfo, this.lazyArgumentInfo));
            return Expression.New(this.lazyConstructor, expr);
        }
    }
}
