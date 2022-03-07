using Stashbox.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Extensions
{
    internal static class InjectionParameterExtensions
    {
        public static Expression? SelectInjectionParameterOrDefault(this IEnumerable<KeyValuePair<string, object?>> injectionParameters,
            TypeInformation typeInformation)
        {
            var memberName = typeInformation.ParameterOrMemberName;
            var matchingParam = injectionParameters.FirstOrDefault(param => param.Key == memberName);
            if (matchingParam.Equals(default(KeyValuePair<string, object?>))) return null;

            if (matchingParam.Value == null)
                return typeInformation.Type == Constants.ObjectType
                    ? matchingParam.Value.AsConstant()
                    : matchingParam.Value.AsConstant().ConvertTo(typeInformation.Type);

            return matchingParam.Value.GetType() == typeInformation.Type
                ? matchingParam.Value.AsConstant()
                : matchingParam.Value.AsConstant().ConvertTo(typeInformation.Type);

        }
    }
}
