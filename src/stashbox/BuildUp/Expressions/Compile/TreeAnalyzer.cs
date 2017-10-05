using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class TreeAnalyzer
    {
        public Expression[] ClosureExpressions { get; private set; } = new Expression[0];

        public object[] ClosureObjects { get; private set; } = new object[0];

        public bool Analyze(Expression expression, params ParameterExpression[] parameters)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:

                    if (Array.IndexOf(parameters, expression) != -1) return true;
                    this.AddClosureItem(expression);
                    return true;

                case ExpressionType.Lambda:
                    var lambda = (LambdaExpression)expression;
                    if (!this.Analyze(lambda.Parameters, parameters))
                        return false;

                    if (!this.Analyze(lambda.Body, parameters))
                        return false;

                    this.AddClosureItem(lambda);
                    return true;

                case ExpressionType.MemberAccess:
                    return this.Analyze(((MemberExpression)expression).Expression, parameters);

                case ExpressionType.Constant:
                    var constant = (ConstantExpression)expression;
                    if (constant.Value == null || this.IsInPlaceEmittableConstant(constant.Type, constant.Value)) return true;
                    this.AddClosureItem(constant, constant.Value);
                    return true;

                case ExpressionType.New:
                    return this.Analyze(((NewExpression)expression).Arguments, parameters);

                case ExpressionType.MemberInit:
                    var memberInit = (MemberInitExpression)expression;
                    if (!this.Analyze(memberInit.NewExpression, parameters))
                        return false;

                    return this.Analyze(memberInit.Bindings, parameters);

                case ExpressionType.Block:
                    var block = (BlockExpression)expression;
                    if (!this.Analyze(block.Variables, parameters))
                        return false;

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

        public bool Analyze(IList<Expression> expressions, params ParameterExpression[] parameters)
        {
            for (var i = 0; i < expressions.Count; i++)
                if (!this.Analyze(expressions[i], parameters))
                    return false;

            return true;
        }

        public bool Analyze(IList<ParameterExpression> expressions, params ParameterExpression[] parameters)
        {
            for (var i = 0; i < expressions.Count; i++)
                if (!this.Analyze(expressions[i], parameters))
                    return false;

            return true;
        }

        public bool Analyze(IList<MemberBinding> bindings, params ParameterExpression[] parameters)
        {
            for (var i = 0; i < bindings.Count; i++)
            {
                var binding = bindings[i];
                if (binding.BindingType != MemberBindingType.Assignment ||
                    !this.Analyze(((MemberAssignment)binding).Expression, parameters))
                    return false;
            }

            return true;
        }

        private void AddClosureItem(Expression expression, object value = null)
        {
            if (this.ClosureExpressions.ContainsElement(expression)) return;
            this.ClosureExpressions = this.ClosureExpressions.AddElement(expression);
            this.ClosureObjects = this.ClosureObjects.AddElement(value);
        }

        private bool IsInPlaceEmittableConstant(Type type, object value)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsPrimitive || typeInfo.IsEnum
                   || (value is string)
                   || (value is Type);
        }
    }
}
