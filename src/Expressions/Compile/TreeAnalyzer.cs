#if IL_EMIT
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Expressions.Compile
{
    internal class TreeAnalyzer
    {
        private bool isNestedLambda;

        public ExpandableArray<Expression> CapturedParameters;

        public ExpandableArray<Expression> DefinedVariables;

        public ExpandableArray<NestedLambda> NestedLambdas;

        public ExpandableArray<object> StoredObjects;

        public TreeAnalyzer()
        {
            this.CapturedParameters = new ExpandableArray<Expression>();
            this.DefinedVariables = new ExpandableArray<Expression>();
            this.NestedLambdas = new ExpandableArray<NestedLambda>();
            this.StoredObjects = new ExpandableArray<object>();
        }

        public bool Analyze(Expression expression, params ParameterExpression[] parameters)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:

                    if (parameters.ContainsReference(expression) || this.DefinedVariables.ContainsReference(expression)) return true;
                    if (!this.isNestedLambda) return false;

                    this.AddCapturedParameter((ParameterExpression)expression);
                    return true;

                case ExpressionType.Lambda:
                    var lambda = (LambdaExpression)expression;

                    var analyzer = this.Clone(true);
                    if (!analyzer.Analyze(lambda.Body, lambda.Parameters.CastToArray()))
                        return false;

                    this.AddNestedLambda(new NestedLambda(lambda,
                        analyzer.DefinedVariables,
                        analyzer.CapturedParameters.Length > 0));

                    return true;

                case ExpressionType.MemberAccess:
                    return this.Analyze(((MemberExpression)expression).Expression, parameters);

                case ExpressionType.Constant:
                    var constant = (ConstantExpression)expression;
                    if (constant.Value == null || Utils.IsInPlaceEmittableConstant(constant.Type, constant.Value)) return true;
                    this.AddStoredItem(constant.Value);
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
                        this.AddDefinedVariable(block.Variables[i]);

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
                    if (expression is UnaryExpression unaryExpression)
                        return this.Analyze(unaryExpression.Operand, parameters);

                    if (expression is BinaryExpression binaryExpression)
                        return this.Analyze(binaryExpression.Left, parameters) &&
                            this.Analyze(binaryExpression.Right, parameters);

                    break;
            }

            return false;
        }

        public TreeAnalyzer Clone(bool isLambda = false)
        {
            var clone = (TreeAnalyzer)this.MemberwiseClone();
            clone.DefinedVariables = new ExpandableArray<Expression>();
            clone.isNestedLambda = isLambda;
            return clone;
        }

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

        public void AddStoredItem(object value)
        {
            if (this.StoredObjects.ContainsReference(value)) return;
            this.StoredObjects.Add(value);
        }

        private void AddCapturedParameter(Expression expression)
        {
            if (this.CapturedParameters.ContainsReference(expression)) return;
            this.CapturedParameters.Add(expression);
        }

        private void AddDefinedVariable(Expression expression)
        {
            if (this.DefinedVariables.ContainsReference(expression)) return;
            this.DefinedVariables.Add(expression);
        }

        private void AddNestedLambda(NestedLambda lambda)
        {
            if (this.NestedLambdas.ContainsReference(lambda)) return;
            this.NestedLambdas.Add(lambda);
        }
    }
}
#endif
