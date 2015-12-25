using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class EnumerableResolver : Resolver
    {
        private delegate object ResolverDelegate(ResolutionInfo resolutionInfo);
        private readonly IServiceRegistration[] registrationCache;
        private ResolverDelegate resolverDelegate;
        private readonly TypeInformation enumerableType;

        internal EnumerableResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.enumerableType = new TypeInformation
            {
                Type = typeInfo.Type.GetEnumerableType(),
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            containerContext.RegistrationRepository.TryGetAllRegistrations(this.enumerableType,
                out registrationCache);

            this.GenerateEnumerableExpression(registrationCache);
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            return this.resolverDelegate(resolutionInfo);
        }

        public override Expression GetExpression(Expression resolutionInfoExpression)
        {
            var length = registrationCache.Length;
            var enumerableItems = new Expression[length];
            for (var i = 0; i < length; i++)
            {
                enumerableItems[i] = registrationCache[i].GetExpression(resolutionInfoExpression, this.enumerableType);
            }

            return Expression.NewArrayInit(this.enumerableType.Type, enumerableItems);
        }

        private void GenerateEnumerableExpression(IReadOnlyList<IServiceRegistration> registrationCache)
        {
            var resolutionInfoParameter = Expression.Parameter(typeof(ResolutionInfo), "resolutionInfo");

            var length = registrationCache.Count;
            var enumerableItems = new Expression[length];
            for (var i = 0; i < length; i++)
            {
                enumerableItems[i] = this.CreateSubscriptionExpression(registrationCache[i], resolutionInfoParameter, enumerableType.Type);
            }

            var arrayInit = Expression.NewArrayInit(enumerableType.Type, enumerableItems);
            this.resolverDelegate = Expression.Lambda<ResolverDelegate>(arrayInit, resolutionInfoParameter).Compile();
        }

        private Expression CreateSubscriptionExpression(IServiceRegistration registration, Expression resolutionInfoParameter, Type enumerableType)
        {
            var target = Expression.Constant(registration, typeof(IServiceRegistration));
            var evaluate = Expression.Call(target, "GetInstance", null, resolutionInfoParameter, Expression.Constant(this.enumerableType));
            var call = Expression.Convert(evaluate, enumerableType);
            return call;
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
