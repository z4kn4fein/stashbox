using Stashbox.Entity;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class TupleResolver : IResolver
    {
        private readonly HashSet<Type> supportedTypes = new HashSet<Type>
        {
            typeof(Tuple<>),
            typeof(Tuple<,>),
            typeof(Tuple<,,>),
            typeof(Tuple<,,,>),
            typeof(Tuple<,,,,>),
            typeof(Tuple<,,,,,>),
            typeof(Tuple<,,,,,,>),
            typeof(Tuple<,,,,,,,>)
        };

        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.IsClosedGenericType() && this.supportedTypes.Contains(typeInfo.Type.GetGenericTypeDefinition());

        public Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var args = typeInfo.Type.GetGenericArguments();
            var tupleConstructor = typeInfo.Type.GetConstructor(args);
            var length = args.Length;
            var expressions = new Expression[length];
            for (var i = 0; i < length; i++)
            {
                var argumentInfo = new TypeInformation { Type = args[i] };
                var expr = containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext, argumentInfo, null);

                if (expr != null)
                    expressions[i] = expr;
                else
                    return null;
            }

            return tupleConstructor.MakeNew(expressions);
        }
    }
}
