using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class EnumerableResolver : Resolver
    {
        private delegate object ResolverDelegate(ResolutionInfo resolutionInfo);
        private readonly IServiceRegistration[] registrationCache;
        private ResolverDelegate resolverDelegate;
        private readonly TypeInformation enumerableType;
        private readonly object syncObject = new object();

        public EnumerableResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.enumerableType = new TypeInformation
            {
                Type = typeInfo.Type.GetEnumerableType(),
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            containerContext.RegistrationRepository.TryGetTypedRepositoryRegistrations(this.enumerableType,
                out registrationCache);
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            if (this.resolverDelegate != null) return this.resolverDelegate(resolutionInfo);
            lock (this.syncObject)
            {
                if (this.resolverDelegate != null) return this.resolverDelegate(resolutionInfo);
                var parameter = Expression.Parameter(typeof(ResolutionInfo));
                this.resolverDelegate = Expression.Lambda<ResolverDelegate>(this.GetExpression(resolutionInfo, parameter), parameter).Compile();
            }

            return this.resolverDelegate(resolutionInfo);
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            if (registrationCache == null)
                return Expression.NewArrayInit(this.enumerableType.Type);

            var length = registrationCache.Length;
            var enumerableItems = new Expression[length];
            for (var i = 0; i < length; i++)
            {
                enumerableItems[i] = registrationCache[i].GetExpression(resolutionInfo, resolutionInfoExpression, this.enumerableType);
            }

            return Expression.NewArrayInit(this.enumerableType.Type, enumerableItems);
        }
    }
}
