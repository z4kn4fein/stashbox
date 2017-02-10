using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.Exceptions;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox.BuildUp.Resolution
{
    internal class FuncResolver : Resolver
    {
        public static ISet<Type> SupportedTypes = new HashSet<Type>
        {
            typeof(Func<>),
            typeof(Func<,>),
            typeof(Func<,,>),
            typeof(Func<,,,>)
        };

        private readonly TypeInformation funcArgumentInfo;

        public FuncResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.funcArgumentInfo = new TypeInformation
            {
                Type = typeInfo.Type.GenericTypeArguments.Last(),
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };
        }

        public override Type WrappedType => this.funcArgumentInfo.Type;

        public override bool CanUseForEnumerableArgumentResolution => true;

        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            this.PrepareExtraParameters(resolutionInfo);
            var registration = this.BuilderContext.RegistrationRepository.GetRegistrationOrDefault(this.funcArgumentInfo, true);

            if (registration != null)
                return Expression.Lambda(registration.GetExpression(resolutionInfo, this.funcArgumentInfo), resolutionInfo.ParameterExpressions);

            Resolver resolver;
            if (!this.BuilderContext.ResolverSelector.TryChooseResolver(this.BuilderContext, this.funcArgumentInfo, out resolver))
                throw new ResolutionFailedException(base.TypeInfo.Type.FullName);

            return Expression.Lambda(resolver.GetExpression(resolutionInfo), resolutionInfo.ParameterExpressions);
        }

        public override Expression[] GetEnumerableArgumentExpressions(ResolutionInfo resolutionInfo)
        {
            this.PrepareExtraParameters(resolutionInfo);
            var registrations = this.BuilderContext.RegistrationRepository.GetRegistrationsOrDefault(this.funcArgumentInfo);
            if (registrations != null)
            {
                var serviceRegistrations = base.BuilderContext.ContainerConfigurator.ContainerConfiguration.EnumerableOrderRule(registrations).ToArray();
                var length = serviceRegistrations.Length;
                var expressions = new Expression[length];
                for (var i = 0; i < length; i++)
                    expressions[i] = Expression.Lambda(serviceRegistrations[i].GetExpression(resolutionInfo, this.funcArgumentInfo), resolutionInfo.ParameterExpressions);

                return expressions;
            }

            Resolver resolver;
            if (this.BuilderContext.ResolverSelector.TryChooseResolver(this.BuilderContext, this.funcArgumentInfo, out resolver) && resolver.CanUseForEnumerableArgumentResolution)
            {
                var exprs = resolver.GetEnumerableArgumentExpressions(resolutionInfo);
                var length = exprs.Length;
                var expressions = new Expression[length];
                for (var i = 0; i < length; i++)
                    expressions[i] = Expression.Lambda(exprs[i], resolutionInfo.ParameterExpressions);

                return expressions;
            }

            return null;
        }

        private void PrepareExtraParameters(ResolutionInfo resolutionInfo)
        {
            var args = base.TypeInfo.Type.GenericTypeArguments;
            var length = args.Length - 1;
            var parameters = new ParameterExpression[length];
            if (length > 0)
            {
                for (var i = 0; i < length; i++)
                {
                    var argType = args[i];
                    var argName = argType.Name + i;
                    parameters[i] = Expression.Parameter(argType, argName);
                }

                resolutionInfo.ParameterExpressions = parameters;
            }
        }
    }
}
