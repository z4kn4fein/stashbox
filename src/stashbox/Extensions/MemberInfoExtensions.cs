using System.Linq.Expressions;

namespace System.Reflection
{
    public static class MemberInfoExtensions
    {
        public static Action<object, object> GetMemberSetter(this MemberInfo memberInfo)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            var prop = memberInfo as PropertyInfo;
            if (prop != null)
            {
                var propertyExpression = Expression.Property(
                    Expression.Convert(instance, prop.DeclaringType), prop.Name);

                var setterExpression = Expression.Assign(propertyExpression,
                    Expression.Convert(value, prop.PropertyType));

                return Expression.Lambda<Action<object, object>>(
                    setterExpression, instance, value).Compile();
            }
            else
            {
                var field = memberInfo as FieldInfo;
                if (field == null)
                    throw new ArgumentException("Invalid argument, it must be a PropertyInfo or a FieldInfo type.",
                        nameof(memberInfo));

                var fieldExpression = Expression.Field(
                    Expression.Convert(instance, field.DeclaringType), field.Name);

                var setterExpression = Expression.Assign(fieldExpression,
                    Expression.Convert(value, field.FieldType));

                return Expression.Lambda<Action<object, object>>(
                    setterExpression, instance, value).Compile();
            }
        }
    }
}