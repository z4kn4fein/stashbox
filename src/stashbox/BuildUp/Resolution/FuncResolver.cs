using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
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

        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            var args = typeInfo.Type.GetGenericArguments();
            var wrappedType = args.Last();
            var funcArgumentInfo = new TypeInformation
            {
                Type = wrappedType,
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            var parameters = this.PrepareExtraParameters(wrappedType, resolutionInfo, args);
            var expression = containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext, resolutionInfo, funcArgumentInfo, null);
            if (expression == null)
                throw new ResolutionFailedException(typeInfo.Type.FullName);

            return Expression.Lambda(expression, parameters);
        }

        public override Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            var args = typeInfo.Type.GetGenericArguments();
            var wrappedType = args.Last();
            var funcArgumentInfo = new TypeInformation
            {
                Type = wrappedType,
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            var parameters = this.PrepareExtraParameters(wrappedType, resolutionInfo, args);
            var expressions = containerContext.ResolutionStrategy.BuildResolutionExpressions(containerContext, resolutionInfo, funcArgumentInfo);

            if (expressions == null)
                return null;

            var length = expressions.Length;
            var funcExpressions = new Expression[length];
            for (var i = 0; i < length; i++)
                funcExpressions[i] = Expression.Lambda(expressions[i], parameters);

            return funcExpressions;
        }

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo) =>
            typeInfo.Type.IsClosedGenericType() && this.supportedTypes.Contains(typeInfo.Type.GetGenericTypeDefinition());

        private ParameterExpression[] PrepareExtraParameters(Type wrappedType, ResolutionInfo resolutionInfo, Type[] args)
        {
            var length = args.Length - 1;
            var parameters = new ParameterExpression[length];
            if (length > 0)
            {
                for (var i = 0; i < length; i++)
                {
                    var argType = args[i];
                    var argName = wrappedType.Name + argType.Name + i;
                    parameters[i] = Expression.Parameter(argType, argName);
                }

                if (resolutionInfo.ParameterExpressions == null)
                    resolutionInfo.ParameterExpressions = parameters;
                else
                    resolutionInfo.ParameterExpressions = resolutionInfo.ParameterExpressions.Concat(parameters).ToArray();
            }

            return parameters;
        }
    }
}
