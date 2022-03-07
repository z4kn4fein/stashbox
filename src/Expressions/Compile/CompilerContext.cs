using Stashbox.Utils.Data;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile
{
    internal class CompilerContext
    {
        public readonly ExpandableArray<Expression> DefinedVariables;

        public readonly bool HasCapturedVariablesArgument;

        public readonly bool IsNestedLambda;

        public readonly ExpandableArray<object> Constants;

        public readonly ExpandableArray<Expression> CapturedArguments;

        public readonly Closure? Target;

        public LocalBuilder[]? LocalBuilders;

        public LocalBuilder? CapturedArgumentsHolderVariable;

        public readonly ExpandableArray<LambdaExpression, NestedLambda> NestedLambdas;

        public readonly bool HasClosure;

        public CompilerContext(Closure? target,
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

        private CompilerContext(ExpandableArray<Expression> definedVariables, 
            bool hasCapturedVariablesArgument, 
            bool isNestedLambda, 
            ExpandableArray<object> constants, 
            ExpandableArray<Expression> capturedArguments, 
            Closure? target, 
            LocalBuilder[]? localBuilders, 
            LocalBuilder? capturedArgumentsHolderVariable, 
            ExpandableArray<LambdaExpression, NestedLambda> nestedLambdas, 
            bool hasClosure)
        {
            DefinedVariables = definedVariables;
            HasCapturedVariablesArgument = hasCapturedVariablesArgument;
            IsNestedLambda = isNestedLambda;
            Constants = constants;
            CapturedArguments = capturedArguments;
            Target = target;
            LocalBuilders = localBuilders;
            CapturedArgumentsHolderVariable = capturedArgumentsHolderVariable;
            NestedLambdas = nestedLambdas;
            HasClosure = hasClosure;
        }

        public CompilerContext Clone(ExpandableArray<Expression> definedVariables, bool isNestedLambda, bool hasCapturedArgument) =>
            new(definedVariables,
                hasCapturedArgument,
                isNestedLambda,
                this.Constants,
                this.CapturedArguments,
                this.Target,
                this.LocalBuilders,
                this.CapturedArgumentsHolderVariable,
                this.NestedLambdas,
                this.HasClosure);
    }
}