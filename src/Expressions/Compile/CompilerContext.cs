#if IL_EMIT
using Stashbox.Utils;
using Stashbox.Utils.Data;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile
{
    internal class CompilerContext
    {
        public ExpandableArray<Expression> DefinedVariables;

        public bool HasCapturedVariablesArgument;

        public bool IsNestedLambda;

        public readonly ExpandableArray<object> Constants;

        public readonly ExpandableArray<Expression> CapturedArguments;

        public readonly Closure Target;

        public LocalBuilder[] LocalBuilders;

        public LocalBuilder CapturedArgumentsHolderVariable;

        public readonly ExpandableArray<LambdaExpression, NestedLambda> NestedLambdas;

        public readonly bool HasClosure;

        public CompilerContext(Closure target,
            ExpandableArray<object> constants,
            ExpandableArray<Expression> definedVariables,
            ExpandableArray<Expression> capturedArguments,
            ExpandableArray<LambdaExpression, NestedLambda> nestedLambdas)
        {
            this.Target = target;
            this.Constants = constants;
            this.DefinedVariables = definedVariables;
            this.CapturedArguments = capturedArguments;
            this.NestedLambdas = nestedLambdas;
            this.HasClosure = target != null;
            this.HasCapturedVariablesArgument = capturedArguments.Length > 0;
        }

        public CompilerContext CreateNew(ExpandableArray<Expression> definedVariables, bool isNestedLambda, bool hasCapturedArgument)
        {
            var clone = (CompilerContext)this.MemberwiseClone();
            clone.DefinedVariables = definedVariables;
            clone.IsNestedLambda = isNestedLambda;
            clone.HasCapturedVariablesArgument = hasCapturedArgument;
            return clone;
        }
    }
}
#endif