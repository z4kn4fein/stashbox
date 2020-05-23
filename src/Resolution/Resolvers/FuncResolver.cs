using Stashbox.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class FuncResolver : IMultiServiceResolver
    {
        internal static readonly HashSet<Type> SupportedTypes = new HashSet<Type>
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

        public Expression GetExpression(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var args = typeInfo.Type.GetGenericArguments();
            var wrappedType = args.Last();
            var funcArgumentInfo = typeInfo.Clone(wrappedType);

            var parameters = this.PrepareExtraParameters(wrappedType, resolutionContext, args);
            var expression = resolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext, funcArgumentInfo);

            return expression?.AsLambda(typeInfo.Type, parameters);
        }

        public Expression[] GetAllExpressions(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var args = typeInfo.Type.GetGenericArguments();
            var wrappedType = args.Last();
            var funcArgumentInfo = typeInfo.Clone(wrappedType);

            var parameters = this.PrepareExtraParameters(wrappedType, resolutionContext, args);
            var expressions = resolutionStrategy.BuildAllResolutionExpressions(containerContext, resolutionContext, funcArgumentInfo);

            if (expressions == null)
                return null;

            var length = expressions.Length;
            var funcExpressions = new Expression[length];
            for (var i = 0; i < length; i++)
                funcExpressions[i] = expressions[i].AsLambda(typeInfo.Type, parameters);

            return funcExpressions;
        }

        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.IsFuncType();

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

            resolutionContext.AddParameterExpressions(parameters);
            return parameters;
        }
    }
}
