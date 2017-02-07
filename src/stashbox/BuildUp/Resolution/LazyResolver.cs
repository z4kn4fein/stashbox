using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox.BuildUp.Resolution
{
    internal class LazyResolver : Resolver
    {
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

            var ctorParamType = typeof(Func<>).MakeGenericType(this.lazyArgumentInfo.Type);
            this.lazyConstructor = base.TypeInfo.Type.GetConstructor(ctorParamType);
        }

        public override Type WrappedType => this.lazyArgumentInfo.Type;

        public override bool CanUseForEnumerableArgumentResolution => true;

        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            var registration = this.BuilderContext.RegistrationRepository.GetRegistrationOrDefault(this.lazyArgumentInfo, true);
            if (registration != null)
                return Expression.New(this.lazyConstructor, Expression.Lambda(registration.GetExpression(resolutionInfo, this.lazyArgumentInfo)));

            Resolver resolver;
            if (!this.BuilderContext.ResolverSelector.TryChooseResolver(this.BuilderContext, this.lazyArgumentInfo, out resolver))
                throw new ResolutionFailedException(base.TypeInfo.Type.FullName);

            return Expression.New(this.lazyConstructor, Expression.Lambda(resolver.GetExpression(resolutionInfo)));
        }

        public override Expression[] GetEnumerableArgumentExpressions(ResolutionInfo resolutionInfo)
        {
            var registrations = this.BuilderContext.RegistrationRepository.GetRegistrationsOrDefault(this.lazyArgumentInfo);
            if (registrations != null)
            {
                var serviceRegistrations = base.BuilderContext.ContainerConfiguration.EnumerableOrderRule(registrations).ToArray();
                var lenght = serviceRegistrations.Length;
                var expressions = new Expression[lenght];
                for (var i = 0; i < lenght; i++)
                    expressions[i] = Expression.New(this.lazyConstructor, Expression.Lambda(serviceRegistrations[i].GetExpression(resolutionInfo, this.lazyArgumentInfo)));

                return expressions;
            }

            Resolver resolver;
            if (this.BuilderContext.ResolverSelector.TryChooseResolver(this.BuilderContext, this.lazyArgumentInfo, out resolver) && resolver.CanUseForEnumerableArgumentResolution)
            {
                var exprs = resolver.GetEnumerableArgumentExpressions(resolutionInfo);
                var length = exprs.Length;
                var expressions = new Expression[length];
                for (var i = 0; i < length; i++)
                    expressions[i] = Expression.New(this.lazyConstructor, Expression.Lambda(exprs[i]));

                return expressions;
            }

            return null;
        }
    }
}
