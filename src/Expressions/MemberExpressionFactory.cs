using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Resolution.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Expressions
{
    internal class MemberExpressionFactory
    {
        public IEnumerable<Expression> GetMemberExpressions(
            IEnumerable<MemberInfo> members,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression instance) =>
            from member in members
            let expression = GetMemberExpression(member, registrationContext, resolutionContext)
            where expression != null
            select instance.Member(member).AssignTo(expression);

        public IEnumerable<MemberBinding> GetMemberBindings(
            IEnumerable<MemberInfo> members,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext) =>
            from member in members
            let expression = GetMemberExpression(member, registrationContext, resolutionContext)
            where expression != null
            select member.AssignTo(expression);


        private static Expression GetMemberExpression(
            MemberInfo member,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext)
        {
            var memberTypeInfo = member.AsTypeInformation(registrationContext,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var injectionParameter = registrationContext.InjectionParameters.SelectInjectionParameterOrDefault(memberTypeInfo);
            if (injectionParameter != null) return injectionParameter;

            var memberExpression = resolutionContext.ResolutionStrategy.BuildExpressionForType(resolutionContext, memberTypeInfo);

            if (memberExpression != null || resolutionContext.NullResultAllowed) return memberExpression;

            var memberType = member is PropertyInfo ? "property" : "field";
            throw new ResolutionFailedException(memberTypeInfo.ParentType,
                $"Unresolvable {memberType}: ({memberTypeInfo.Type.FullName}){memberTypeInfo.ParameterOrMemberName}.");

        }
    }
}
