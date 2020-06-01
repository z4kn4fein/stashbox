#if IL_EMIT
using System.Linq.Expressions;
using System.Reflection.Emit;
using Stashbox.Utils;

namespace Stashbox.Expressions.Compile
{
    internal class CompilerContext
    {
        public ExpandableArray<Expression> DefinedVariables;

        public bool HasCapturedVariablesArgument;

        public bool IsNestedLambda;

        public readonly ExpandableArray<Expression> CapturedArguments;

        public readonly Closure Target;

        public ExpandableArray<LocalBuilder> LocalBuilders;

        public LocalBuilder CapturedArgumentsHolderVariable;

        public readonly ExpandableArray<NestedLambda> NestedLambdas;

        public readonly bool HasClosure;

        public CompilerContext(Closure target,
            ExpandableArray<Expression> definedVariables,
            ExpandableArray<Expression> capturedArguments,
            ExpandableArray<NestedLambda> nestedLambdas)
        {
            this.Target = target;
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