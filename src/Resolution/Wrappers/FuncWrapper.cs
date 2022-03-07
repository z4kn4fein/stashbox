using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Wrappers
{
    internal class FuncWrapper : IParameterizedWrapper
    {
        private static readonly HashSet<Type> SupportedTypes = new()
        {
            typeof(Func<>),
            typeof(Func<,>),
            typeof(Func<,,>),
            typeof(Func<,,,>),
            typeof(Func<,,,,>),
            typeof(Func<,,,,,>),
            typeof(Func<,,,,,,>),
            typeof(Func<,,,,,,,>),
        };

        private static bool IsFunc(Type type) => type.IsClosedGenericType() && SupportedTypes.Contains(type.GetGenericTypeDefinition());

        public Expression WrapExpression(TypeInformation originalTypeInformation, TypeInformation wrappedTypeInformation,
            ServiceContext serviceContext, IEnumerable<ParameterExpression> parameterExpressions) => 
            serviceContext.ServiceExpression.AsLambda(originalTypeInformation.Type, parameterExpressions);

        public bool TryUnWrap(TypeInformation typeInformation, out TypeInformation unWrappedType, out IEnumerable<Type> parameterTypes)
        {
            if (!IsFunc(typeInformation.Type))
            {
                unWrappedType = default;
                parameterTypes = Constants.EmptyTypes;
                return false;
            }

            var args = typeInformation.Type.GetGenericArguments();
            unWrappedType = typeInformation.Clone(args.LastElement());
            parameterTypes = args.SelectButLast();
            return true;
        }
    }
}
