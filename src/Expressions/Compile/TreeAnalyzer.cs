using Stashbox.Utils.Data;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Expressions.Compile
{
    internal class TreeAnalyzer
    {
        private readonly bool isNestedLambda;

        public readonly ExpandableArray<Expression> CapturedParameters;

        public ExpandableArray<Expression> DefinedVariables;

        public readonly ExpandableArray<LambdaExpression, NestedLambda> NestedLambdas;

        public readonly ExpandableArray<object> Constants;

        public TreeAnalyzer()
        {
            this.CapturedParameters = new ExpandableArray<Expression>();
            this.DefinedVariables = new ExpandableArray<Expression>();
            this.NestedLambdas = new ExpandableArray<LambdaExpression, NestedLambda>();
            this.Constants = new ExpandableArray<object>();
        }

        private TreeAnalyzer(bool isNestedLambda,
            ExpandableArray<Expression> capturedParameters,
            ExpandableArray<Expression> definedVariables,
            ExpandableArray<LambdaExpression, NestedLambda> nestedLambdas,
            ExpandableArray<object> constants)
        {
            this.isNestedLambda = isNestedLambda;
            CapturedParameters = capturedParameters;
            DefinedVariables = definedVariables;
            NestedLambdas = nestedLambdas;
            Constants = constants;
        }

        public bool Analyze(Expression expression, params ParameterExpression[] parameters)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:

                    if (parameters.ContainsReference(expression) || this.DefinedVariables.IndexOf(expression) != -1) return true;
                    if (!this.isNestedLambda) return false;

                    this.CapturedParameters.AddOrKeep((ParameterExpression)expression);
                    return true;

                case ExpressionType.Lambda:
                    var lambda = (LambdaExpression)expression;

                    var analyzer = this.Clone(true);
                    if (!analyzer.Analyze(lambda.Body, lambda.Parameters.CastToArray()))
                        return false;

                    this.NestedLambdas.AddOrKeep(lambda, new NestedLambda(analyzer.DefinedVariables,
                        analyzer.CapturedParameters.Length > 0));
                    return true;

                case ExpressionType.MemberAccess when expression is MemberExpression memberExpression && memberExpression.Expression != null:
                    return this.Analyze(memberExpression.Expression, parameters);

                case ExpressionType.Constant:
                    var constant = (ConstantExpression)expression;
                    if (constant.Value == null || Utils.IsInPlaceEmittableConstant(constant.Type, constant.Value)) return true;
                    this.Constants.AddOrKeep(constant.Value);
                    return true;

                case ExpressionType.New:
                    return this.Analyze(((NewExpression)expression).Arguments, parameters);

                case ExpressionType.MemberInit:
                    var memberInit = (MemberInitExpression)expression;
                    return this.Analyze(memberInit.NewExpression, parameters) &&
                           this.Analyze(memberInit.Bindings, parameters);

                case ExpressionType.Block:
                    var block = (BlockExpression)expression;
                    var blockVarLength = block.Variables.Count;
                    for (var i = 0; i < blockVarLength; i++)
                        this.DefinedVariables.AddOrKeep(block.Variables[i]);

                    return this.Analyze(block.Expressions, parameters);

                case ExpressionType.Conditional:
                    var condition = (ConditionalExpression)expression;
                    return this.Analyze(condition.Test, parameters) &&
                           this.Analyze(condition.IfTrue, parameters) &&
                           this.Analyze(condition.IfFalse, parameters);

                case ExpressionType.Default:
                    return true;
                case ExpressionType.Call:
                    var call = (MethodCallExpression)expression;
                    return (call.Object == null || this.Analyze(call.Object, parameters)) &&
                           this.Analyze(call.Arguments, parameters);
                case ExpressionType.Invoke:
                    var invoke = (InvocationExpression)expression;
                    return this.Analyze(invoke.Expression, parameters) &&
                           this.Analyze(invoke.Arguments, parameters);
                case ExpressionType.NewArrayInit:
                    return this.Analyze(((NewArrayExpression)expression).Expressions, parameters);
                default:
                    switch (expression)
                    {
                        case UnaryExpression unaryExpression:
                            return this.Analyze(unaryExpression.Operand, parameters);
                        case BinaryExpression binaryExpression:
                            return this.Analyze(binaryExpression.Left, parameters) &&
                                   this.Analyze(binaryExpression.Right, parameters);
                    }

                    break;
            }

            return false;
        }

        private TreeAnalyzer Clone(bool isLambda = false) =>
            new(isLambda, this.CapturedParameters, new ExpandableArray<Expression>(), this.NestedLambdas, this.Constants);

        private bool Analyze(IList<Expression> expressions, params ParameterExpression[] parameters)
        {
            var length = expressions.Count;
            for (var i = 0; i < length; i++)
                if (!this.Analyze(expressions[i], parameters))
                    return false;

            return true;
        }

        private bool Analyze(IList<MemberBinding> bindings, params ParameterExpression[] parameters)
        {
            var length = bindings.Count;
            for (var i = 0; i < length; i++)
            {
                var binding = bindings[i];
                if (binding.BindingType != MemberBindingType.Assignment ||
                    !this.Analyze(((MemberAssignment)binding).Expression, parameters))
                    return false;
            }

            return true;
        }
    }
}
