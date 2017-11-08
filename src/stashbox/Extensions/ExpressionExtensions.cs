using Stashbox.Infrastructure;
using Stashbox.Resolution;
using System.Reflection;
using System.Collections.Generic;
using Stashbox.Entity.Resolution;

#if NET45 || NET40 || NETSTANDARD1_3
using Stashbox.BuildUp.Expressions.Compile;
#endif

namespace System.Linq.Expressions
{
    /// <summary>
    /// Holds the <see cref="Expression"/> extension methods.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Compiles an <see cref="Expression"/> to a <see cref="Func{T,R}"/> of <see cref="IResolutionScope"/>, <see cref="object"/>.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <returns>The compiled delegate.</returns>
        public static Func<IResolutionScope, object> CompileDelegate(this Expression expression, ResolutionContext resolutionContext)
        {
            if (expression.NodeType == ExpressionType.Constant)
            {
                var instance = ((ConstantExpression)expression).Value;
                return scope => instance;
            }

            if (resolutionContext.DefinedVariables.Length > 0)
            {
                var originalExpression = expression;
                expression = Expression.Block(resolutionContext.DefinedVariables,
                    resolutionContext.SingleInstructions.Add(originalExpression));
            }

#if NET45 || NET40 || NETSTANDARD1_3
            if (!expression.TryEmit(out Delegate factory, typeof(Func<IResolutionScope, object>), typeof(object),
                resolutionContext.CurrentScopeParameter))
                factory = Expression.Lambda(expression, resolutionContext.CurrentScopeParameter).Compile();

            return (Func<IResolutionScope, object>)factory;
#else
            return Expression.Lambda<Func<IResolutionScope, object>>(expression, resolutionContext.CurrentScopeParameter).Compile();
#endif
        }

        /// <summary>
        /// Compiles an <see cref="Expression"/> to a <see cref="Func{T,R}"/> of <see cref="IResolutionScope"/>, <see cref="Delegate"/>.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <returns>The compiled delegate.</returns>
        public static Func<IResolutionScope, Delegate> CompileDynamicDelegate(this Expression expression, ResolutionContext resolutionContext)
        {
            if (resolutionContext.DefinedVariables.Length > 0)
            {
                var originalExpression = expression;
                expression = Expression.Block(resolutionContext.DefinedVariables,
                    resolutionContext.SingleInstructions.Add(originalExpression));
            }

#if NET45 || NET40 || NETSTANDARD1_3
            if (!expression.TryEmit(out Delegate factory, typeof(Func<IResolutionScope, Delegate>), typeof(Delegate),
                resolutionContext.CurrentScopeParameter))
                factory = Expression.Lambda<Func<IResolutionScope, Delegate>>(expression, resolutionContext.CurrentScopeParameter).Compile();

            return (Func<IResolutionScope, Delegate>)factory;
#else
            return Expression.Lambda<Func<IResolutionScope, Delegate>>(expression, resolutionContext.CurrentScopeParameter).Compile();
#endif
        }

        internal static BinaryExpression AssignTo(this Expression left, Expression right) => Expression.Assign(left, right);

        internal static MemberAssignment AssignTo(this MemberInfo memberInfo, Expression expression) =>
            Expression.Bind(memberInfo, expression);

        internal static ConstantExpression AsConstant(this object obj) => Expression.Constant(obj);

        internal static ConstantExpression AsConstant(this object obj, Type type) => Expression.Constant(obj, type);

        internal static DefaultExpression AsDefault(this Type type) => Expression.Default(type);

        internal static BlockExpression AsBlock(this IList<Expression> expressions, params ParameterExpression[] variables) =>
            Expression.Block(variables, expressions);

        internal static LambdaExpression AsLambda(this Expression expression, params ParameterExpression[] parameters) =>
            Expression.Lambda(expression, parameters);

        internal static LambdaExpression AsLambda(this Expression expression, IEnumerable<ParameterExpression> parameters) =>
            Expression.Lambda(expression, parameters);

        internal static ParameterExpression AsVariable(this Type type, string name = null) => Expression.Variable(type, name);

        internal static ParameterExpression AsParameter(this Type type, string name = null) => Expression.Parameter(type, name);

        internal static MethodCallExpression InvokeMethod(this MethodInfo methodInfo, params Expression[] parameters) =>
            Expression.Call(methodInfo, parameters);

        internal static MethodCallExpression CallMethod(this Expression target, MethodInfo methodInfo, params Expression[] parameters) =>
            Expression.Call(target, methodInfo, parameters);

        internal static Expression ConvertTo(this Expression expression, Type type) => Expression.Convert(expression, type);

        internal static InvocationExpression InvokeLambda(this LambdaExpression expression, params Expression[] parameters) =>
            Expression.Invoke(expression, parameters);

        internal static NewExpression MakeNew(this ResolutionConstructor constructor) =>
            Expression.New(constructor.Constructor, constructor.Parameters);

        internal static NewExpression MakeNew(this ConstructorInfo constructor, IEnumerable<Expression> arguments) =>
           Expression.New(constructor, arguments);

        internal static NewExpression MakeNew(this ConstructorInfo constructor, params Expression[] arguments) =>
           Expression.New(constructor, arguments);

        internal static MemberExpression Member(this Expression expression, MemberInfo memberInfo) =>
            memberInfo is PropertyInfo prop ? Expression.Property(expression, prop) : Expression.Field(expression, memberInfo as FieldInfo);

        internal static MemberExpression Prop(this Expression expression, PropertyInfo propertyInfo) =>
            Expression.Property(expression, propertyInfo);

        internal static MemberInitExpression InitMembers(this Expression expression, IList<MemberBinding> bindings) =>
           Expression.MemberInit((NewExpression)expression, bindings);

        internal static NewArrayExpression InitNewArray(this Type type, params Expression[] initializers) =>
          Expression.NewArrayInit(type, initializers);
    }
}
