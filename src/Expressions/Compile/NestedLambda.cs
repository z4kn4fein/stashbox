#if IL_EMIT
using System.Linq.Expressions;
using Stashbox.Utils;

namespace Stashbox.Expressions.Compile
{
    internal class NestedLambda
    {
        public readonly ExpandableArray<Expression> ParameterExpressions;
        public readonly bool UsesCapturedArgument;
        public readonly LambdaExpression LambdaExpression;

        public NestedLambda(LambdaExpression lambdaExpression, ExpandableArray<Expression> parameterExpressions, bool usesCapturedArgument)
        {
            this.ParameterExpressions = parameterExpressions;
            this.UsesCapturedArgument = usesCapturedArgument;
            this.LambdaExpression = lambdaExpression;
        }
    }
}
#endif
