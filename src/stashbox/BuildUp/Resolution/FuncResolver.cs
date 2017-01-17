using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Resolution
{
    internal class FuncResolver : Resolver
    {
        private readonly IServiceRegistration registrationCache;
        private delegate object ResolverDelegate(ResolutionInfo resolutionInfo);
        private readonly ResolverDelegate resolverDelegate;
        private readonly TypeInformation funcArgumentInfo;
        private readonly MethodInfo resolverMethodInfo;

        public FuncResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.funcArgumentInfo = new TypeInformation
            {
                Type = typeInfo.Type.GenericTypeArguments[0],
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            containerContext.RegistrationRepository.TryGetRegistrationWithConditions(this.funcArgumentInfo, out this.registrationCache);

            var genericLazyResolverMethod = this.GetType().GetTypeInfo().GetDeclaredMethod("ResolveFunc");
            this.resolverMethodInfo = genericLazyResolverMethod.MakeGenericMethod(typeInfo.Type.GenericTypeArguments[0]);
            this.resolverDelegate = (ResolverDelegate)resolverMethodInfo.CreateDelegate(typeof(ResolverDelegate), this);
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            return this.resolverDelegate(resolutionInfo);
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            return Expression.Call(Expression.Constant(this), this.resolverMethodInfo, resolutionInfoExpression);
        }

        private Func<T> ResolveFunc<T>(ResolutionInfo resolutionInfo) where T : class
        {
            return () => (T)registrationCache.GetInstance(resolutionInfo, this.funcArgumentInfo);
        }
    }
}
