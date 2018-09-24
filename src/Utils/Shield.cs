using System;
using System.Linq.Expressions;

namespace Stashbox.Utils
{
    /// <summary>
    /// Represents a utility class for input validation.
    /// </summary>
    public static class Shield
    {
        private static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            var expressionBody = memberExpression.Body as MemberExpression;
            if (expressionBody == null)
                throw new ArgumentException("The given expression is not a MemberExpression.");

            return expressionBody.Member.Name;
        }

        /// <summary>
        /// Checks the value of the given object and throws an ArgumentNullException if it is null.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="memberExpression">The expression which returns with the object to be checked.</param>
        public static void EnsureNotNull<T>(Expression<Func<T>> memberExpression) where T : class
        {
            if (memberExpression == null)
                throw new ArgumentNullException(nameof(memberExpression));

            if (memberExpression.Compile()() == null)
                throw new ArgumentNullException(GetMemberName(memberExpression));
        }

        /// <summary>
        /// Checks the value of the given object and throws an ArgumentNullException if it is null.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="parameterName">The name of the parameter to be checked.</param>
        public static void EnsureNotNull<T>(T obj, string parameterName) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Checks the value of the given object and throws an ArgumentNullException with the given message if it is null.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="memberExpression">The expression which returns with the object to be checked.</param>
        /// <param name="message">The message to be shown in the exception.</param>
        public static void EnsureNotNull<T>(Expression<Func<T>> memberExpression, string message) where T : class
        {
            if (memberExpression == null)
                throw new ArgumentNullException(nameof(memberExpression));

            if (memberExpression.Compile()() == null)
                throw new ArgumentNullException(GetMemberName(memberExpression), message);
        }

        /// <summary>
        /// Checks the value of the given object and throws an ArgumentNullException with the given message if it is null.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="parameterName">The name of the parameter to be checked.</param>
        /// <param name="message">The message to be shown in the exception.</param>
        public static void EnsureNotNull<T>(T obj, string parameterName, string message) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(parameterName, message);
        }


        /// <summary>
        /// Checks the value of the given string and throws an ArgumentException if it is null or empty.
        /// </summary>
        /// <param name="memberExpression">The expression which returns with the string to be checked.</param>
        public static void EnsureNotNullOrEmpty(Expression<Func<string>> memberExpression)
        {
            if (memberExpression == null)
                throw new ArgumentNullException(nameof(memberExpression));

            if (string.IsNullOrWhiteSpace(memberExpression.Compile()()))
                throw new ArgumentException(string.Empty, GetMemberName(memberExpression));
        }

        /// <summary>
        /// Checks the value of the given string and throws an ArgumentException if it is null or empty.
        /// </summary>
        /// <param name="obj">The string to be checked.</param>
        /// <param name="parameterName">The name of the parameter to be checked.</param>
        public static void EnsureNotNullOrEmpty(string obj, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(obj))
                throw new ArgumentException(string.Empty, parameterName);
        }

        /// <summary>
        /// Checks two integers and throws an ArgumentException if the actual is lesser than the reference.
        /// </summary>
        /// <param name="actual">The actual value.</param>
        /// <param name="reference">The reference value.</param>
        public static void EnsureGreaterThan(int actual, int reference)
        {
            if (reference >= actual)
                throw new ArgumentException($"The given number is lesser or equal than: {reference}");
        }

        /// <summary>
        /// Checks a bool condition and throws an ArgumentException if it is false.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="message">Exception message.</param>
        public static void EnsureTrue(bool condition, string message)
        {
            if (!condition)
                throw new ArgumentException(message);
        }

        /// <summary>
        /// Checks the type of the given object and throws an ArgumentException if it doesn't matches with the given type parameter.
        /// </summary>
        /// <typeparam name="TType">The type parameter.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        public static void EnsureTypeOf<TType>(object obj)
        {
            if (!(obj is TType))
                throw new ArgumentException(nameof(obj));
        }
    }
}
