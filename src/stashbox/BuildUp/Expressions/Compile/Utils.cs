using System;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal static class Utils
    {
        public static Type[] ConcatCapturedArgumentWithParameterWithReturnType(Type[] parameters, Type capturedArgumentsType, Type returnType)
        {
            var count = parameters.Length;
            if (count == 0)
                return new[] { capturedArgumentsType, returnType };

            var length = count + 2;
            var types = new Type[length];
            types[0] = capturedArgumentsType;

            if (count == 1)
                types[1] = parameters[0];
            if (count > 1)
                Array.Copy(parameters, 0, types, 1, count);

            types[length - 1] = returnType;

            return types;
        }

        public static Type GetFuncType(Type[] paramTypes)
        {
            switch (paramTypes.Length)
            {
                case 1: return typeof(Func<>).MakeGenericType(paramTypes);
                case 2: return typeof(Func<,>).MakeGenericType(paramTypes);
                case 3: return typeof(Func<,,>).MakeGenericType(paramTypes);
                case 4: return typeof(Func<,,,>).MakeGenericType(paramTypes);
                case 5: return typeof(Func<,,,,>).MakeGenericType(paramTypes);
                case 6: return typeof(Func<,,,,,>).MakeGenericType(paramTypes);
                case 7: return typeof(Func<,,,,,,>).MakeGenericType(paramTypes);
                case 8: return typeof(Func<,,,,,,,>).MakeGenericType(paramTypes);
                default:
                    throw new NotSupportedException(
                        string.Format("Func with so many ({0}) parameters is not supported!", paramTypes.Length));
            }
        }
    }
}
