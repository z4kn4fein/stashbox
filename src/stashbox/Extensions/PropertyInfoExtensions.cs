using System.Linq.Expressions;

namespace System.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static Action<object, object> GetPropertySetter(this PropertyInfo propertyInfo)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            var propertyExpression = Expression.Property(
                Expression.Convert(instance, propertyInfo.DeclaringType), propertyInfo.Name);

            var setterExpression = Expression.Assign(propertyExpression,
                Expression.Convert(value, propertyInfo.PropertyType));

            return Expression.Lambda<Action<object, object>>(
                setterExpression, instance, value).Compile();
        }
    }
}