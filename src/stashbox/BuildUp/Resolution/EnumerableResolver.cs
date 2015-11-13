using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class EnumerableResolver : Resolver
    {
        private delegate object ResolverDelegate(ResolutionInfo resolutionInfo);
        private ResolverDelegate resolverDelegate;

        internal EnumerableResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            IServiceRegistration[] registrationCache;
            containerContext.RegistrationRepository.TryGetAllRegistrations(new TypeInformation { Type = typeInfo.Type.GetEnumerableType() },
                out registrationCache);

            this.GenerateEnumerableExpression(registrationCache);
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            return this.resolverDelegate(resolutionInfo);
        }

        private void GenerateEnumerableExpression(IServiceRegistration[] registrationCache)
        {
            var resolutionInfoParameter = Expression.Parameter(typeof(ResolutionInfo), "resolutionInfo");

            var length = registrationCache.Length;
            var enumerableItems = new Expression[length];
            var enumerableType = base.TypeInfo.Type.GetEnumerableType();
            for (int i = 0; i < length; i++)
            {
                enumerableItems[i] = this.CreateSubscriptionExpression(registrationCache[i], resolutionInfoParameter, enumerableType);
            }

            var arrayInit = Expression.NewArrayInit(enumerableType, enumerableItems);
            this.resolverDelegate = Expression.Lambda<ResolverDelegate>(arrayInit, new ParameterExpression[] { resolutionInfoParameter }).Compile();
        }

        private Expression CreateSubscriptionExpression(IServiceRegistration registration, ParameterExpression resolutionInfoParameter, Type enumerableType)
        {
            var target = Expression.Constant(registration, typeof(IServiceRegistration));
            var evaluate = Expression.Call(target, "GetInstance", null, new Expression[] { resolutionInfoParameter });
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
