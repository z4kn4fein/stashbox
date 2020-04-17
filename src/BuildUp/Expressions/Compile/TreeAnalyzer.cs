using Stashbox.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class TreeAnalyzer
    {
        private readonly bool isNestedLambda;

        public Expression[] StoredExpressions { get; private set; } = new Expression[0];

        public Expression[] CapturedParameters { get; private set; } = new Expression[0];

        public Expression[] DefinedVariables { get; private set; } = new Expression[0];

        public LambdaExpression[] NestedLambdas { get; private set; } = new LambdaExpression[0];

        public Expression[][] NestedLambdaVariables { get; private set; } = new Expression[0][];

        public Type[] StoredObjectTypes { get; private set; } = new Type[0];

        public object[] StoredObjects { get; private set; } = new object[0];

        public TreeAnalyzer(bool isNestedLambda = false)
        {
            this.isNestedLambda = isNestedLambda;
        }

        public bool Analyze(Expression expression, params ParameterExpression[] parameters)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:

                    if (parameters.ContainsElement(expression) || this.DefinedVariables.ContainsElement(expression)) return true;
                    if (!this.isNestedLambda) return false;

                    this.AddCapturedParameter((ParameterExpression)expression);
                    return true;

                case ExpressionType.Lambda:
                    var lambda = (LambdaExpression)expression;

                    var analyzer = new TreeAnalyzer(true);
                    if (!analyzer.Analyze(lambda.Body, lambda.Parameters.CastToArray()))
                        return false;

                    this.AddNestedLambda(lambda, analyzer.DefinedVariables);

                    var length = analyzer.StoredExpressions.Length;
                    for (var i = 0; i < length; i++)
                        this.AddStoredItem(analyzer.StoredExpressions[i], analyzer.StoredExpressions[i].Type, analyzer.StoredObjects[i]);

                    var lambdaLength = analyzer.NestedLambdas.Length;
                    for (var i = 0; i < lambdaLength; i++)
                        this.AddNestedLambda(analyzer.NestedLambdas[i], analyzer.NestedLambdaVariables[i]);

                    var capturedLength = analyzer.CapturedParameters.Length;
                    for (var i = 0; i < capturedLength; i++)
                        this.AddCapturedParameter(analyzer.CapturedParameters[i]);

                    return true;

                case ExpressionType.MemberAccess:
                    return this.Analyze(((MemberExpression)expression).Expression, parameters);

                case ExpressionType.Constant:
                    var constant = (ConstantExpression)expression;
                    if (constant.Value == null || IsInPlaceEmittableConstant(constant.Type, constant.Value)) return true;
                    this.AddStoredItem(constant, constant.Type, constant.Value);
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
                    for (var i = 0; i < block.Variables.Count; i++)
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

        private bool Analyze(IList<Expression> expressions, params ParameterExpression[] parameters)
        {
            for (var i = 0; i < expressions.Count; i++)
                if (!this.Analyze(expressions[i], parameters))
                    return false;

            return true;
        }

        private bool Analyze(IList<MemberBinding> bindings, params ParameterExpression[] parameters)
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

        public void AddStoredItem(Expression expression, Type type, object value = null)
        {
            if (this.StoredExpressions.ContainsElement(expression)) return;
            this.StoredExpressions = this.StoredExpressions.AddElement(expression);
            this.StoredObjectTypes = this.StoredObjectTypes.AddElement(type);
            this.StoredObjects = this.StoredObjects.AddElement(value);
        }

        private void AddCapturedParameter(Expression expression)
        {
            if (this.CapturedParameters.ContainsElement(expression)) return;
            this.CapturedParameters = this.CapturedParameters.AddElement(expression);
        }

        private void AddDefinedVariable(Expression expression)
        {
            if (this.DefinedVariables.ContainsElement(expression)) return;
            this.DefinedVariables = this.DefinedVariables.AddElement(expression);
        }

        private void AddNestedLambda(LambdaExpression lambda, Expression[] variables)
        {
            if (this.NestedLambdas.ContainsElement(lambda)) return;
            this.NestedLambdas = this.NestedLambdas.AddElement(lambda);
            this.NestedLambdaVariables = this.NestedLambdaVariables.AddElement(variables);
        }

        private static bool IsInPlaceEmittableConstant(Type type, object value)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsPrimitive || typeInfo.IsEnum
                   || value is string
                   || value is Type;
        }
    }
}
