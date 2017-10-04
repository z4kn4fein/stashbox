#if NET45 || NET40 || NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal static class ExpressionEmitter
    {
        public static readonly Lazy<ModuleBuilder> ModuleBuilder = new Lazy<ModuleBuilder>(() =>
            CreateAssembly().DefineDynamicModule("Stashbox.Dynamic"));

        private static AssemblyBuilder CreateAssembly()
        {
#if NETSTANDARD1_3
            return AssemblyBuilder.DefineDynamicAssembly(
                 new AssemblyName("Stashbox.Dynamic"),
                 AssemblyBuilderAccess.Run);
#else
            return AppDomain.CurrentDomain.DefineDynamicAssembly(
                 new AssemblyName("Stashbox.Dynamic"),
                 AssemblyBuilderAccess.Run);
#endif
        }

#if NETSTANDARD1_3
        private static readonly MethodInfo DelegateTargetProperty = typeof(Delegate).GetPropertyGetMethod("Target");
#else
        private static readonly MethodInfo DelegateTargetProperty = typeof(Delegate).GetProperty("Target").GetGetMethod();
#endif

        public static bool TryEmit(this Expression expression, out Delegate resultDelegate, Type delegateType, Type returnType, params ParameterExpression[] parameters) =>
            expression.TryEmit(delegateType, returnType, out resultDelegate, out DelegateTargetInformation delegateTarget, parameters);

        public static bool TryEmit(this Expression expression, Type delegateType, Type returnType,
            out Delegate resultDelegate, out DelegateTargetInformation delegateTarget, params ParameterExpression[] parameters)
        {
            resultDelegate = null;
            delegateTarget = null;
            var constants = new Constants();

            if (!expression.TryCollectConstants(constants, parameters))
                return false;

            delegateTarget = DelegateTargetSelector.GetDelegateTarget(constants);

            var method = CreateDynamicMethod(delegateTarget, returnType, parameters);
            var generator = method.GetILGenerator();

            if (delegateTarget.Locals.Count > 0)
                DeclareLocals(generator, delegateTarget, constants);

#if NET45 || NET40

            //var dyn = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Emitted"), AssemblyBuilderAccess.Save, "c:\\temp");
            //var mod = dyn.DefineDynamicModule("Emitted", "Emitted.dll");
            //var typ = mod.DefineType("EmittedNS.EmittedType", TypeAttributes.Public);

            //var m = typ.DefineMethod("nojrwonfreonre", MethodAttributes.Public | MethodAttributes.Static, returnType, delegateTarget.Target.GetType().ConcatDelegateTargetParameter(parameters.GetTypes()));
            //var g = m.GetILGenerator();
            //if (!expression.TryEmit(delegateTarget, g, parameters))
            //    return false;

            //g.Emit(OpCodes.Ret);

            //var t = typ.CreateType();
            //dyn.Save("Emitted.dll");
#endif
            if (!expression.TryEmit(delegateTarget, generator, parameters))
                return false;

            generator.Emit(OpCodes.Ret);

            resultDelegate = delegateTarget.Target == null ? method.CreateDelegate(delegateType) : method.CreateDelegate(delegateType, delegateTarget.Target);
            return true;
        }

        private static DynamicMethod CreateDynamicMethod(DelegateTargetInformation target, Type returnType, params ParameterExpression[] parameters)
        {
            if (target.Target == null)
                return new DynamicMethod(string.Empty, returnType, parameters.GetTypes(), typeof(ExpressionEmitter), true);

            var targetType = target.Target.GetType();
            return new DynamicMethod(string.Empty, returnType, targetType.ConcatDelegateTargetParameter(parameters.GetTypes()), targetType, true);
        }

        private static Type[] GetTypes(this IList<ParameterExpression> parameters)
        {
            var count = parameters.Count;
            if (count == 0)
                return Stashbox.Constants.EmptyTypes;
            if (count == 1)
                return new[] { parameters[0].Type };

            var types = new Type[count];
            for (var i = 0; i < count; i++)
                types[i] = parameters[i].Type;
            return types;
        }

        private static Type[] ConcatDelegateTargetParameter(this Type targetType, Type[] parameters)
        {
            var count = parameters.Length;
            if (count == 0)
                return new[] { targetType };

            var types = new Type[count + 1];
            types[0] = targetType;

            if (count == 1)
                types[1] = parameters[0];
            if (count > 1)
                Array.Copy(parameters, 0, types, 1, count);

            return types;
        }

        private static void DeclareLocals(ILGenerator generator, DelegateTargetInformation target, Constants constants)
        {
            var length = constants.Locals.Count;
            for (var i = 0; i < length; i++)
                target.LocalBuilders.Add(generator.DeclareLocal(constants.Locals[i].Type));
        }

        private static bool TryEmit(this Expression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    return ((MethodCallExpression)expression).TryEmit(target, generator, parameters);
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).TryEmit(target, generator, parameters);
                case ExpressionType.Invoke:
                    return ((InvocationExpression)expression).TryEmit(target, generator, parameters);
                case ExpressionType.Parameter:
                    return ((ParameterExpression)expression).TryEmit(target, generator, parameters);
                case ExpressionType.Lambda:
                    return ((LambdaExpression)expression).TryEmit(target, generator, parameters);
                case ExpressionType.Convert:
                    return ((UnaryExpression)expression).TryEmit(target, generator, parameters);
                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).TryEmit(target, generator);
                case ExpressionType.MemberInit:
                    return ((MemberInitExpression)expression).TryEmit(target, generator, parameters);
                case ExpressionType.New:
                    return ((NewExpression)expression).TryEmit(target, generator, parameters);
                case ExpressionType.Block:
                    return ((BlockExpression)expression).TryEmit(target, generator, parameters);
                case ExpressionType.NewArrayInit:
                    return ((NewArrayExpression)expression).TryEmit(target, generator, parameters);
                case ExpressionType.Default:
                    return ((DefaultExpression)expression).TryEmit(generator);
            }

            return false;
        }

        private static bool TryEmit(this IList<Expression> expressions, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            for (var i = 0; i < expressions.Count; i++)
                if (!expressions[i].TryEmit(target, generator, parameters))
                    return false;

            return true;
        }

        private static bool TryEmit(this NewArrayExpression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            var type = expression.Type;
            var typeInfo = type.GetTypeInfo();
            var itemType = type.GetEnumerableType();

            var length = expression.Expressions.Count;

            generator.EmitInteger(length);
            generator.Emit(OpCodes.Newarr, itemType);

            for (var i = 0; i < length; i++)
            {
                generator.Emit(OpCodes.Dup);
                generator.EmitInteger(i);

                if (!expression.Expressions[i].TryEmit(target, generator, parameters))
                    return false;

                if (typeInfo.IsValueType)
                    generator.Emit(OpCodes.Stelem, itemType);
                else
                    generator.Emit(OpCodes.Stelem_Ref);
            }

            return true;
        }

        private static bool TryEmit(this DefaultExpression expression, ILGenerator il)
        {
            var type = expression.Type;

            if (type == typeof(void))
                return true;

            if (type == typeof(string))
                il.Emit(OpCodes.Ldnull);
            else if (type == typeof(bool) ||
                     type == typeof(byte) ||
                     type == typeof(char) ||
                     type == typeof(sbyte) ||
                     type == typeof(int) ||
                     type == typeof(uint) ||
                     type == typeof(short) ||
                     type == typeof(ushort))
                il.Emit(OpCodes.Ldc_I4_0);
            else if (type == typeof(long) ||
                     type == typeof(ulong))
            {
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Conv_I8);
            }
            else if (type == typeof(float))
                il.Emit(OpCodes.Ldc_R4, default(float));
            else if (type == typeof(double))
                il.Emit(OpCodes.Ldc_R8, default(double));
            else if (type.GetTypeInfo().IsValueType)
            {
                var lb = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldloca, lb);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc, lb);
            }
            else
                il.Emit(OpCodes.Ldnull);

            return true;
        }

        private static bool TryEmit(this ParameterExpression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            var index = Array.IndexOf(parameters, expression);
            if (index != -1) return generator.LoadParameter(target.Target == null ? index : index + 1);

            if (target.Locals != null)
            {
                var paramIndex = target.Locals.IndexOf(expression);
                if (paramIndex != -1)
                {
                    generator.Emit(OpCodes.Ldloc, target.LocalBuilders[paramIndex]);
                    return true;
                }
            }

            var constantIndex = target.Constants.IndexOf(expression);
            return constantIndex != -1 && generator.LoadConstantFromField(target, constantIndex);
        }

        private static bool TryEmit(this BlockExpression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            var length = expression.Expressions.Count;
            for (var i = 0; i < length; i++)
            {
                var expr = expression.Expressions[i];
                if (expr.NodeType == ExpressionType.Assign)
                {
                    var binary = (BinaryExpression)expr;

                    if (binary.Left is ParameterExpression parameterExpression)
                    {
                        var localIndex = target.Locals.IndexOf(parameterExpression);
                        if (localIndex == -1)
                            return false;

                        if (!binary.Right.TryEmit(target, generator, parameters))
                            return false;

                        generator.Emit(OpCodes.Stloc, target.LocalBuilders[localIndex]);
                    }

                    if (binary.Left is MemberExpression memberExpression)
                    {
                        if (!memberExpression.Expression.TryEmit(target, generator, parameters))
                            return false;

                        if (!binary.Right.TryEmit(target, generator, parameters))
                            return false;

                        if (memberExpression.Member is PropertyInfo property)
                        {
                            var setMethod = property.GetSetMethod();
                            if (setMethod == null)
                                return false;
                            generator.EmitMethod(setMethod);
                        }
                        else if (memberExpression.Member is FieldInfo field)
                            generator.Emit(OpCodes.Stfld, field);
                    }
                }
                else if (!expr.TryEmit(target, generator, parameters))
                    return false;
            }

            return true;
        }

        private static bool TryEmit(this MethodCallExpression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            if (expression.Object != null && !expression.Object.TryEmit(target, generator, parameters))
                return false;

            return expression.Arguments.TryEmit(target, generator, parameters) &&
                generator.EmitMethod(expression.Method);
        }

        private static bool TryEmit(this NewExpression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            if (!expression.Arguments.TryEmit(target, generator, parameters))
                return false;

            generator.Emit(OpCodes.Newobj, expression.Constructor);

            return true;
        }

        private static bool TryEmit(this UnaryExpression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            var typeFrom = expression.Operand.Type;
            var typeTo = expression.Type;

            if (!expression.Operand.TryEmit(target, generator, parameters))
                return false;

            if (typeFrom == typeTo)
                return true;

            if (!typeFrom.GetTypeInfo().IsValueType && typeTo.GetTypeInfo().IsValueType)
                generator.Emit(OpCodes.Unbox_Any, typeTo);
            else if (typeFrom.GetTypeInfo().IsValueType && typeTo == typeof(object))
                generator.Emit(OpCodes.Box, typeFrom);
            else
                generator.Emit(OpCodes.Castclass, typeTo);

            return true;
        }

        private static bool TryEmit(this LambdaExpression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            var constantIndex = target.Constants.IndexOf(expression);
            if (constantIndex == -1) return false;

            generator.LoadConstantFromField(target, constantIndex);

            LambdaTargetInformation lambda = null;
            var lambdaLength = target.Lambdas.Count;
            for (var i = 0; i < lambdaLength; i++)
                if (target.Lambdas[i].LambdaExpression.Equals(expression))
                    lambda = target.Lambdas[i];

            if (lambda == null) return false;

            if (lambda.DelegateTargetInfo.Parameters == null) return true;

            var length = lambda.DelegateTargetInfo.Parameters.Count;
            for (var i = 0; i < length; i++)
            {
                var param = lambda.DelegateTargetInfo.Parameters[i];

                generator.Emit(OpCodes.Dup);
                generator.EmitMethod(DelegateTargetProperty);

                var lambdaParamConstantIndex = lambda.DelegateTargetInfo.Constants.IndexOf(param);

                var paramIndex = Array.IndexOf(parameters, param);
                if (paramIndex == -1)
                {
                    var paramConstantIndex = target.Constants.IndexOf(param);
                    if (paramConstantIndex == -1) return false;

                    generator.LoadConstantFromField(target, paramConstantIndex);
                }
                else
                    generator.LoadParameter(paramIndex + 1);

                generator.Emit(OpCodes.Stfld, lambda.DelegateTargetInfo.ConstantFields[lambdaParamConstantIndex]);
            }

            return true;
        }

        private static bool TryEmit(this MemberInitExpression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            if (!expression.NewExpression.TryEmit(target, generator, parameters))
                return false;

            var length = expression.Bindings.Count;
            for (var i = 0; i < length; i++)
            {
                var binding = expression.Bindings[i];
                if (binding.BindingType != MemberBindingType.Assignment)
                    return false;

                generator.Emit(OpCodes.Dup);

                if (!((MemberAssignment)binding).Expression.TryEmit(target, generator, parameters))
                    return false;

                if (binding.Member is PropertyInfo property)
                {
                    var setMethod = property.GetSetMethod();
                    if (setMethod == null)
                        return false;
                    generator.EmitMethod(setMethod);
                }
                else if (binding.Member is FieldInfo field)
                    generator.Emit(OpCodes.Stfld, field);
            }

            return true;
        }

        private static bool TryEmit(this MemberExpression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            if (!expression.Expression.TryEmit(target, generator, parameters))
                return false;

            if (expression.Member is FieldInfo field)
            {
                generator.Emit(field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);
                return true;
            }
            else if (expression.Member is PropertyInfo property)
            {
                var getMethod = property.GetGetMethod();
                if (getMethod == null)
                    return false;
                generator.EmitMethod(getMethod);

                return true;
            }

            return false;
        }

        private static bool TryEmit(this InvocationExpression expression, DelegateTargetInformation target, ILGenerator generator, params ParameterExpression[] parameters)
        {
            if (!expression.Expression.TryEmit(target, generator, parameters) || !expression.Arguments.TryEmit(target, generator, parameters))
                return false;

            var invokeMethod = expression.Expression.Type.GetMethod("Invoke");
            generator.EmitMethod(invokeMethod);

            return true;
        }

        private static bool TryEmit(this ConstantExpression expression, DelegateTargetInformation target, ILGenerator generator)
        {
            var value = expression.Value;
            if (value == null)
            {
                generator.Emit(OpCodes.Ldnull);
                return true;
            }

            var type = expression.Type;

            if (type == typeof(int))
                generator.EmitInteger((int)value);
            else if (type == typeof(char))
                generator.EmitInteger((char)value);
            else if (type == typeof(short))
                generator.EmitInteger((short)value);
            else if (type == typeof(byte))
                generator.EmitInteger((byte)value);
            else if (type == typeof(ushort))
                generator.EmitInteger((ushort)value);
            else if (type == typeof(sbyte))
                generator.EmitInteger((sbyte)value);
            else if (type == typeof(long))
                generator.Emit(OpCodes.Ldc_I8, (long)value);
            else if (type == typeof(float))
                generator.Emit(OpCodes.Ldc_R8, (float)value);
            else if (type == typeof(double))
                generator.Emit(OpCodes.Ldc_R8, (double)value);
            else if (type == typeof(string))
                generator.Emit(OpCodes.Ldstr, (string)value);
            else if (type == typeof(bool))
                generator.Emit((bool)value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            else if (type == typeof(double))
                generator.Emit(OpCodes.Ldc_R8, (double)value);
            else if (type == typeof(Type))
            {
                generator.Emit(OpCodes.Ldtoken, type);
                generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            }
            else if (target != null)
            {
                var constantIndex = target.Constants.IndexOf(expression);
                return constantIndex != -1 && generator.LoadConstantFromField(target, constantIndex);
            }
            else
                return false;

            return true;
        }

        private static bool LoadConstantFromField(this ILGenerator generator, DelegateTargetInformation target, int constantIndex)
        {
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, target.ConstantFields[constantIndex]);

            return true;
        }

        public static bool LoadParameter(this ILGenerator generator, int index)
        {
            switch (index)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= byte.MaxValue)
                        generator.Emit(OpCodes.Ldarg_S, (byte)index);
                    else
                        generator.Emit(OpCodes.Ldarg, index);
                    break;
            }

            return true;
        }

        private static bool EmitMethod(this ILGenerator generator, MethodInfo info)
        {
            generator.Emit(info.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, info);
            return true;
        }

        private static void EmitInteger(this ILGenerator generator, int intValue)
        {
            switch (intValue)
            {
                case 0:
                    generator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    generator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    generator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    generator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    generator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    generator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    generator.Emit(OpCodes.Ldc_I4, intValue);
                    break;
            }
        }
    }
}

#endif