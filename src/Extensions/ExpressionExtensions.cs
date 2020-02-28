using Stashbox.Resolution;
using System.Reflection;
using System.Collections.Generic;
using Stashbox;
using Stashbox.Configuration;
using Stashbox.Entity.Resolution;
using Stashbox.Utils;
#if IL_EMIT
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
        /// <param name="containerConfiguration">The container configuration.</param>
        /// <returns>The compiled delegate.</returns>
        public static Func<IResolutionScope, object> CompileDelegate(this Expression expression,
            ResolutionContext resolutionContext, ContainerConfiguration containerConfiguration)
        {
            if (expression.NodeType == ExpressionType.Constant)
            {
                var instance = ((ConstantExpression)expression).Value;
                return scope => instance;
            }

            if (resolutionContext.DefinedVariables.Length > 0)
            {
                var instructions = new ArrayList<Expression>(resolutionContext.SingleInstructions) { expression };
                expression = Expression.Block(resolutionContext.DefinedVariables, instructions);
                
                resolutionContext.SingleInstructions.Clear();
            }

#if IL_EMIT
            if (containerConfiguration.ForceUseMicrosoftExpressionCompiler ||
                !expression.TryEmit(out Delegate factory, typeof(Func<IResolutionScope, object>), typeof(object),
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
        /// <param name="containerConfiguration">The container configuration.</param>
        /// <returns>The compiled delegate.</returns>
        public static Func<IResolutionScope, Delegate> CompileDynamicDelegate(this Expression expression,
            ResolutionContext resolutionContext, ContainerConfiguration containerConfiguration)
        {
            if (resolutionContext.DefinedVariables.Length > 0)
            {
                var instructions = new ArrayList<Expression>(resolutionContext.SingleInstructions) { expression };
                expression = Expression.Block(resolutionContext.DefinedVariables, instructions);
                
                resolutionContext.SingleInstructions.Clear();
            }

#if IL_EMIT
            if (containerConfiguration.ForceUseMicrosoftExpressionCompiler ||
                !expression.TryEmit(out Delegate factory, typeof(Func<IResolutionScope, Delegate>), typeof(Delegate),
                resolutionContext.CurrentScopeParameter))
                factory = Expression.Lambda<Func<IResolutionScope, Delegate>>(expression, resolutionContext.CurrentScopeParameter).Compile();

            return (Func<IResolutionScope, Delegate>)factory;
#else
            return Expression.Lambda<Func<IResolutionScope, Delegate>>(expression, resolutionContext.CurrentScopeParameter).Compile();
#endif
        }

        /// <summary>
        /// Compiles a <see cref="LambdaExpression"/> to a <see cref="Delegate"/>. For testing purposes.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The delegate.</returns>
        public static Delegate CompileDelegate(this LambdaExpression expression)
        {
#if IL_EMIT
            if (!expression.TryEmit(out var result))
                throw new InvalidOperationException("Could not compile the given expression!");

            return result;
#else
            throw new InvalidOperationException("Could not compile the given expression!");
#endif
        }

        /// <summary>
        /// Compiles a lambda expression into a Func delegate.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="expression">The expression</param>
        /// <returns>The delegate.</returns>
        public static Func<T> CompileFunc<T>(this Expression<Func<T>> expression) =>
            (Func<T>)expression.CompileDelegate();

        /// <summary>
        /// Compiles a lambda expression into a Func delegate.
        /// </summary>
        /// <typeparam name="T1">First parameter type.</typeparam>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="expression">The expression</param>
        /// <returns>The delegate.</returns>
        public static Func<T1, T> CompileFunc<T1, T>(this Expression<Func<T1, T>> expression) =>
            (Func<T1, T>)expression.CompileDelegate();

        /// <summary>
        /// Constructs an assigment expression, => Expression.Assign(left, right)
        /// </summary>
        /// <param name="left">The left part.</param>
        /// <param name="right">The right part.</param>
        /// <returns>The assignment expression.</returns>
        public static BinaryExpression AssignTo(this Expression left, Expression right) => Expression.Assign(left, right);

        /// <summary>
        /// Constructs an assigment expression, => Expression.Bind(member, expression)
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="expression">The right part.</param>
        /// <returns>The assignment expression.</returns>
        public static MemberAssignment AssignTo(this MemberInfo memberInfo, Expression expression) =>
            Expression.Bind(memberInfo, expression);

        /// <summary>
        /// Constructs a constant expression from an object, => Expression.Constant(obj)
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The constant expression.</returns>
        public static ConstantExpression AsConstant(this object obj) => Expression.Constant(obj);

        /// <summary>
        /// Constructs a constant expression from an object and a type, => Expression.Constant(obj, type)
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="type">The type.</param>
        /// <returns>The constant expression.</returns>
        public static ConstantExpression AsConstant(this object obj, Type type) => Expression.Constant(obj, type);

        /// <summary>
        /// Constructs a default expression from a type, => Expression.Default(type)
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default expression.</returns>
        public static DefaultExpression AsDefault(this Type type) => Expression.Default(type);

        /// <summary>
        /// Constructs a block expression from an expression collection and variables, => Expression.Block(variables, expressions)
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <param name="variables">The variables.</param>
        /// <returns>The block expression.</returns>
        public static BlockExpression AsBlock(this IList<Expression> expressions, params ParameterExpression[] variables) =>
            Expression.Block(variables, expressions);

        /// <summary>
        /// Constructs a lambda expression from an expression and parameters, => Expression.Lambda(expression, parameters)
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The lambda expression.</returns>
        public static LambdaExpression AsLambda(this Expression expression, params ParameterExpression[] parameters) =>
            Expression.Lambda(expression, parameters);

        /// <summary>
        /// Constructs a lambda expression from an expression and parameters, => Expression.Lambda(expression, parameters)
        /// </summary>
        /// <param name="delegateType">The type of the delegate.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The lambda expression.</returns>
        public static LambdaExpression AsLambda(this Expression expression, Type delegateType, params ParameterExpression[] parameters) =>
            Expression.Lambda(delegateType, expression, parameters);

        /// <summary>
        /// Constructs a lambda expression from an expression and parameters, => Expression.Lambda(expression, parameters)
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The lambda expression.</returns>
        public static LambdaExpression AsLambda(this Expression expression, IEnumerable<ParameterExpression> parameters) =>
            Expression.Lambda(expression, parameters);

        /// <summary>
        /// Constructs a lambda expression from an expression and parameters, => Expression.Lambda{TDelegate}(expression, parameters)
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The lambda expression.</returns>
        public static Expression<TDelegate> AsLambda<TDelegate>(this Expression expression, params ParameterExpression[] parameters) =>
            Expression.Lambda<TDelegate>(expression, parameters);

        /// <summary>
        /// Constructs a variable expression from a type, => Expression.Variable(type, name)
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <returns>The variable expression.</returns>
        public static ParameterExpression AsVariable(this Type type, string name = null) => Expression.Variable(type, name);

        /// <summary>
        /// Constructs a parameter expression from a type, => Expression.Parameter(type, name)
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <returns>The parameter expression.</returns>
        public static ParameterExpression AsParameter(this Type type, string name = null) => Expression.Parameter(type, name);

        /// <summary>
        /// Constructs a static method call expression from a <see cref="MethodInfo"/> and its parameters, => Expression.Call(methodInfo, parameters)
        /// </summary>
        /// <param name="methodInfo">The static method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The call expression.</returns>
        public static MethodCallExpression CallStaticMethod(this MethodInfo methodInfo, params Expression[] parameters) =>
            Expression.Call(methodInfo, parameters);

        /// <summary>
        /// Constructs a method call expression from a target expression, method info and parameters, => Expression.Call(target, methodInfo, parameters)
        /// </summary>
        /// <param name="target">The target expression.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The call expression.</returns>
        public static MethodCallExpression CallMethod(this Expression target, MethodInfo methodInfo, params Expression[] parameters) =>
            Expression.Call(target, methodInfo, parameters);

        /// <summary>
        /// Constructs a method call expression from a target expression, method info and parameters, => Expression.Call(target, methodInfo, parameters)
        /// </summary>
        /// <param name="target">The target expression.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The call expression.</returns>
        public static MethodCallExpression CallMethod(this MethodInfo methodInfo, Expression target, params Expression[] parameters) =>
            target.CallMethod(methodInfo, parameters);

        /// <summary>
        /// Constructs a convert expression, => Expression.Convert(expression, type)
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="type">The type.</param>
        /// <returns>The convert expression.</returns>
        public static Expression ConvertTo(this Expression expression, Type type) => Expression.Convert(expression, type);

        /// <summary>
        /// Constructs an invocation expression, => Expression.Invoke(expression, parameters)
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The invocation expression.</returns>
        public static InvocationExpression InvokeLambda(this LambdaExpression expression, params Expression[] parameters) =>
            Expression.Invoke(expression, parameters);

        /// <summary>
        /// Constructs an invocation expression, => Expression.Invoke(delegate.AsConstant(), parameters)
        /// </summary>
        /// <param name="delegate">The delegate to invoke.</param>
        /// <param name="parameters">The delegate parameters.</param>
        /// <returns>The invocation expression.</returns>
        public static InvocationExpression InvokeDelegate(this Delegate @delegate, params Expression[] parameters) =>
            Expression.Invoke(@delegate.AsConstant(), parameters);

        internal static NewExpression MakeNew(this ResolutionConstructor constructor) =>
            Expression.New(constructor.Constructor, constructor.Parameters);

        /// <summary>
        /// Constructs an new expression, => Expression.New(constructor, arguments)
        /// </summary>
        /// <param name="constructor">The constructor info.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The new expression.</returns>
        public static NewExpression MakeNew(this ConstructorInfo constructor, IEnumerable<Expression> arguments) =>
           Expression.New(constructor, arguments);

        /// <summary>
        /// Constructs an new expression, => Expression.New(constructor, arguments)
        /// </summary>
        /// <param name="constructor">The constructor info.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The new expression.</returns>
        public static NewExpression MakeNew(this ConstructorInfo constructor, params Expression[] arguments) =>
           Expression.New(constructor, arguments);

        /// <summary>
        /// Constructs a member access expression, => Expression.Property(expression, prop) or Expression.Field(expression, field)
        /// </summary>
        /// <param name="expression">The target expression.</param>
        /// <param name="memberInfo">The property or field info.</param>
        /// <returns>The member access expression.</returns>
        public static MemberExpression Member(this Expression expression, MemberInfo memberInfo) =>
            memberInfo is PropertyInfo prop ? Expression.Property(expression, prop) : Expression.Field(expression, memberInfo as FieldInfo);

        /// <summary>
        /// Constructs a property access expression, => Expression.Property(expression, prop)
        /// </summary>
        /// <param name="expression">The target expression.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>The property access expression.</returns>
        public static MemberExpression Prop(this Expression expression, PropertyInfo propertyInfo) =>
            Expression.Property(expression, propertyInfo);

        /// <summary>
        /// Constructs a property access expression, => Expression.Property(expression, prop)
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="expression">The target expression.</param>
        /// <returns>The property access expression.</returns>
        public static MemberExpression Access(this PropertyInfo propertyInfo, Expression expression) =>
            Expression.Property(expression, propertyInfo);

        /// <summary>
        /// Constructs a member init expression, => Expression.MemberInit(expression, bindings)
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="bindings">The member bindings.</param>
        /// <returns>The member init expression.</returns>
        public static MemberInitExpression InitMembers(this Expression expression, IList<MemberBinding> bindings) =>
           Expression.MemberInit((NewExpression)expression, bindings);

        /// <summary>
        /// Constructs a new array expression, => Expression.NewArrayInit(type, initializers)
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="initializers">The element initializer expressions.</param>
        /// <returns>The new array expression.</returns>
        public static NewArrayExpression InitNewArray(this Type type, params Expression[] initializers) =>
          Expression.NewArrayInit(type, initializers);
    }
}
