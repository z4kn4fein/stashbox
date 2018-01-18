using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class FuncResolver : Resolver
    {
        private readonly HashSet<Type> supportedTypes = new HashSet<Type>
        {
            typeof(Func<>),
            typeof(Func<,>),
            typeof(Func<,,>),
            typeof(Func<,,,>),
            typeof(Func<,,,,>),
            typeof(Func<,,,,,>),
            typeof(Func<,,,,,,>),
            typeof(Func<,,,,,,,>)
        };

        public override bool SupportsMany => true;

        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var args = typeInfo.Type.GetGenericArguments();
            var wrappedType = args.Last();
            var funcArgumentInfo = typeInfo.Clone(wrappedType);

            var parameters = this.PrepareExtraParameters(wrappedType, resolutionContext, args);
            var expression = containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext, funcArgumentInfo, null);

            return expression?.AsLambda(parameters);
        }

        public override Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var args = typeInfo.Type.GetGenericArguments();
            var wrappedType = args.Last();
            var funcArgumentInfo = typeInfo.Clone(wrappedType);

            var parameters = this.PrepareExtraParameters(wrappedType, resolutionContext, args);
            var expressions = containerContext.ResolutionStrategy.BuildResolutionExpressions(containerContext, resolutionContext, funcArgumentInfo);

            if (expressions == null)
                return null;

            var length = expressions.Length;
            var funcExpressions = new Expression[length];
            for (var i = 0; i < length; i++)
                funcExpressions[i] = expressions[i].AsLambda(parameters);

            return funcExpressions;
        }

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.IsClosedGenericType() && this.supportedTypes.Contains(typeInfo.Type.GetGenericTypeDefinition());

        private ParameterExpression[] PrepareExtraParameters(Type wrappedType, ResolutionContext resolutionContext, Type[] args)
        {
            var length = args.Length - 1;
            var parameters = new ParameterExpression[length];
            if (length <= 0) return parameters;

            for (var i = 0; i < length; i++)
            {
                var argType = args[i];
                var argName = wrappedType.Name + argType.Name + i;
                var parameter = argType.AsParameter(argName);
                parameters[i] = parameter;
            }

            resolutionContext.AddParameterExpressions(wrappedType, parameters);
            return parameters;
        }
    }
}
