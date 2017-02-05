using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        private readonly IServiceRegistration registrationCache;
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

            this.registrationCache = containerContext.RegistrationRepository.GetRegistrationOrDefault(this.funcArgumentInfo, true);
            if(this.registrationCache == null)
                throw new ResolutionFailedException(typeInfo.Type.FullName);
        }
        
        public override Expression GetExpression(ResolutionInfo resolutionInfo)
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
            var expr = registrationCache.GetExpression(resolutionInfo, this.funcArgumentInfo);
            return Expression.Lambda(expr, parameters);
        }
    }
}
