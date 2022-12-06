using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Resolution.Extensions;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Expressions
{
    internal static partial class ExpressionFactory
    {
        private static IEnumerable<Expression> GetMemberExpressions(
            IEnumerable<MemberInfo> members,
            ServiceRegistration? serviceRegistration,
            ResolutionContext resolutionContext,
            Expression instance,
            TypeInformation typeInformation) =>
            from member in members
            let expression = GetMemberExpression(member, serviceRegistration, resolutionContext, typeInformation)
            where expression != null
            select instance.Member(member).AssignTo(expression);

        private static IEnumerable<MemberBinding> GetMemberBindings(
            IEnumerable<MemberInfo> members,
            ServiceRegistration? serviceRegistration,
            ResolutionContext resolutionContext,
            TypeInformation typeInformation) =>
            from member in members
            let expression = GetMemberExpression(member, serviceRegistration, resolutionContext, typeInformation)
            where expression != null
            select member.AssignTo(expression);


        private static Expression GetMemberExpression(
            MemberInfo member,
            ServiceRegistration? serviceRegistration,
            ResolutionContext resolutionContext,
            TypeInformation typeInformation)
        {
            var memberTypeInfo = member.AsTypeInformation(serviceRegistration, typeInformation,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var injectionParameter = serviceRegistration?.Options.GetOrDefault<ExpandableArray<KeyValuePair<string, object?>>>(RegistrationOption.InjectionParameters)?.SelectInjectionParameterOrDefault(memberTypeInfo);
            if (injectionParameter != null) return injectionParameter;

            var serviceContext = resolutionContext.CurrentContainerContext
                .ResolutionStrategy.BuildExpressionForType(resolutionContext, memberTypeInfo);

            if (!serviceContext.IsEmpty() || resolutionContext.NullResultAllowed) return serviceContext.ServiceExpression;

            var memberType = member is PropertyInfo ? "property" : "field";
            throw new ResolutionFailedException(memberTypeInfo.ParentType, memberTypeInfo.DependencyName,
                $"Unresolvable {memberType}: ({memberTypeInfo.Type.FullName}){memberTypeInfo.ParameterOrMemberName}.");

        }
    }
}
