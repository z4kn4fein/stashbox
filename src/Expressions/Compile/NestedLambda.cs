#if IL_EMIT
using Stashbox.Utils;
using Stashbox.Utils.Data;
using System.Linq.Expressions;

namespace Stashbox.Expressions.Compile
{
    internal class NestedLambda
    {
        public readonly ExpandableArray<Expression> ParameterExpressions;
        public readonly bool UsesCapturedArgument;

        public NestedLambda(ExpandableArray<Expression> parameterExpressions, bool usesCapturedArgument)
        {
            this.ParameterExpressions = parameterExpressions;
            this.UsesCapturedArgument = usesCapturedArgument;
        }
    }
}
#endif
